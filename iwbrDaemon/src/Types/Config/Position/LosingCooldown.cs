using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Position
{
    [BsonIgnoreExtraElements]
    public class LosingCooldown
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("minutes")]
        public int Minutes { get; private set; }

        [BsonElement("startFrom")]
        public decimal StartFrom { get; private set; }      

        public LosingCooldown(bool isEnabled, int minutes, decimal startFrom)
        {
            IsEnabled = isEnabled;
            Minutes = minutes;
            StartFrom = startFrom;
        }
    }
}