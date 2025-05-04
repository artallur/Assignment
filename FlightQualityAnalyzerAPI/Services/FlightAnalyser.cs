using FlightQualityAnalyzerAPI.Helpers;
using FlightQualityAnalyzerAPI.Interfaces;
using FlightQualityAnalyzerAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightQualityAnalyzerAPI.Services
{
    public class FlightAnalyzer : IFlightAnalyser
    {
        private readonly List<FlightData> _flightDataList;
        private readonly TimeSpan _minimumTurnAround = TimeSpan.FromMinutes(45); //used to check minimum turn around time between the arrival and departure time of same flight.
        
        /// <summary>
        /// Constructor that directly sets list of FlightData objects from parameter.
        /// </summary>
        public FlightAnalyzer(List<FlightData> flightDatas) => _flightDataList = new FlightDataLoader(flightDatas).LoadFlights();

        /// <summary>
        /// Constructor uses <see cref="FlightDataLoader"></see> and loads flight information from CSV file.
        /// </summary>
        /// <param name="memoryCache">Used to set and load TimeZoneInfo based on airport arrival or destination airport timezone. 
        /// Used to calculate UTC time for arrival and destination date time values</param>
        /// <param name="flightDatasCSVFilePath">CSV file path of flights.csv file.</param>
        public FlightAnalyzer(IMemoryCache memoryCache, string flightDatasCSVFilePath)
            => _flightDataList = new FlightDataLoader(memoryCache, flightDatasCSVFilePath).LoadFlights();
        /// <summary>
        /// Checks the flight departure time is greater than arrival time. It checks the UTC time of flight information.
        /// </summary>
        /// <returns></returns>
        public List<string> CheckTimeLogic()
        {
            return _flightDataList
                .Where(f => f.DepartureUtc >= f.ArrivalUtc)
                .Select(f => $"Time inconsistency: ID {f.Id} Flight {f.FlightNumber} " +
                $"departs:{f.DepartureAirport} {f.DepartureDateTime}({f.DepartureUtc} UTC) -> " +
                $"arrives:{f.ArrivalAirport} {f.ArrivalDateTime}({f.ArrivalUtc} UTC)")
                .ToList();
        }

        /// <summary>
        /// Checks the overlaps by taking <see cref="_minimumTurnAround"/> in to consideration between the current flight arrival time 
        /// and next flight departure time for the same flight.
        /// </summary>
        public List<string> CheckAircraftOverlap()
        {
            List<string> flightsWithOverlaps = new List<string>();
            var grouped = _flightDataList.GroupBy(f => f.FlightRegistrationNumber);
            foreach (var group in grouped)
            {
                var ordered = group.OrderBy(f => f.DepartureUtc).ToList();
                for (int i = 0; i < ordered.Count - 1; i++)
                {
                    var current = ordered[i];
                    var next = ordered[i + 1];
                    if (current.ArrivalUtc + _minimumTurnAround > next.DepartureUtc)
                    {
                        flightsWithOverlaps.Add($"Inconsistency with little Turn Around time Sequence Detected: " +
                            $"flight registration {current.FlightRegistrationNumber} " +
                            $"with id: {current.Id} Arrives at {current.ArrivalAirport} and {current.ArrivalUtc} UTC is " +
                            $"not sufficient for flight with id: {next.Id} Departures to {next.ArrivalAirport} and {next.DepartureUtc} UTC");
                        break;
                    }
                }
            }

            return flightsWithOverlaps;
        }        
        
        /// <summary>
        /// Checks whether departure airport and arrival airport are the same and flags the flight information as invalid route, if there is any violation.
        /// </summary>
        /// <returns>List of invalid routes</returns>
        public List<string> CheckRouteLogic()
        {
            return _flightDataList
                .Where(f => string.Equals(f.DepartureAirport, f.ArrivalAirport, StringComparison.OrdinalIgnoreCase))
                .Select(f => $"Invalid route:  ID {f.Id} Flight {f.FlightNumber} departs at {f.ArrivalAirport} and arrives at {f.DepartureAirport} means same airport")
                .ToList();
        }

        /// <summary>
        /// Checks whether current flight arrival airport is same as next departure airport and flags the flight information as invalid if they are not same.
        /// </summary>
        public List<string> CheckAirportSequence()
        {
            var issues = new List<string>();
            var grouped = _flightDataList.GroupBy(f => f.FlightRegistrationNumber);            

            foreach (var group in grouped)
            {
                var ordered = group.OrderBy(f => f.DepartureUtc).ToList();
                for (int i = 0; i < ordered.Count - 1; i++)
                {
                    var current = ordered[i];
                    var next = ordered[i + 1];

                    if (!string.Equals(current.ArrivalAirport, next.DepartureAirport, StringComparison.OrdinalIgnoreCase))
                    {
                        issues.Add($"Mismatch Airport Sequence Detected: " +
                            $"flight registration {current.FlightRegistrationNumber} " +
                            $"with id: {current.Id} Arrives at {current.ArrivalAirport} but same flight with id: {next.Id} Departures to {next.DepartureAirport}");

                        break;
                    }
                }
            }

            return issues;
        }

        /// <summary>
        /// Checks if the same flight information exists in another row of the csv file and flags as duplicate for the same flight number, flightregistrationnumber, departure and arrival time.
        /// </summary>
        public List<string> CheckDuplicateFlights()
        {
            return _flightDataList
                .GroupBy(f => new
                {
                    f.FlightNumber,
                    Departure = f.DepartureUtc,
                    Arrival = f.ArrivalUtc,
                    f.FlightRegistrationNumber
                })
                .Where(g => g.Count() > 1)
                .Select(g => $"Duplicate flight: flight number {g.Key.FlightNumber} at {g.Key.Departure} UTC by {g.Key.FlightRegistrationNumber}")
                .ToList();
        }                     
    }

}
