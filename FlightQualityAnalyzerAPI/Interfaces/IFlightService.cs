using FlightQualityAnalyzerAPI.Models;

namespace FlightQualityAnalyzerAPI.Interfaces
{
    public interface IFlightService
    {
        List<FlightData> GetAllFlights();
    }
}
