using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using WebSocketSharp;
using RestSharp;
using IwbrDaemon.Trading;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {

        public static readonly int KLinesRefreshMinutes = 2;

        //protected static readonly string[] KLineIntervals = { "1s", "1m", "5m", "15m", "30m", "1h", "4h", "1d", "1M" };
        public static readonly string[] KLineIntervals = { "1m", "5m", "30m", "1h" };
        public static bool RecentKLinesIsRefreshing { get; private set; } = false;
        public static bool RecentKLinesLoaded { get; private set; } = false;
        private static int RecentKLinesThreadsLoaded;

        //private static ConcurrentDictionary<(string,string), object> SymIntLock = new ConcurrentDictionary<(string,string), object>();
        private static ConcurrentQueue<string> RecentKLinesQueue;

        // ---------------------- WebService  ------------------------------

        private static void _WSKLine(string interval) {
            string methodName = $"_WSKLine {interval}".PadRight(Config.PadRightMethodName);
            try{
                while (true)
                {
                    var combinedWSRequest = TradingPairs
                                            .OrderBy(p => p.Symbol)
                                            .Select(p => $"{p.Symbol.ToLower()}@kline_{interval}")
                                            .ToList();

                    string callUrl = _baseUrlWebSocketCombining + string.Join("/", combinedWSRequest);

                    using (var ws = new WebSocket(callUrl))
                    {
                        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

                        string reasonOfClose = null;

                        ws.OnError += (sender, e) =>
                        {
                            // gestione degli errori
                            log.Error($"{methodName} WS error: {e.Message}");
                        };
                        ws.OnOpen += (sender, e) =>
                        {
                            log.Info($"{methodName} WS opened");
                        };
                        ws.OnClose += (sender, e) =>
                        {
                            reasonOfClose = $"WS closed with code {e.Code} and reason {e.Reason}";
                        };

                        ws.OnMessage += (sender, e) =>
                        {
                            //if(Binance.RecentKLinesIsRefreshing) return;

                            var json = JObject.Parse(e.Data);

                            KLine tmpKLine = new KLine(json["data"]["k"]["s"].ToString() // symbol
                                                , json["data"]["k"]["i"].ToString() // interval 
                                                , long.Parse(json["data"]["k"]["t"].ToString()) // open time 
                                                , decimal.Parse(json["data"]["k"]["o"].ToString()) // open price
                                                , decimal.Parse(json["data"]["k"]["h"].ToString()) // high price
                                                , decimal.Parse(json["data"]["k"]["l"].ToString()) // low price
                                                , decimal.Parse(json["data"]["k"]["c"].ToString()) // close price
                                                , decimal.Parse(json["data"]["k"]["v"].ToString()) // volume
                                                , long.Parse(json["data"]["k"]["T"].ToString()) // close time
                                                , decimal.Parse(json["data"]["k"]["q"].ToString()) // quote volume
                                                , int.Parse(json["data"]["k"]["n"].ToString()) // trade count
                                                , decimal.Parse(json["data"]["k"]["V"].ToString()) // taker buy base volume
                                                , decimal.Parse(json["data"]["k"]["Q"].ToString()) // taker buy base volume
                                                , Boolean.Parse(json["data"]["k"]["x"].ToString()) // is kline closed?
                                                , "WS"
                                                );
                                                
                            if (tmpKLine.Interval != "1s") {
                                IwbrDb.MergeKLine(tmpKLine);
                            }

                            //if(Config.DebugEnable) log.Debug($"Kline {tmpKLine.Symbol} {tmpKLine.Interval} {tmpKLine.ClosePrice} received");

                            if(!Binance.UserAssetsIsLoaded) return;

                            new Thread(new ThreadStart(() => {
                                    if (tmpKLine.Interval == "1m") {
                                        if ( Position.IsAlreadyOpen(tmpKLine.Symbol) ) {

                                            IwbrDb.GetOpenPosition(tmpKLine.Symbol).Check();
                                        } 
                                        //else if( !Position.MaxPositionsReached() ) {
                                        //    Context.ExecuteSymbolAnalysis(tmpKLine.Symbol, DateTime.UtcNow);
                                        //}                           
                                    }
                                }
                            )).Start();


                        };

                        try{
                            ws.Connect();
                        }
                        catch(System.PlatformNotSupportedException e) {
                            continue;
                        }

                        while( ws.Ping() );

                        if(Config.DebugEnable) log.Debug($"{methodName} close. Reason: {reasonOfClose}");
                    }

                    Thread.Sleep(100);
                }
            }
            catch(System.OutOfMemoryException ex){
                log.Error($"{methodName}: {ex.ToString()}");
                System.Environment.Exit(1);
            }
        }

        public static void KLineWSLoader(){
            string methodName = $"KLineWSLoader".PadRight(Config.PadRightMethodName);

            while(!TradingPairsIsLoaded);
            //while(!RecentKLinesLoaded);

            foreach (var interval in KLineIntervals.Where(p => p=="1m")) //
            {
                try {
                    var threadName = WSKLineGetTaskName(interval);

                    if (!Threads.ContainsKey(threadName))
                    {
                        Threads.TryAdd(  threadName, new Thread(new ThreadStart(() => _WSKLine(interval))) );
                        Threads[threadName].Start();
                    }

                    if (!Threads[threadName].IsAlive )
                    {
                        Thread.Sleep(5000); 
                        Threads[threadName] =  new Thread(new ThreadStart(() => _WSKLine(interval)));
                        Threads[threadName].Start();
                    }    
                }
                catch(System.Threading.ThreadStateException e){
                    continue;
                } 
            }

            if(Config.DebugEnable) log.Debug($"{methodName} executed");
        }

        // ---------------------- Recent history Reloading ------------------------------

        private static void _loadRecentKLines(string symbol, string interval) {
            string methodName = $"_loadRecentKLines".PadRight(Config.PadRightMethodName) + $"{symbol.PadLeft(10)} {interval.PadLeft(5)}";

            RestRequest request = new RestRequest("/api/v3/klines", Method.Get);

            long endTime = Utils.GetCurrentTimestamp();
            long startTime = 0;

            long KLinesSize = !RecentKLinesLoaded?Config.Binance.RecentKLinesSize:(KLinesRefreshMinutes*10);


            switch (interval) {
                case "1m": startTime  = endTime -KLinesSize * (long)(1 * 60 * 1000); break;
                case "5m": startTime  = endTime - KLinesSize * (long)(5 * 60 * 1000); break;
                case "30m": startTime = endTime - KLinesSize * (long)(30 * 60 * 1000); break;
                case "1h": startTime  = endTime - KLinesSize * (long)(1 * 60 * 60 * 1000); break;
            }

            request.AddParameter("symbol", symbol);
            request.AddParameter("interval", interval);
            request.AddParameter("startTime", startTime);
            request.AddParameter("endTime", endTime);
            request.AddParameter("limit",  KLinesSize  );

            var response = ApiCall(request, AuthLevel.NoLogin);

            if (!response.IsOk)
            {
                log.Error($"{methodName} {symbol} {interval} error on ApiCall");
                return;
            }

            var tmpArray = JArray.Parse(response.HttpResponse.Content);

            List<KLine> _KLineList = tmpArray
                                    .Select(p => KLine.FromJArray(symbol, interval, p.ToObject<JArray>(), "API", true))
                                    .ToList();


            // lock(SymIntLockGetObj(symbol, interval) ) {
                IwbrDb.MergeKLines(_KLineList);

                DateTime dt;

                try { 
                    dt =  _KLineList.First().CreationTime;
                } catch(Exception e) {
                    dt = DateTime.UtcNow;
                }

                //Context.ExecuteAnalysis(symbol, dt);
            //}

            bool status = RecentKLinesQueue.TryDequeue(out _);
            if (!status)
            {
                string msg = $"{methodName} failed on RecentKLinesQueue.TryDequeue";
                log.Error(msg);
                throw new Exception(msg);
            }

            //TODO add flag debug in config files
            if(Config.DebugEnable) log.Debug($"{methodName} {_KLineList.Count()} KLines fetched. {RecentKLinesThreadsLoaded} threads loaded. {RecentKLinesQueue.Count} still running");
        }

        public static void LoadRecentKLines()
        {
            string methodName = $"LoadRecentKLines".PadRight(Config.PadRightMethodName);

            if (Config.Binance.RecentKLinesSize == 0)
            {
                RecentKLinesLoaded = true;
                return;
            }

            /*
             * -----------------------------------------
             *  wait until TradingPairs is not empty
             * -----------------------------------------
             */
            while (TradingPairsIsEmpty()) ;

            /*
             * -----------------------------------------
             *  load threads
             * -----------------------------------------
             */
            log.Debug($"{methodName}start");

            RecentKLinesQueue = new ConcurrentQueue<string>();

            RecentKLinesIsRefreshing = true;

            RecentKLinesThreadsLoaded = 0;
            foreach ( var pair in TradingPairs.OrderBy(p => p.Symbol) )
            {
                foreach (var interval in KLineIntervals.Where(p => p!="1s"))
                {
                    var key = $"{pair.Symbol}-{interval}";

                    Thread t = new Thread(new ThreadStart(() => _loadRecentKLines(pair.Symbol, interval)));

                    RecentKLinesQueue.Enqueue(key);

                    t.Start();
                    RecentKLinesThreadsLoaded++;

                    Thread.Sleep(50);
                }
            }

            /*
             * ------------------------------------------
             *  wait until all fetching will be finished
             * ------------------------------------------
             */
            while (RecentKLinesQueue.Count > 0) ;

            RecentKLinesIsRefreshing = false;
            RecentKLinesLoaded = true;

            log.Debug($"{methodName}finished");
        }

        // ---------------------- Internal functions ------------------------------
        private static string WSKLineGetTaskName(string interval) {
            return $"WSKLines-{interval}";
        }

    }
}
