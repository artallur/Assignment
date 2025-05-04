using FlightQualityAnalyzerAPI.Interfaces;
using FlightQualityAnalyzerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlightQualityAnalyzerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IFlightService _flightService;
        private readonly IFlightAnalyser _flightAnalyzer;

        public AnalysisController(IFlightService flightService, IFlightAnalyser flightAnalyzer)
        {
            _flightService = flightService;
            _flightAnalyzer = flightAnalyzer;
        }        
        /// <summary>
        /// Returns list of inconsistencies based on various checks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<FlightData>> GetDetailedFlightInconsistencies()
        {
            List<string> detailedList = new List<string>();

            List<string> duplicateFlightList =  _flightAnalyzer.CheckDuplicateFlights();
            if (duplicateFlightList.Count > 0) 
            {
                detailedList.AddRange(duplicateFlightList);
            }

            List<string> airCraftOverlapList = _flightAnalyzer.CheckAircraftOverlap();
            if (airCraftOverlapList.Count > 0) 
            {
                detailedList.AddRange(airCraftOverlapList);
            }

            List<string> airCraftSequenceList = _flightAnalyzer.CheckAirportSequence();
            if (airCraftSequenceList.Count > 0)
            {
                detailedList.AddRange(airCraftSequenceList);
            }

            List<string> routeList = _flightAnalyzer.CheckRouteLogic();
            if (routeList.Count > 0)
            {
                detailedList.AddRange(routeList);
            }

            List<string> timeList = _flightAnalyzer.CheckTimeLogic();
            if (timeList.Count > 0)
            {
                detailedList.AddRange(timeList);
            }

            return Ok(detailedList);
        }

        [HttpGet("checkairportsequence")]
        public ActionResult<List<FlightData>> GetFlightInconsistenciesWithAirportSequence()
        {
            List<string> detailedList = new List<string>();
            List<string> airCraftSequenceList = _flightAnalyzer.CheckAirportSequence();
            if (airCraftSequenceList.Count > 0)
            {
                detailedList.AddRange(airCraftSequenceList);
            }
            return Ok(detailedList);
        }

        [HttpGet("checkduplicateflights")]
        public ActionResult<List<FlightData>> GetFlightInconsistenciesWithDuplicateFlights()
        {
            List<string> detailedList = new List<string>();
            List<string> duplicateFlightList = _flightAnalyzer.CheckDuplicateFlights();
            if (duplicateFlightList.Count > 0)
            {
                detailedList.AddRange(duplicateFlightList);
            }
            return Ok(detailedList);
        }

        [HttpGet("checkaircraftoverlaps")]
        public ActionResult<List<FlightData>> GetFlightInconsistenciesWithAirCraftOverLaps()
        {
            List<string> detailedList = new List<string>();
            List<string> AirCraftOverlapList = _flightAnalyzer.CheckAircraftOverlap();
            if (AirCraftOverlapList.Count > 0)
            {
                detailedList.AddRange(AirCraftOverlapList);
            }
            return Ok(detailedList);
        }        

        [HttpGet("checkroutelogic")]
        public ActionResult<List<FlightData>> GetFlightInconsistenciesWithRouteLogic()
        {
            List<string> detailedList = new List<string>();
            List<string> routeList = _flightAnalyzer.CheckRouteLogic();
            if (routeList.Count > 0)
            {
                detailedList.AddRange(routeList);
            }
            return Ok(detailedList);
        }

        [HttpGet("checktimelogic")]
        public ActionResult<List<FlightData>> GetFlightInconsistenciesWithTimeLogic()
        {
            List<string> detailedList = new List<string>();
            List<string> timeList = _flightAnalyzer.CheckTimeLogic();
            if (timeList.Count > 0)
            {
                detailedList.AddRange(timeList);
            }
            return Ok(detailedList);
        }

    }
}
