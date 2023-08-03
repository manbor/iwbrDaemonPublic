using IwbrDaemon.Types.Config.Analysis.Indicators;
using Newtonsoft.Json.Linq;

namespace IwbrDaemon
{
    public static partial class Config
    {
        public class IndicatorsConfig
        {
            public PivotPoints PivotPoints { get; private set; }
            public ADX ADX { get; private set; }
            public BollingerBands BollingerBands { get; private set; }
            public MACD MACD { get; private set; }
            public RSI RSI { get; private set; }
            public SMA SMA { get; private set; }
            public Stochastic Stochastic { get; private set; }

            public IndicatorsConfig(JToken indicatorsConfig)
            {
                PivotPoints = indicatorsConfig["PivotPoints"].ToObject<PivotPoints>();

                ADX = indicatorsConfig["ADX"].ToObject<ADX>();

                BollingerBands = indicatorsConfig["BollingerBands"].ToObject<BollingerBands>();

                MACD = indicatorsConfig["MACD"].ToObject<MACD>();

                RSI = indicatorsConfig["RSI"].ToObject<RSI>();

                SMA = indicatorsConfig["SMA"].ToObject<SMA>();

                Stochastic = indicatorsConfig["Stochastic"].ToObject<Stochastic>();
            }
        }
    }
}