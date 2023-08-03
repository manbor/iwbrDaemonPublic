using Newtonsoft.Json.Linq;

namespace IwbrDaemon
{

    public static partial class Config
    {
        public static IwbrDaemon.Types.Config.Binance Binance { get; private set; }
        public static IwbrDaemon.Types.Config.MongoDb MongoDb { get; private set; }
        public static IwbrDaemon.Types.Config.CLI CLI { get; private set; }
        public static List<string> Blacklist { get; private set; }
        public static int PadRightMethodName { get; private set; }
        public static AnalysisConfig Analysis { get; private set; }
        public static PositionConfig Position { get; private set; }
        public static Boolean DebugEnable {get; private set;}


        static Config()
        {
            LoadConfig();          
        }

        public static void LoadConfig(){
            var configJson = JObject.Parse(File.ReadAllText("config.json"));

            Binance = configJson["Binance"].ToObject<IwbrDaemon.Types.Config.Binance>();
            MongoDb = configJson["MongoDb"].ToObject<IwbrDaemon.Types.Config.MongoDb>();
            CLI = configJson["CLI"].ToObject<IwbrDaemon.Types.Config.CLI>();
            Blacklist = configJson["Blacklist"].ToObject<List<string>>();
            DebugEnable = configJson["DebugEnable"].ToObject<Boolean>();

            PadRightMethodName = 35;

            Analysis = new AnalysisConfig(configJson["Analysis"]);
            Position = new PositionConfig(configJson["Position"]);

        }


    }






}