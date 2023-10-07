using mongodb_dotnet_example.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using mongodb_dotnet_example.Exceptions;
using MongoDB.Bson;

namespace mongodb_dotnet_example.Services
{
    public class UserService
    {
        private readonly IMongoCollection<Users> _users;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<Users>(settings.GamesCollectionName);
        }
        public List<Users> Get() => _users.Find(game => true).ToList();

       
        public List<Users> GetTravellers()
        {
            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(user => user.Role, "traveller"),
                Builders<Users>.Filter.Eq(user => user.Status, "inactive"),
                Builders<Users>.Filter.Eq(user => user.IsApprove, false)
            );

            var users = _users.Find(filter).ToList();
            return users;
        }

        public List<Users> GetTravellerProfile()
        {
            var filter = Builders<Users>.Filter.And(
               Builders<Users>.Filter.Eq(user => user.Role, "traveller")
           );
            var users = _users.Find(filter).ToList();
            return users;
        }


        public Users Login(Users user)
        {
            var DBUser = Get(user.NIC);

            if (DBUser != null)
            {
                if (DBUser.Password == user.Password)
                {
                    return DBUser; // Authentication successful, return the user object
                }
                else
                {
                    throw new InvalidPasswordException("Invalid password");// Password mismatch, handle the error (e.g., throw an exception)
                }
            }
            else
            {
                throw new InvalidUserException("Invalid User"); // User not found in the database, handle the error (e.g., throw an exception)
            }

            return null;
        }
        public Users Get(string id) => _users.Find(game => game.NIC == id).FirstOrDefault();

        public Users Create(Users game)
        {
            _users.InsertOne(game);
            return game;
        }

        public Users Update(string NIC, Users updatedGame)
        {
            updatedGame.NIC = NIC; 
            _users.ReplaceOne(game => game.NIC == NIC, updatedGame);
            return updatedGame;
        }

        public void Delete(Users gameForDeletion) => _users.DeleteOne(game => game.NIC == gameForDeletion.NIC);

        public void Delete(string id) => _users.DeleteOne(game => game.NIC == id);
    }
}