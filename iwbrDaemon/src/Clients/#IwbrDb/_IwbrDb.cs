
using System.Reflection;
using MongoDB.Driver;
using IwbrDaemon.Types;
using IwbrDaemon.Trading;
using IwbrDaemon.Trading.Analysis;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static string ConnectionString;

        private static MongoClient Client;
        private static IMongoDatabase Database;

        private static class Collection
        {
            public static IMongoCollection<CloseAllInfo> CloseAllHistory = Database.GetCollection<CloseAllInfo>("closeAllInfo");
            public static IMongoCollection<KLine> KLines = Database.GetCollection<KLine>("klines");
            public static IMongoCollection<Order> Orders = Database.GetCollection<Order>("orders");
            public static IMongoCollection<TradingPair> TradingPairs = Database.GetCollection<TradingPair>("tradingPairs");
            public static IMongoCollection<Ticker> Tickers = Database.GetCollection<Ticker>("tickers");
            public static IMongoCollection<SymbolAnalysis> SymbolsAnalysis = Database.GetCollection<SymbolAnalysis>("symbolsAnalysis");
            public static IMongoCollection<Position> Positions = Database.GetCollection<Position>("positions");
            public static IMongoCollection<WalletStatus> WalletStatus = Database.GetCollection<WalletStatus>("walletStatus");
        }

        static IwbrDb()
        {

            ConnectionString += $"mongodb://{Config.MongoDb.Username}:{Config.MongoDb.Password}@{Config.MongoDb.Host}:{Config.MongoDb.Port}/{Config.MongoDb.Database}?maxPoolSize=1500&minPoolSize=30";
            Client = new MongoClient(ConnectionString);
            Database = Client.GetDatabase(Config.MongoDb.Database);

            log.Info("MongoDB connection loaded.");
        }

    }
}