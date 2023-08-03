using IwbrDaemon.Types.Exchange;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using WebSocketSharp;
using RestSharp;
using IwbrDaemon.Trading;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        // ---------------------- WebService  ------------------------------

        private static void _tickerWSLoader() {
            string methodName = $"_tickerWSLoader".PadRight(Config.PadRightMethodName);
            
            try{
                while (true)
                {
                    var combinedWSRequest = TradingPairs
                                            .OrderBy(p => p.Symbol)
                                            .Select(p => $"{p.Symbol.ToLower()}@ticker")
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

                            var json = JObject.Parse(e.Data);

                            var ticker = new Ticker( json["data"]["s"].ToString()                                    
                                                    ,decimal.Parse(json["data"]["c"].ToString())
                                                    ,decimal.Parse(json["data"]["P"].ToString())
                                                    ,DateTime.UtcNow);                   

                            //var symbolAllowed =  Binance.TradingPairs.Where(p => p.Symbol == ticker.Symbol).Count() > 0;
                            //if(!symbolAllowed)
                            //    return;

                            IwbrDb.MergeTicker(ticker);



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

        public static void TickerWSLoader(){
            string methodName = $"TickerWSLoader".PadRight(Config.PadRightMethodName);

            while(!TradingPairsIsLoaded);
            //while(!RecentKLinesLoaded);

            try {
                var threadName = "WSTicker";

                if (!Threads.ContainsKey(threadName))
                {
                    Threads.TryAdd(  threadName, new Thread(new ThreadStart(() => _tickerWSLoader())) );
                    Threads[threadName].Start();
                }

                if (!Threads[threadName].IsAlive )
                {
                    Thread.Sleep(5000); 
                    Threads[threadName] =  new Thread(new ThreadStart(() => _tickerWSLoader()));
                    Threads[threadName].Start();
                }    
            }
            catch(System.Threading.ThreadStateException e){
                return;
            } 

            if(Config.DebugEnable) log.Debug($"{methodName} executed");
        }



    }
}
