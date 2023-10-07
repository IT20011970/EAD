using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace mongodb_dotnet_example.Models
{

    public class Reservation
    {
        [BsonId]
        public string NIC { get; set; }

        public DateTime TodayDate { get; set; }

        public List<Train> trains { get; set; }
    }
}
