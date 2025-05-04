
using FlightQualityAnalyzerAPI.Helpers;
using System.Globalization;
using System.Text.Json.Serialization;

namespace FlightQualityAnalyzerAPI.Models
{
    public class FlightData
    {        

        [JsonPropertyName("id")]       
        public int Id { get; set; }

        [JsonPropertyName("aircraft_registration_number")]
        public string FlightRegistrationNumber { get; set; }

        [JsonPropertyName("aircraft_type")]
        public string FlightType { get; set; }

        [JsonPropertyName("flight_number")]
        public string FlightNumber { get; set; }

        [JsonPropertyName("departure_airport")]
        public string DepartureAirport { get; set; }

        [JsonPropertyName("departure_datetime")]
        public string DepartureDateTime { get; set; }

        [JsonPropertyName("arrival_airport")]
        public string ArrivalAirport { get; set; }

        [JsonPropertyName("arrival_datetime")]
        public string ArrivalDateTime { get; set; }

        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; }
    }

}
