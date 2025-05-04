This assignment provides RestAPI that reads flight data from a flights.csv file located on the server under "\FlightQualityAnalyzerAPI\Data" directory and converts the CSV data into a structured JSON format. The solution also analyzes the data and 
makes different checks, provides the result as a list of inconsistency strings.

This Assignment contains two directories. One directory contains ASP.NET Core WebAPI project "FlightQualityAnalyzerAPI" and another directory contains xUnit C# project "FlightQualityAnalyzerTests".
The solution file is included in FlightQualityAnalyzerAPI which has reference to FlightQualityAnalyzerAPI and FlightQualityAnalyzerTests c# projects.

Rest API exposes the below endpoints for testing.

/api/flights   -> Reads flight data and converts the CSV data to JSON format. 

/api/analysis  -> Reads flight data and does the analysis part. Checks all five rules like airport sequence, duplicate flights, aircraft overlaps, route logic and time logic checks.
/api/analysis/checkairportsequence  -> Checks whether current flight arrival airport is same as next departure airport and flags the flight information as invalid if they are not same.
/api/analysis/checkduplicateflights -> Checks if the same flight information exists and flags as duplicate for the same flight number, flightregistrationnumber, departure and arrival time.
/api/analysis/checkaircraftoverlaps -> checks the overlaps by taking minimum turn around time (currently hard coded value) in to consideration between the current flight arrival time and next flight departure time for the same flight.
/api/analysis/checkroutelogic -> Checks whether departure airport and arrival airport are the same and flags the flight information as invalid route, if there is any violation.
/api/analysis/checktimelogic -> checks the flight departure time is greater than arrival time. It checks the UTC time of flight information.

(Check FlightQualityAnalyzerAPI.http for more reference and testing.)

While analyzing the data I have used UTC values for arrival and departure datetime information based on the airport location. For that I have used TimeZoneProvider library (https://github.com/mattjohnsonpint/TimeZoneConverter) and a file that 
has information about airport codes and IANA time zone identifier. To be more precise in checking the aircraft overlaps.

If you have any questions feel free to contact me. Thank you for reviewing.
