using Microsoft.Extensions.Caching.Memory;
using TimeZoneConverter;
namespace FlightQualityAnalyzerAPI.Helpers
{
    public class TimeZoneProvider
    {        
        private const string MappingFilePath = @"Data\AirportCodesTimeZones.txt";
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "AirportTimeZones";
        private const int CacheExpirationDuration = 15;
        private Dictionary<string, string> _airportTimeZones;
        
        public TimeZoneProvider()
        {
            _airportTimeZones = GetAirportTimeZones();
        }

        public TimeZoneProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _airportTimeZones = GetAirportTimeZones();
        }

        /// <summary>
        /// Convert local date time to UTC and it is based on the Airport location.
        /// </summary>
        /// <param name="localDateTimeString">Local date time string</param>
        /// <param name="location">Location of the airport</param>
        /// <returns></returns>
        public DateTime ConvertLocalToUtc(string localDateTimeString, string location)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(localDateTimeString)) { return DateTime.UtcNow; }

                var timeZone = this.GetTimeZoneForAirport(location);

                //IF location based TimeZoneInfo not found in the map, return localDateTimeString as it is.
                if (timeZone == null)
                {
                    return DateTime.Parse(localDateTimeString).ToUniversalTime();
                }

                var localTime = DateTime.Parse(localDateTimeString);
                // Adjust the time forward by 1 hour to make it valid
                if (timeZone.IsInvalidTime(localTime))
                {
                    localTime = localTime.AddHours(1);
                }
                var utc = TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);
                return new DateTimeOffset(utc, TimeSpan.Zero).UtcDateTime;
            }
            catch (Exception ex)
            {
                //need to log to admin.
                //location based date time cannot be found so use localDateTimeString only.
                return DateTime.Parse(localDateTimeString).ToUniversalTime();
            }
        }
        /// <summary>
        /// GetTimeZone for specific Airport.
        /// </summary>
        /// <param name="airportCode"></param>
        /// <returns></returns>
        private TimeZoneInfo? GetTimeZoneForAirport(string airportCode)
        {
            if (!string.IsNullOrEmpty(airportCode))
            {
                if (_airportTimeZones.Any() && _airportTimeZones.TryGetValue(airportCode.ToUpper(), out var ianaId))
                {
                    // Convert IANA time zone identifier to Windows time zone identifier
                    string windowsId = TZConvert.IanaToWindows(ianaId);
                    return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                }
            }
            return null;
        }
        /// <summary>
        /// Method to get mapping of airport codes to IANA time zone identifiers
        /// </summary>
        private Dictionary<string, string> GetAirportTimeZones()
        {
            
            if (_memoryCache != null && _memoryCache.TryGetValue(CacheKey, out _airportTimeZones))
            {
                return _airportTimeZones;
            }

            _airportTimeZones = new Dictionary<string, string>();
            if (File.Exists(MappingFilePath))
            {
                var lines = File.ReadAllLines(MappingFilePath);
                
                foreach (var line in lines)
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        _airportTimeZones.Add(parts[0], parts[1]);
                    }
                }

                if(_memoryCache != null)
                {
                    // Store in cache with 15-minute expiration
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationDuration));

                    _memoryCache.Set(CacheKey, _airportTimeZones, cacheEntryOptions);
                }
            }
            
            return _airportTimeZones;
        }
        
    }
}
