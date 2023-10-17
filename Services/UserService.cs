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
                Builders<Users>.Filter.Eq(user => user.Role, "traveller"), //filter traveller
                 Builders<Users>.Filter.Eq(user => user.IsApprove, true),//filter isapproved
                Builders<Users>.Filter.Eq(user => user.Status, "inactive")//filter status
            );

            var users = _users.Find(filter).ToList();
            return users; //return object
        }

        public List<Users> GetActiveTravellers()
        {
            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(user => user.Role, "traveller"),//filter traveller
                 Builders<Users>.Filter.Eq(user => user.IsApprove, true),//filter isapproved
                Builders<Users>.Filter.Eq(user => user.Status, "active")  //filter status
            );

            var users = _users.Find(filter).ToList();
            return users;//return object
        }

        public List<Users> GetTravellerProfile()
        {
            var filter = Builders<Users>.Filter.And(
               Builders<Users>.Filter.Eq(user => user.Role, "traveller"),//filter traveller
               Builders<Users>.Filter.Eq(user => user.IsApprove, true)//filter isapproved
           );
            var users = _users.Find(filter).ToList();
            return users;
        }


        public Users Login(Users user)
        {
            var DBUser = Get(user.NIC); // Check user

            if (DBUser != null)
            {
                if (DBUser.Password == user.Password)// Validate password
                {
                    if (DBUser.Role != "traveller")
                    {
                        return DBUser; // Authentication successful, return the user object
                    }
                    else
                    {
                        if (DBUser.Status == "active"&& DBUser.IsApprove==true) //Check traveller status
                        {
                            return DBUser;
                        }
                        else
                        {
                            throw new CustomException("Traveller not allowed to login"); //Invalid traveller
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
            var DBUser = Get(user.NIC); // Check user

            if (DBUser == null)
            {

                _users.InsertOne(user); //Create new user
                return user;
            }
            else
            {
                throw new CustomException("User already exists");// User already exists exception (e.g., throw an exception)
            }
            return null;
        }

        public Users UpdateTraveller(string NIC, Users updatedUser) //update user
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

        public Users Update(string NIC, Users updatedGame)
        {
            updatedGame.NIC = NIC;
            _users.ReplaceOne(game => game.NIC == NIC, updatedGame);
            return updatedGame;
        }



        public void Delete(Users userForDeletion) => _users.DeleteOne(train => train.NIC == userForDeletion.NIC);

        public void Delete(string id) => _users.DeleteOne(train => train.NIC == id);
    }
}