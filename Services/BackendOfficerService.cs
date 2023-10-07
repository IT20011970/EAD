using mongodb_dotnet_example.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using mongodb_dotnet_example.Exceptions;
using MongoDB.Bson;

namespace mongodb_dotnet_example.Services
{
    public class BackendOfficerService
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Train> _train;
        private readonly IMongoCollection<Reservation> _reservation;
        public BackendOfficerService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<Users>(settings.GamesCollectionName);
            _train = database.GetCollection<Train>(settings.TrainCollectionName);
            _reservation = database.GetCollection<Reservation>(settings.ReservationCollectionName);
        }
        public List<Users> Get() => _users.Find(game => true).ToList();
        public List<Users> GetTravellers()
        {
            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(user => user.Status, "inactive"),
                Builders<Users>.Filter.Eq(user => user.IsApprove, false)
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

        public Train Create(Train train)
        {
            _train.InsertOne(train);
            return train;
        }

        public Reservation CreateReservation(Reservation reservation)
        {
            foreach (Train tr in reservation.trains)
            {
                var train = _train.Find(t => t.Id == tr.Id).FirstOrDefault();
                if (train != null)
                {
                    DateTime trainArrivalTime = train.Arrival_Time;
                    double differenceInDays = (trainArrivalTime - reservation.TodayDate).TotalDays;

                    if (differenceInDays > 30)
                    {
                        _reservation.InsertOne(reservation);
                        return reservation;
                    }
                    else
                    {
                        // Handle the case where the difference is not greater than 30 days
                        throw new InvalidDateException("Reservation must be made at least 30 days before train arrival.");
                    }

                }
                else
                {
                    throw new InvalidTrainException("Invalid Train");
                    break;
                }
            }
            return null;
        }




        public void Update(string NIC, Users updatedGame) => _users.ReplaceOne(game => game.NIC == NIC, updatedGame);

        public void Delete(Users gameForDeletion) => _users.DeleteOne(game => game.NIC == gameForDeletion.NIC);

        public void Delete(string id) => _users.DeleteOne(game => game.NIC == id);
    }
}