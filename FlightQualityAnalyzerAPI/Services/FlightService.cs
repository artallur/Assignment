using FlightQualityAnalyzerAPI.Helpers;
using FlightQualityAnalyzerAPI.Interfaces;
using FlightQualityAnalyzerAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightQualityAnalyzerAPI.Services
{
    public class FlightService : IFlightService
    {
        private List<FlightData> _flightDataList;
        /// <summary>
        /// Constructor that directly sets list of FlightData objects from parameter.
        /// </summary>
        public FlightService(List<FlightData> flightDataList) => _flightDataList = new FlightDataLoader(flightDataList).LoadFlights();

        /// <summary>
        /// Constructor that uses <see cref="FlightDataLoader"></see> and loads flight information from CSV file.
        /// </summary>
        /// <param name="memoryCache">Used to set and load TimeZoneInfo based on airport arrival or destination airport timezone. 
        /// Used to calculate UTC time for arrival and destination date time values</param>
        /// <param name="flightDatasCSVFilePath">CSV file path of flights.csv file.</param>
        public FlightService(IMemoryCache memoryCache, string flightDatasCSVFilePath)
        {
            _flightDataList = new FlightDataLoader(memoryCache, flightDatasCSVFilePath).LoadFlights();
        }

        public List<FlightData> GetAllFlights() => _flightDataList;

        public List<FlightData> GetInconsistentFlightChains()
        {
            var inconsistencies = new List<FlightData>();
            var groupedByAircraft = _flightDataList
                .GroupBy(f => f.FlightRegistrationNumber);

            foreach (var group in groupedByAircraft)
            {
                var ordered = group.OrderBy(f => f.DepartureDateTime).ToList();

                for (int i = 1; i < ordered.Count; i++)
                {
                    var previousArrival = ordered[i - 1].ArrivalAirport;
                    var currentDeparture = ordered[i].DepartureAirport;

                    if (previousArrival != currentDeparture)
                    {
                        inconsistencies.Add(ordered[i]);
                    }
                }
            }

            return inconsistencies;
        }
    }
}
