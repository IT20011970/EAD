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


        private readonly UserService _userService; //User service
        private readonly BackendOfficerService _backendService; //Backend service

        public ControllerClass(UserService userService, BackendOfficerService backendService)
        {
            _userService = userService;
            _backendService = backendService;
        }

        [HttpPost] // Specifies that this method handles HTTP  requests
        [Route("Train")] // Specifies the endpoint route for this method
        public ActionResult<Train> Create(Train train)
        {
            try
            {
                var trainreturn = _backendService.Create(train); //pass object

                return CreatedAtRoute(new { id = train.Id }, train);
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }

        }

        [HttpGet] // Specifies that this method handles HTTP  requests
        [Route("Train")]// Specifies the endpoint route for this method
        public ActionResult<List<Train>> GetTrain()
        {
            try
            {
                return _backendService.GetTrain();
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }
        [HttpPut] // Specifies that this method handles HTTP  requests
        [Route("Train")]// Specifies the endpoint route for this method
        public IActionResult UpdateTrain(string id, Train train)
        {
            try
            {
                var trainDb = _backendService.GetTrainById(id);  //pass id

                if (trainDb == null)
                {
                return NotFound();
                }

            var User = _backendService.UpdateTrain(id, train); 
            return Ok(User);
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }
        [HttpPut] // Specifies that this method handles HTTP  requests
        [Route("CancelTrain")]// Specifies the endpoint route for this method
        public IActionResult CancelTrain(string id, Train train)
        {

            try
            {
                var trainDB = _backendService.GetTrainById(id);  //pass id

                if (trainDB == null)
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

        [HttpGet] // Specifies that this method handles HTTP GET requests
        [Route("History")] // Specifies the endpoint route for this method
        public ActionResult<List<History>> GetReservationHistory()
        {
            // Call the GetReservationHistory method from the _backendService to retrieve reservation history
            // and return the result as an ActionResult containing a list of History objects.
            return _backendService.GetReservationHistory();
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
            try
            {
                return _backendService.GetReservationByID(id); //pass id
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
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

        [HttpGet] // Specifies that this method handles HTTP GET requests
        [Route("traveller")]// Specifies the endpoint route for this method
        public ActionResult<List<Users>> GetTravellers()
        {
            try
            {
                return _userService.GetTravellers();//Call get function
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }

        [HttpGet]// Specifies that this method handles HTTP GET requests
        [Route("activetraveller")] //Specifies the endpoint route for this method
        public ActionResult<List<Users>> GetActiveTravellers()
        {
            try
            {
                return _userService.GetActiveTravellers(); //Call get function
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }

        [HttpGet]// Specifies that this method handles HTTP GET requests
        [Route("travellerprofile")] //Specifies the endpoint route for this method
        public ActionResult<List<Users>> GetTravellerProfile()
        {
            try
            {
                return _userService.GetTravellerProfile();
            }
            catch (Exception ex)
            {
                // Handle other exceptions here if needed
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
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

        [HttpPost] // Specifies that this method handles HTTP POST requests
        public ActionResult<Users> Create(Users users) // Specifies the endpoint route for this method
        {
            try
            {
                _userService.Create(users); //Pass object to user service create method

                return CreatedAtRoute(new { id = users.NIC.ToString() }, users); //return the result 
            }
            catch (Exceptions.CustomException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }

        [HttpPost]// Specifies that this method handles HTTP POST requests
        [Route("login")]// Specifies the endpoint route for this method
        public ActionResult<Users> Login(Users users)
        {
            try
            {
                var result = _userService.Login(users); //Pass object to login service
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
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }
        }

        [HttpPut]// Specifies that this method handles HTTP PUT requests
        public IActionResult Update(string id, Users users)
        {
            try
            {
                var user = _userService.Get(id); //Check user

            if (user == null)
            {
                return NotFound();  // Return a user not found
            }
            var User = _userService.Update(id, users);
            return Ok(User);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error"); // Return a 500 Internal Server Error status code
            }

        }

        [HttpDelete()]
        public IActionResult Delete(string id)
        {
            var user = _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Delete(user.NIC);

            return NoContent();
        }
    }
}