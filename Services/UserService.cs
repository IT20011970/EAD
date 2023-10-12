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

            _users = database.GetCollection<Users>(settings.UserCollectionName);
        }
        public List<Users> Get() => _users.Find(users => true).ToList();

       
        public List<Users> GetTravellers()
        {
            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(user => user.Role, "traveller"),
                Builders<Users>.Filter.Eq(user => user.Status, "inactive")
            );

            var users = _users.Find(filter).ToList();
            return users;
        }

        public List<Users> GetActiveTravellers()
        {
            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(user => user.Role, "traveller"),
                Builders<Users>.Filter.Eq(user => user.Status, "active")  
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
                    if (DBUser.Role != "traveller")
                    {
                        return DBUser; // Authentication successful, return the user object
                    }
                    else
                    {
                        if (DBUser.Status == "active")
                        {
                            return DBUser;
                        }
                        else
                        {
                            throw new CustomException("Traveller not allowed to login");
                        }
                       
                    }
                    
                }
                else
                {
                    throw new CustomException("Invalid password");// Password mismatch, handle the error (e.g., throw an exception)
                }
            }
            else
            {
                throw new InvalidUserException("Invalid User"); // User not found in the database, handle the error (e.g., throw an exception)
            }

            return null;
        }
        public Users Get(string id) => _users.Find(users => users.NIC == id).FirstOrDefault();

        public Users Create(Users user)
        {
            _users.InsertOne(user);
            return user;
        }

        public Users Update(string NIC, Users updatedUser)
        {
            var user = _users.Find(userObj => userObj.NIC == NIC).FirstOrDefault();
            updatedUser.NIC = NIC;
            if (updatedUser.Status == "active") { 
                user.Name = updatedUser.Name;
                user.Address = updatedUser.Address;
                user.ContactNumber = updatedUser.ContactNumber;
            }else{
                user.Status = updatedUser.Status;
            }
            _users.ReplaceOne(userObj => userObj.NIC == NIC, user);
            return updatedUser;
        }



        public void Delete(Users userForDeletion) => _users.DeleteOne(game => game.NIC == userForDeletion.NIC);

        public void Delete(string id) => _users.DeleteOne(game => game.NIC == id);
    }
}