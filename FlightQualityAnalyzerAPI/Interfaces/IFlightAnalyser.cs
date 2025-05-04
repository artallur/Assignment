namespace FlightQualityAnalyzerAPI.Interfaces
{
    public interface IFlightAnalyser
    {
        /// <summary>
        /// Checks the flight departure time is greater than arrival time. It checks the UTC time of flight information.
        /// </summary>
        /// <returns></returns>
        List<string> CheckTimeLogic();

        /// <summary>
        /// Checks the overlaps by taking <see cref="_minimumTurnAround"/> in to consideration between the current flight arrival time 
        /// and next flight departure time for the same flight.
        /// </summary>
        List<string> CheckAircraftOverlap();

        /// <summary>
        /// Checks whether departure airport and arrival airport are the same and flags the flight information as invalid route, if there is any violation.
        /// </summary>
        /// <returns>List of invalid routes</returns>
        List<string> CheckRouteLogic();

        /// <summary>
        /// Checks whether current flight arrival airport is same as next departure airport and flags the flight information as invalid if they are not same.
        /// </summary>
        List<string> CheckAirportSequence();

        /// <summary>
        /// Checks if the same flight information exists in another row of the csv file and flags as duplicate for the same flight number, flightregistrationnumber, departure and arrival time.
        /// </summary>
        List<string> CheckDuplicateFlights();
    }
}
