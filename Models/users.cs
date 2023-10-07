using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace mongodb_dotnet_example.Models
{

    public class Users
    {
        [BsonId]
        public string NIC { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        public string Address { get; set; }

        public string Role { get; set; }

        public string ContactNumber { get; set; }

        public string Password { get; set; }

        public string Status { get; set; }

        public Boolean IsApprove { get; set; }
    }
}
