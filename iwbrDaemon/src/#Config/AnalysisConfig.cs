using Newtonsoft.Json.Linq;

namespace IwbrDaemon
{

    public static partial class Config
    {
        public class AnalysisConfig {
            public IndicatorsConfig Indicators { get; private set; }

            public AnalysisConfig(JToken configAnalysis) {
                Indicators = new IndicatorsConfig(configAnalysis["Indicators"]);
            }
        }
    }
}