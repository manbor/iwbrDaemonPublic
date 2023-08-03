using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Position
{
    [BsonIgnoreExtraElements]
    public class FreezeCooldown
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("minutes")]
        public int Minutes { get; private set; }

        [BsonElement("acceptableError")]
        public decimal AcceptableError { get; private set; }      

        public FreezeCooldown(bool isEnabled, int minutes, decimal acceptableError)
        {
            IsEnabled = isEnabled;
            Minutes = minutes;
            AcceptableError = acceptableError;
        }
    }
}