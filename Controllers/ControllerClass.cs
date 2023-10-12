using mongodb_dotnet_example.Models;
using mongodb_dotnet_example.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace mongodb_dotnet_example.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class ControllerClass : ControllerBase
    {


        private readonly UserService _userService;
        private readonly BackendOfficerService _backendService;

        public ControllerClass(UserService userService, BackendOfficerService backendService)
        {
            _userService = userService;
            _backendService = backendService;
        }

        [HttpPost]
        [Route("Train")]
        public ActionResult<Train> Create(Train train)
        {
            var trainreturn = _backendService.Create(train);

            return CreatedAtRoute(new { id = train.Id }, train);
        }

        [HttpGet]
        [Route("Train")]
        public ActionResult<List<Train>> GetTrain()
        {
            return _backendService.GetTrain();
        }
        [HttpPut]
        [Route("Train")]
        public IActionResult UpdateTrain(string id, Train train)
        {

            var trainDb = _backendService.GetTrainById(id);

            if (trainDb == null)
            {
                return NotFound();
            }

            var User = _backendService.UpdateTrain(id, train);
            return Ok(User);
        }
        [HttpPut]
        [Route("CancelTrain")]
        public IActionResult CancelTrain(string id, Train train)
        {

            try
            {
                var game = _backendService.GetTrainById(id);

                if (game == null)
                {
                    return NotFound();
                }

                var User = _backendService.CancelTrain(id, train);
                return Ok(User);
            }
            catch (Exceptions.InvalidReservation ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }



        }
        [HttpPost]
        [Route("Reservation")]
        public ActionResult<Reservation> CreateReservation(Reservation reservation)
        {
            try
            {
                var trainreturn = _backendService.CreateReservation(reservation);

                return CreatedAtRoute(new { id = reservation.NIC.ToString() }, trainreturn);
            }
            catch (Exceptions.InvalidTrainException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exceptions.InvalidDateException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exceptions.InvalidReservation ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }


        }
        [HttpGet]
        [Route("History/{id}")]
        public ActionResult<List<History>> GetReservationHistory(String id)
        {
            return _backendService.GetReservationHistoryByID(id);
        }
        [HttpGet]
        [Route("Reservation")]
        public ActionResult<List<Reservation>> GetReservation()
        {
            return _backendService.GetReservation();
        }

        [HttpGet("Reservation/{id}")]
        public ActionResult<Reservation> GetReservation(String id)
        {
            return _backendService.GetReservationByID(id);
        }


        [HttpPut]
        [Route("CancelReservation")]
        public IActionResult CalcelReservation(string id, Reservation reservation)
        {
            try
            {
                var reservationDb = _backendService.GetReservationByID(reservation.NIC);

                if (reservationDb == null)
                {
                    return NotFound();
                }
                var User = _backendService.CancelReservation(id, reservationDb);
                return Ok(User);
            }

            catch (Exceptions.InvalidTrainException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }

        }

        [HttpGet]
        public ActionResult<List<Users>> Get()
        {
            return _userService.Get();
        }

        [HttpGet]
        [Route("traveller")]
        public ActionResult<List<Users>> GetTravellers()
        {
            return _userService.GetTravellers();
        }

        [HttpGet]
        [Route("activetraveller")]
        public ActionResult<List<Users>> GetActiveTravellers()
        {
            return _userService.GetActiveTravellers();
        }

        [HttpGet]
        [Route("travellerprofile")]
        public ActionResult<List<Users>> GetTravellerProfile()
        {
            return _userService.GetTravellerProfile();
        }

        [HttpGet("{id}")]
        public ActionResult<Users> Get(string id)
        {
            var users = _userService.Get(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        [HttpPost]
        public ActionResult<Users> Create(Users users)
        {
            _userService.Create(users);

            return CreatedAtRoute("GetGame", new { id = users.NIC.ToString() }, users);
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<Users> Login(Users users)
        {
            try
            {
                var result = _userService.Login(users);
                return Ok(result);
            }
            catch (Exceptions.CustomException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exceptions.InvalidUserException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }

        [HttpPut]
        public IActionResult Update(string id, Users gameIn)
        {
            var game = _userService.Get(id);

            if (game == null)
            {
                return NotFound();
            }
            var User = _userService.Update(id, gameIn);
            return Ok(User);

        }

        [HttpDelete()]
        public IActionResult Delete(string id)
        {
            var game = _userService.Get(id);

            if (game == null)
            {
                return NotFound();
            }

            _userService.Delete(game.NIC);

            return NoContent();
        }
    }
}