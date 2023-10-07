using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace mongodb_dotnet_example.Models
{
    public class Train
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Number { get; set; }

        public string Depatre_Station { get; set; }

        public string Arrival_Station { get; set; }

        public DateTime Depatre_Time { get; set; }

        public DateTime Arrival_Time { get; set; }

        public string Status { get; set; }
        public string NIC { get; internal set; }
    }
}
