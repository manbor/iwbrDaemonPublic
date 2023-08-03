using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Analysis.Indicators
{
    [BsonIgnoreExtraElements]
    public class PivotPoints
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        public PivotPoints(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}