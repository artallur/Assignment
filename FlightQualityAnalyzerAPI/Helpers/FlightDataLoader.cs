using FlightQualityAnalyzerAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FlightQualityAnalyzerAPI.Helpers
{
    public class FlightDataLoader
    {
        private readonly string _csvFilePath;
        private readonly List<FlightData> _flightDataList;  
        private readonly TimeZoneProvider _timeZoneProvider;
        
        /// <summary>
        /// Use this constructor to load the flight data directly from CSV file path.
        /// </summary>
        /// <param name="memoryCache">memory cache used to cache time zone and location map data only at the moment. It can be extended later.</param>
        /// <param name="flightsCSVFilePath">file path of the flights information csv file.</param>
        public FlightDataLoader(IMemoryCache memoryCache, string flightsCSVFilePath)
        {
            _timeZoneProvider = new TimeZoneProvider(memoryCache);
            _csvFilePath = flightsCSVFilePath;
        }

        /// <summary>
        /// Use this constructor to load information directly from list of <see cref="FlightData"></see> objects/>
        /// </summary>
        /// <param name="flightDataList">list of flight data objects</param>
        /// <remarks>At the moment this constructor is only used in Tests so no need for reading from memory cache.</remarks>
        public FlightDataLoader(List<FlightData> flightDataList)
        {
            _flightDataList = flightDataList;
            _csvFilePath = string.Empty;
            _timeZoneProvider = new TimeZoneProvider();
        }

        /// <summary>
        /// Load flights information from CSV file. If file path exists and file exists it reads from csv file otherwise it reads from list of file data objects passed through constructor. 
        /// </summary>
        /// <returns></returns>
        public List<FlightData> LoadFlights()
        {
            if (!string.IsNullOrEmpty(_csvFilePath)) 
            {
                var flights = new List<FlightData>();
                if (!File.Exists(_csvFilePath))
                    return flights;

                var lines = File.ReadAllLines(_csvFilePath).Skip(1); // Skip header

                foreach (var line in lines)
                {
                    var parts = line.Split(',');

                    if (parts.Length == 8)
                    {
                        flights.Add(new FlightData
                        {
                            Id = int.Parse(parts[0]),
                            FlightRegistrationNumber = parts[1],
                            FlightType = parts[2],
                            FlightNumber = parts[3],
                            DepartureAirport = parts[4],
                            DepartureDateTime = parts[5],
                            ArrivalAirport = parts[6],
                            ArrivalDateTime = parts[7],
                            DepartureUtc = _timeZoneProvider.ConvertLocalToUtc(parts[5], parts[4]),
                            ArrivalUtc = _timeZoneProvider.ConvertLocalToUtc(parts[7], parts[6])
                        });
                    }
                }
                return flights;
            }
            else
            {                
                foreach(FlightData flight in _flightDataList)
                {
                    flight.DepartureUtc = _timeZoneProvider.ConvertLocalToUtc(flight.DepartureDateTime, flight.DepartureAirport);
                    flight.ArrivalUtc = _timeZoneProvider.ConvertLocalToUtc(flight.ArrivalDateTime, flight.ArrivalAirport);
                }
                return _flightDataList;
            }
        }
    }
}
