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

            return CreatedAtRoute("GetGame", new { id = train.Id.ToString() }, train);
        }

        [HttpPost]
        [Route("Reservation")]
        public ActionResult<Reservation> CreateReservation(Reservation reservation)
        {
            try
            {
                var trainreturn = _backendService.CreateReservation(reservation);

                return CreatedAtRoute("GetGame", new { id = reservation.NIC.ToString() }, reservation);
            }
            catch (Exceptions.InvalidTrainException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request status code along with the error message
            }
            catch (Exceptions.InvalidDateException ex)
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

        [HttpGet("{id}", Name = "GetGame")]
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
            catch (Exceptions.InvalidPasswordException ex)
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

            _userService.Update(id, gameIn);

            return NoContent();
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