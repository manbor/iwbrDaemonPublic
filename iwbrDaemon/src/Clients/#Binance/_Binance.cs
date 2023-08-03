using System.Collections.Concurrent;
using System.Reflection;
using RestSharp;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        private static System.Type thisType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(thisType);

        protected static readonly string _baseUrlRestApi = "https://api.binance.com";
        protected static readonly string _baseUrlWebSocket = "wss://stream.binance.com:9443";
        protected static readonly string _baseUrlWebSocketCombining = "wss://stream.binance.com:9443/stream?streams=";

        protected static string _ApiKey { get; private set; }
        protected static string _ApiSecretKey { get; private set; }
        protected static readonly RestClient client;
        public static bool isBoot = true;
        private static ConcurrentDictionary<string, Thread> Threads = new ConcurrentDictionary<string, Thread>();

        static Binance()
        {
            _ApiKey = Config.Binance.ApiKey;
            _ApiSecretKey = Config.Binance.SecretKey;

            client = new RestClient(_baseUrlRestApi);

            log.Info("Binance loaded");
        }

    }
}