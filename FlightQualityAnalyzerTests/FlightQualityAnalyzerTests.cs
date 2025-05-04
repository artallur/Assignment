using FlightQualityAnalyzerAPI.Models;
using FlightQualityAnalyzerAPI.Services;

namespace FlightQualityAnalyzerTests
{
    public class FlightAnalyzerTests
    {
        [Fact]
        public void CheckTimeLogic_ShouldReturnFlightsWithTimeInconsistencies()
        {
            // Arrange
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "XY123",
                    DepartureDateTime = "2024-10-1 12:0:0",
                    ArrivalDateTime = "2024-10-1 14:0:0"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "XY124",
                    DepartureDateTime = "2024-10-2 10:0:0",
                    ArrivalDateTime = "2024-10-2 10:0:0"
                }
            };
            var analyzer = new FlightAnalyzer(flights);

            // Act
            var result = analyzer.CheckTimeLogic();

            // Assert
            Assert.Single(result);
            Assert.Contains("ID 2", result[0]);
        }

        [Fact]
        public void CheckAircraftOverlap_ShouldReturnOverlappingFlights()
        {
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "XY100",
                    FlightRegistrationNumber = "REG1",
                    DepartureAirport = "HEL",
                    DepartureDateTime = "2024-10-1 8:00:00",
                    ArrivalAirport = "OUL",
                    ArrivalDateTime = "2024-10-1 9:30:00"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "XY101",
                    FlightRegistrationNumber = "REG1",
                    DepartureAirport = "OUL",
                    DepartureDateTime = "2024-10-1 10:00:00",
                    ArrivalAirport = "HEL",
                    ArrivalDateTime = "2024-10-1 11:30:00"
                }
            };

            var analyzer = new FlightAnalyzer(flights);
            var result = analyzer.CheckAircraftOverlap();

            Assert.Single(result);
            Assert.Contains("REG1", result[0]);
        }

        [Fact]
        public void CheckAircraftOverlap_ShouldReturnOverlappingFlightsWithDifferentTimeZones()
        {
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "XY100",
                    FlightRegistrationNumber = "REG1",
                    DepartureAirport = "",
                    DepartureDateTime = "2024-10-1 8:00:00",
                    ArrivalAirport = "OUL",
                    ArrivalDateTime = "2024-10-1 9:30:00"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "XY101",
                    FlightRegistrationNumber = "REG1",
                    DepartureAirport = "OUL",
                    DepartureDateTime = "2024-10-1 10:00:00",
                    ArrivalAirport = "HEL",
                    ArrivalDateTime = "2024-10-1 11:30:00"
                }
            };

            var analyzer = new FlightAnalyzer(flights);
            var result = analyzer.CheckAircraftOverlap();

            Assert.Single(result);
            Assert.Contains("REG1", result[0]);
        }

        [Fact]
        public void CheckRouteLogic_ShouldReturnFlightsWithSameDepartureAndArrival()
        {
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "AB123",
                    DepartureAirport = "JFK",
                    ArrivalAirport = "JFK"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "AB124",
                    DepartureAirport = "JFK",
                    ArrivalAirport = "LAX"
                }
            };

            var analyzer = new FlightAnalyzer(flights);
            var result = analyzer.CheckRouteLogic();

            Assert.Single(result);
            Assert.Contains("ID 1", result[0]);
        }

        [Fact]
        public void CheckAirportSequence_ShouldDetectSequenceMismatch()
        {
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "CD101",
                    FlightRegistrationNumber = "REG2",
                    DepartureAirport = "JFK",
                    ArrivalAirport = "LAX",
                    DepartureDateTime = "2024-10-1 6:0:0"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "CD102",
                    FlightRegistrationNumber = "REG2",
                    DepartureAirport = "ORD", // mismatch here it should be same as LAX.
                    ArrivalAirport = "ATL",
                    DepartureDateTime = "2024-10-1 12:0:0"
                }
            };

            var analyzer = new FlightAnalyzer(flights);
            var result = analyzer.CheckAirportSequence();

            Assert.Single(result);
            Assert.Contains("Mismatch", result[0]);
        }

        [Fact]
        public void CheckAirportSequence_ShouldDetectNoSequenceMismatch()
        {
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "CD101",
                    FlightRegistrationNumber = "REG2",
                    DepartureAirport = "JFK",
                    ArrivalAirport = "LAX",
                    DepartureDateTime = "2024-10-1 6:0:0"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "CD102",
                    FlightRegistrationNumber = "REG2",
                    DepartureAirport = "LAX",
                    ArrivalAirport = "ATL",
                    DepartureDateTime = "2024-10-1 12:0:0"
                }
            };

            var analyzer = new FlightAnalyzer(flights);
            var result = analyzer.CheckAirportSequence();

            Assert.True(result.Count == 0);
        }

        [Fact]
        public void CheckDuplicateFlights_ShouldReturnDuplicateFlightMessages()
        {
            var flights = new List<FlightData>
            {
                new FlightData
                {
                    Id = 1,
                    FlightNumber = "EF101",
                    FlightRegistrationNumber = "REG3",
                    DepartureDateTime = "2024-10-3 6:0:0",
                    ArrivalDateTime = "2024-10-3 8:0:0"
                },
                new FlightData
                {
                    Id = 2,
                    FlightNumber = "EF101",
                    FlightRegistrationNumber = "REG3",
                    DepartureDateTime = "2024-10-3 6:0:0",
                    ArrivalDateTime = "2024-10-3 8:0:0"
                }
            };

            var analyzer = new FlightAnalyzer(flights);
            var result = analyzer.CheckDuplicateFlights();

            Assert.Single(result);
            Assert.Contains("Duplicate", result[0]);
        }
    }
}
