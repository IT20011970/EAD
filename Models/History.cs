using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace mongodb_dotnet_example.Models
{

    public class History
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string NIC { get; set; }
        public string TrainId { get; set; }
        public string Number { get; set; }
        public string Departre_Station { get; set; }

        public string Arrival_Station { get; set; }

        public DateTime Departre_Time { get; set; }

        public DateTime Arrival_Time { get; set; }

        public DateTime Reserverved_Time { get; set; }
    }
}
