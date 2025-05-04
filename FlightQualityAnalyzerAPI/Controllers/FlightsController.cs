using FlightQualityAnalyzerAPI.Interfaces;
using FlightQualityAnalyzerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightQualityAnalyzerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        /// <summary>
        /// Gets all the flights information from the CSV file in to JSON format.
        /// </summary>
        [HttpGet]
        public ActionResult<List<FlightData>> GetAllFlights()
        {
            var flights = _flightService.GetAllFlights();
            return Ok(flights);
        }
    }
}
