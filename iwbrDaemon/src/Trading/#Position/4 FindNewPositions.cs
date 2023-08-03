using System.Collections.Concurrent;
using IwbrDaemon.Clients;
using IwbrDaemon.Trading.Analysis;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private static ConcurrentBag<SymbolAnalysis> SymAnalList ;
                
        private void _findNewPositions(string symbol, DateTime schedulerTime)
        {
            string methodName = "_findNewPositions".PadRight(Config.PadRightMethodName);
        }

        public static void FindNewPositions()
        {
            string methodName = $"FindNewPositions".PadRight(Config.PadRightMethodName);

            if(IsClosingAllForProfit)
                return;

            if(Config.DebugEnable) log.Debug($"{methodName}start");

            var actualBalce = IwbrDb.GetActualBalance();
            if( actualBalce <= Config.Position.MinWalletBalance) {
                log.Debug($"No balance for open new positions. {actualBalce} / {Config.Position.MinWalletBalance}");
                return;
            }

            SymAnalList = new ConcurrentBag<SymbolAnalysis>();
            var executionTime = DateTime.UtcNow;

            foreach(var pair in Binance.TradingPairs)
            {
                Thread t = new Thread(new ThreadStart(() =>   {
                                                                SymAnalList.Add( new SymbolAnalysis(pair.Symbol, executionTime));
                                                                }
                                                    )
                                    );
                t.Start();
                Thread.Sleep(100);
            }

            Thread.Sleep(10000);

            //---------------------------------------------------------------------------------------------------------------------
            var symbolToOpen = SymAnalList
                                .Where(p => p.PositionAction == ePositionAction.Open && !Position.IsAlreadyOpen(p.Symbol))
                                .OrderBy(p => p.OrderResult )
                                ;

            if(Config.Position.FindHighVolatility){
                symbolToOpen = symbolToOpen
                                .Where(p => p.Volatility > 0.85m)
                                .OrderByDescending(p => p.Volatility);
            } else  {
                symbolToOpen = symbolToOpen.OrderBy(p => p.Volatility);
            }


            // TODO remove this filter symbolToOpen = Config.Position.FindFirstLong ? symbolToOpen.OrderByDescending(p => p.PositionType) : symbolToOpen.OrderBy(p => p.PositionType);
            //---------------------------------------------------------------------------------------------------------------------

            log.Debug($"found {symbolToOpen.Count()} possible positions");
            
            if(!Position.MaxPositionsReached()){

                var posRecentClosedWithNoProfit= IwbrDb.GetRecentClosedPositionsNoProfit(null, 5*60, false);

                foreach(var pos in posRecentClosedWithNoProfit){
                    
                    if((DateTime.UtcNow - pos.LastUpdateTime).TotalMinutes > 30)
                        continue;
                        
                    try {
                        var type = pos.Type==ePositionType.Long?ePositionType.Short:ePositionType.Long;
                        new Position(pos.Symbol, type );     
                    } catch (System.InvalidOperationException e) {

                    }

                    if(Position.MaxPositionsReached())
                        break;
                }
            }

            if(!Position.MaxPositionsReached()){
                foreach(var analysis in symbolToOpen){

                    try {
                        new Position(analysis);     
                    } catch (System.InvalidOperationException e) {

                    }

                    if(Position.MaxPositionsReached())
                        break;
                }
            }

            if(Config.DebugEnable) log.Debug($"{methodName}finished");

        }
    }
}
