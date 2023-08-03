using IwbrDaemon.Types.Exchange;
using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types
{
    [BsonIgnoreExtraElements]
    public class CloseAllInfo
    {
        [BsonElement("creationTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("balance")]
        public ActualBalance Balance {get; private set;}


        public CloseAllInfo(DateTime creationTime, ActualBalance balance)
        {
            CreationTime = creationTime;
            Balance = balance;
        }
    }
}
