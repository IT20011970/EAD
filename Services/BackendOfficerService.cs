using mongodb_dotnet_example.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System;
using mongodb_dotnet_example.Exceptions;
using MongoDB.Bson;
using DnsClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Numerics;

namespace mongodb_dotnet_example.Services
{
    public class BackendOfficerService
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Train> _train;
        private readonly IMongoCollection<Reservation> _reservation;
        private readonly IMongoCollection<History> _history;
        public BackendOfficerService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<Users>(settings.UserCollectionName);
            _train = database.GetCollection<Train>(settings.TrainCollectionName);
            _reservation = database.GetCollection<Reservation>(settings.ReservationCollectionName);
            _history = database.GetCollection<History>(settings.HistoryCollectionName);
        }
        public List<Users> Get() => _users.Find(game => true).ToList();

        public List<Reservation> GetReservation() => _reservation.Find(game => true).ToList();

        public List<History> GetReservationHistoryByID(string id) => _history.Find(userObj => userObj.NIC == id).ToList();

        public Reservation GetReservationByID(string id) => _reservation.Find(game => game.NIC == id).FirstOrDefault();
        public List<Train> GetTrain() => _train.Find(game => true).ToList();
        public List<Users> GetTravellers()
        {
            var filter = Builders<Users>.Filter.And(
                Builders<Users>.Filter.Eq(user => user.Status, "inactive"),
                Builders<Users>.Filter.Eq(user => user.IsApprove, false)
            );

            var users = _users.Find(filter).ToList();
            return users;
        }
  
        public Train GetTrainById(string id) => _train.Find(game => game.Id == id).FirstOrDefault();

        public Train UpdateTrain(string NIC, Train updatedGame)
        {

            var filter = Builders<Train>.Filter.Eq(u => u.Id, NIC);

            var update = Builders<Train>.Update
                .Set(u => u.Arrival_Time, updatedGame.Arrival_Time)
                .Set(u => u.Depatre_Time, updatedGame.Depatre_Time)
             .Set(u => u.Status, updatedGame.Status);
            _train.UpdateOne(filter, update);

           // updatedGame.Id = NIC;
           // _train.ReplaceOne(game => game.Id == NIC, updatedGame);
            return updatedGame;
        }
        public Train CancelTrain(string NIC, Train updatedGame)
        {
            var reservation = GetReservation();
            bool isthere = false;
           
            foreach (Reservation res in reservation)
            {
                foreach (Train train in res.trains)
                {
                    if (train.Id == NIC)
                    {
                        isthere = true;
                        break;
                    }
                }

                if (isthere)
                {
                    // If there is a train with the specified Id (NIC), no need to continue checking other reservations
                    break;
                }
            }

            if (!isthere)
            {
                var filter = Builders<Train>.Filter.Eq(u => u.Id, NIC);

                var update = Builders<Train>.Update
                    .Set(u => u.Status, updatedGame.Status);

                _train.UpdateOne(filter, update);
                return null;
                // updatedGame.Id = NIC;
                // _train.ReplaceOne(game => game.Id == NIC, updatedGame);
            }
            else
            {
                throw new InvalidReservation("Please remove reservations");
            }
           
            
            return null;
              
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
                    throw new CustomException("Invalid password");// Password mismatch, handle the error (e.g., throw an exception)
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
        public Reservation CancelReservation(string id,Reservation reservation)
        {
            if (reservation.trains.Count > 0)
            {
                var trainToRemove = reservation.trains.FirstOrDefault(t => t.Id == id);

                if (trainToRemove != null)
                {         
                    reservation.trains.Remove(trainToRemove);
                    var filter = Builders<Reservation>.Filter.Eq(r => r.NIC, reservation.NIC);
                    _reservation.ReplaceOne(filter, reservation);
                    return reservation;
                }
                else
                {
                    throw new InvalidTrainException("Invalid Train");
                }
            }

            return null;
        }
        public Reservation CreateReservation(Reservation reservation)
        {
            var DBReservation = _reservation.Find(game => game.NIC == reservation.NIC).FirstOrDefault();

            if (DBReservation == null)
            {
                foreach (Train tr in reservation.trains)
                {
                    var train = _train.Find(t => t.Id == tr.Id).FirstOrDefault();
                    if (train != null&& train.Status=="active")
                    {
                        tr.Number = train.Number;
                        tr.Depatre_Station = train.Depatre_Station;
                        tr.Arrival_Station = train.Arrival_Station;
                        tr.Depatre_Time = train.Depatre_Time;
                        tr.Arrival_Time = train.Arrival_Time;
                        tr.Status = train.Status;
                        DateTime trainArrivalTime = train.Arrival_Time;
                        double differenceInDays = (trainArrivalTime - reservation.TodayDate).TotalDays;

                        if (differenceInDays > 30)
                        {
                            History ht = new History();
                            ht.NIC = reservation.NIC;
                            ht.Departre_Station = train.Depatre_Station;
                            ht.Arrival_Station = train.Arrival_Station;
                            ht.Departre_Time = train.Depatre_Time;
                            ht.Arrival_Time = train.Arrival_Time;
                            ht.Reserverved_Time = reservation.TodayDate;
                            ht.TrainId = train.Id;
                            ht.Number = train.Number;
                            _history.InsertOne(ht);
                            _reservation.InsertOne(reservation);
                            return reservation;
                        }
                        else
                        {
                            // Handle the case where the difference is not greater than 30 days
                            throw new InvalidDateException("Reservation must be made at least 30 days before train arrival.");
                            break;
                        }

                    }
                    else
                    {
                        throw new InvalidTrainException("Invalid Train");
                        break;
                    }
                }
            }
            else
            {
                if (DBReservation.trains.Count < 4)
                {
                    foreach (Train tr in reservation.trains)
                    {
                        var train = _train.Find(t => t.Id == tr.Id).FirstOrDefault();
                        if (train != null && train.Status == "active")
                        {
                            tr.Number = train.Number;
                            tr.Depatre_Station = train.Depatre_Station;
                            tr.Arrival_Station = train.Arrival_Station;
                            tr.Depatre_Time = train.Depatre_Time;
                            tr.Arrival_Time = train.Arrival_Time;
                            tr.Status = train.Status;
                            DateTime trainArrivalTime = train.Arrival_Time;
                            double differenceInDays = (trainArrivalTime - reservation.TodayDate).TotalDays;

                            bool trainExistsInDB = DBReservation.trains.Any(t => t.Id == tr.Id);
                            if (!trainExistsInDB)
                            {

                                if (differenceInDays > 30)
                                {
                                    reservation.trains.Add(tr);
                                    var filter = Builders<Reservation>.Filter.Eq(r => r.NIC, DBReservation.NIC);
                                    var update = Builders<Reservation>.Update.Push(r => r.trains, tr);
                                    _reservation.UpdateOne(filter, update);
                                    Reservation updatedReservation = _reservation.Find(t => t.NIC == reservation.NIC).FirstOrDefault();

                                    History ht = new History();
                                    ht.NIC = reservation.NIC;
                                    ht.Departre_Station = tr.Depatre_Station;
                                    ht.Arrival_Station = tr.Arrival_Station;
                                    ht.Departre_Time = tr.Depatre_Time;
                                    ht.Arrival_Time = tr.Arrival_Time;
                                    ht.Reserverved_Time = reservation.TodayDate;
                                    ht.TrainId = tr.Id;
                                    ht.Number = tr.Number;
                                    _history.InsertOne(ht);
                                   

                                    return updatedReservation;
                                }
                                else
                                {
                                    // Handle the case where the difference is not greater than 30 days
                                    throw new InvalidDateException("Reservation must be made at least 30 days before train arrival.");
                                    break;
                                }
                            }
                            else
                            {
                                if (differenceInDays > 4)
                                {

                                    foreach (var t in DBReservation.trains)
                                    {
                                        t.Arrival_Time = tr.Arrival_Time;
                                        t.Depatre_Time = tr.Depatre_Time;
                                    }

                                    // Construct filter and update to target the specific reservation and update the trains
                                    var filter = Builders<Reservation>.Filter.Eq(r => r.NIC, DBReservation.NIC);
                                    var update = Builders<Reservation>.Update.Set(r => r.trains, DBReservation.trains);

                                    // Update the reservation in the database
                                    _reservation.UpdateOne(filter, update);

                                    var filter2 = Builders<History>.Filter.Eq(h => h.NIC, reservation.NIC); // Assuming NIC is the unique identifier for your documents
                                    var update2 = Builders<History>.Update
                                        .Set(h => h.Departre_Station, tr.Depatre_Station)
                                        .Set(h => h.Arrival_Station, tr.Arrival_Station)
                                        .Set(h => h.Departre_Time, tr.Depatre_Time)
                                        .Set(h => h.Arrival_Time, tr.Arrival_Time);                                   
                                       

                                    var result = _history.UpdateOne(filter2, update2);

                                    // Fetch and return the updated reservation
                                    Reservation updatedReservation = _reservation.Find(r => r.NIC == reservation.NIC).FirstOrDefault();
                                    return updatedReservation;
                                }
                                else
                                {
                                    // Handle the case where the difference is not greater than 30 days
                                    throw new InvalidDateException("Reservation must be made at least 30 days before train arrival.");
                                    break;
                                }
                            }

                        }
                        else
                        {
                            throw new InvalidTrainException("Invalid Train");
                            break;
                        }

                    }
                }
                else
                {
                    throw new InvalidReservation("Reservation count increased.");
                }
            }

            return null;
        }




        public void Update(string NIC, Users updatedGame) => _users.ReplaceOne(game => game.NIC == NIC, updatedGame);

        public void Delete(Users gameForDeletion) => _users.DeleteOne(game => game.NIC == gameForDeletion.NIC);

        public void Delete(string id) => _users.DeleteOne(game => game.NIC == id);
    }
}