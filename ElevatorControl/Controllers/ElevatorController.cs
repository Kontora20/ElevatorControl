using ElevatorControl.Models;
using ElevatorControl.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ElevatorControl.Controllers
{
    [ApiController]
    public class ElevatorController : ControllerBase
    {
        private readonly IElevatorService _elevatorService;

        public ElevatorController(IElevatorService elevatorService)
        {
            _elevatorService = elevatorService;
        }

        [Route("elevator/{id}/floor/{floor}")]
        [HttpPatch]
        public IActionResult MoveElevator(int id, int floor)
        {
            try
            {
                _elevatorService.MoveElevator(id, floor);
                return Ok();
            }
            catch (ArgumentException)
            {
                return BadRequest("Provided elevator id or floor is incorrect");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something bad happened: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("elevator/{id}")]
        [HttpGet]
        public ActionResult<Elevator> GetElevator(int id)
        {
            try
            {
                return Ok(_elevatorService.GetElevatorInfo(id));
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Incorrect elevator ID selected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something bad happened: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Route("elevator/{id}/logs")]
        [HttpGet]
        public ActionResult<List<ElevatorLogEntry>> GetElevatorLogs(int id, int? beforeTimeMinutes = 5)
        {
            try
            {
                return Ok(_elevatorService.GetElevatorLogs(id, beforeTimeMinutes.Value));
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Incorrect elevator ID selected");
            }
        }
    }
}
