using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerceSystem.Shared.Documents
{
    public class ProductDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int ProductId { get; set; }

        public string Name { get; set; }
        public DateTime LastSynced { get; set; }
    }
}