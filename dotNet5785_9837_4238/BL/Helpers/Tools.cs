using BO;
using DalApi;
using System.Net;
using System.Text;

namespace Helpers;
/// <summary>
/// class for the help methods
/// </summary>

public static class Tools
{
    /// <summary>
    /// explanation to the function : for all elements in T, we take its value, and add it with the name of the property
    /// and apply the basic toString function on our element wich is type stringbuilder
    /// </summary>
    public static string ToStringProperty<T>(this T t)
    {
        StringBuilder sb = new StringBuilder();

        // for all elements in T
        foreach (var property in typeof(T).GetProperties())
        {
            // take its value
            object value = property.GetValue(t, null);

            // add the name of the property and its value together
            sb.AppendLine($"{property.Name}: {value}");
        }
        return sb.ToString();
    }

    /// <summary>
    /// function to check if a value is int
    /// </summary>
    public static bool CheckInt(object value)
    {
        return value is int;
    }


    /// <summary>
    /// function to check if a value is double
    /// </summary>
    public static bool CheckDouble(object value)
    {
        return value is double;
    }




    /// <summary>
    /// function that finds the geographical coordinates (latitude and longitude) for a given address
    /// it utses the LocationIQ API. The function validates the input address, 
    /// sends a GET request to the API, processes the JSON response, and returns the coordinates 
    /// as a tuple (latitude, longitude). If no results are found or if the request fails, 
    /// it throws an exception.
    /// /// </summary>
    public static (double Latitude, double Longitude) GetAddressCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be null or empty.", nameof(address));
        }

        const string LocationIqApiKey = "pk.ddce0bbd11edfee17d07cb35922321f7";
        const string BaseUrl = "https://us1.locationiq.com/v1/search.php";
        //in the request we send a key and the address to find
        string requestUrl = $"{BaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";

        using (var client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(requestUrl).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error fetching geolocation data: {response.ReasonPhrase}");
            }

            string responseContent = response.Content.ReadAsStringAsync().Result;

            var locations = System.Text.Json.JsonSerializer.Deserialize<List<LocationIqResponse>>(responseContent);

            if (locations == null || locations.Count == 0)
            {
                throw new Exception("No geolocation data found for the provided address.");
            }

            var firstResult = locations.First();
            return (Latitude: double.Parse(firstResult.Lat), Longitude: double.Parse(firstResult.Lon));
        }
    }

    /// <summary>
    /// class to get the latitude and longitude of a LocationIqResponse
    /// </summary>
    private class LocationIqResponse
    {
        public string Lat { get; set; }
        public string Lon { get; set; }
    }

    /// <summary>
    /// function that checks if the coordinates of a volunteer match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static bool CheckAddressVolunteer(Volunteer vol)
    {
        if (vol.Latitude == null || vol.Longitude == null)
        {
            throw new Exception("Latitude or Longitude is null.");
        }

        var (expectedLatitude, expectedLongitude) = Tools.GetAddressCoordinates(vol.Adress);

        const double tolerance = 0.0001;
        bool isLatitudeMatch = Math.Abs(vol.Latitude.Value - expectedLatitude) < tolerance;
        bool isLongitudeMatch = Math.Abs(vol.Longitude.Value - expectedLongitude) < tolerance;

        return isLatitudeMatch && isLongitudeMatch;
    }

    /// <summary>
    /// function that checks if the coordinates of a call match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static bool CheckAddressCall(Call c)
    {
        var (expectedLatitude, expectedLongitude) = Tools.GetAddressCoordinates(c.Address);
        const double tolerance = 0.0001;

        bool isLatitudeMatch = Math.Abs(c.Latitude - expectedLatitude) < tolerance;
        bool isLongitudeMatch = Math.Abs(c.Longitude - expectedLongitude) < tolerance;

        return isLatitudeMatch && isLongitudeMatch;
    }

    /// <summary>
    ///  function to calculate the distance between two addresses
    ///  we use Haversine formula
    /// </summary>
    public static double CalculateDistanceBetweenAddresses(string address1, string address2)
    {
        var (latitude1, longitude1) = GetAddressCoordinates(address1);
        var (latitude2, longitude2) = GetAddressCoordinates(address2);

        const double EarthRadiusKm = 6371.0;

        double latitude1Rad = DegreesToRadians(latitude1);
        double longitude1Rad = DegreesToRadians(longitude1);
        double latitude2Rad = DegreesToRadians(latitude2);
        double longitude2Rad = DegreesToRadians(longitude2);

        double deltaLatitude = latitude2Rad - latitude1Rad;
        double deltaLongitude = longitude2Rad - longitude1Rad;

        double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                   Math.Cos(latitude1Rad) * Math.Cos(latitude2Rad) *
                   Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKm * c;

        return distance;
    }

    /// <summary>
    /// function to transform Degrees To Radians
    /// </summary>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180);
    }


    /// <summary>
    ///class to calculate any type of distance between to entries
    /// </summary>
    public static class DistanceCalculator
    {
        public static double CalculateDistance(string address1, string address2, DistanceType distanceType)
        {
            if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(address2))
            {
                throw new ArgumentException("Addresses cannot be null or empty.");
            }

            switch (distanceType)
            {
                case DistanceType.AirDistance:
                    return CalculateAirDistance(address1, address2);

                case DistanceType.WalkDistance:
                    return CalculateWalkingDistance(address1, address2);

                case DistanceType.CarDistance:
                    return CalculateDrivingDistance(address1, address2);

                default:
                    throw new ArgumentOutOfRangeException(nameof(distanceType), "Invalid distance type.");
            }
        }

        /// <summary>
        /// calulate the air distance with the coordinates
        /// </summary>
        private static double CalculateAirDistance(string address1, string address2)
        {
            var (latitude1, longitude1) = GetAddressCoordinates(address1);
            var (latitude2, longitude2) = GetAddressCoordinates(address2);

            return CalculateDistanceBetweenCoordinates(latitude1, longitude1, latitude2, longitude2);
        }
        private static double CalculateWalkingDistance(string address1, string address2)
        {
            return CalculateTravelDistance(address1, address2, "foot");
        }

        private static double CalculateDrivingDistance(string address1, string address2)
        {
            return CalculateTravelDistance(address1, address2, "driving");
        }


        /// <summary>
        /// calculate driving and walking distance
        /// </summary>=
        private static double CalculateTravelDistance(string address1, string address2, string mode)
        {
            const string LocationIqApiKey = "pk.ddce0bbd11edfee17d07cb35922321f7";
            const string BaseUrl = "https://us1.locationiq.com/v1/directions/";

            var (latitude1, longitude1) = GetAddressCoordinates(address1);
            var (latitude2, longitude2) = GetAddressCoordinates(address2);

            string requestUrl = $"{BaseUrl}{mode}/{longitude1},{latitude1};{longitude2},{latitude2}?key={LocationIqApiKey}&overview=false";

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(requestUrl).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching route data: {response.ReasonPhrase}");
                }

                string responseContent = response.Content.ReadAsStringAsync().Result;

                var routeData = System.Text.Json.JsonSerializer.Deserialize<LocationIqDirectionsResponse>(responseContent);

                if (routeData == null || routeData.Routes == null || routeData.Routes.Length == 0)
                {
                    throw new Exception("No route data found for the provided addresses.");
                }

                return routeData.Routes[0].Distance / 1000.0;
            }
        }

        /// <summary>
        /// calculate the distances between coordinates
        /// </summary>
        private static double CalculateDistanceBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double EarthRadiusKm = 6371.0;

            double latitude1Rad = DegreesToRadians(latitude1);
            double longitude1Rad = DegreesToRadians(longitude1);
            double latitude2Rad = DegreesToRadians(latitude2);
            double longitude2Rad = DegreesToRadians(longitude2);

            double deltaLatitude = latitude2Rad - latitude1Rad;
            double deltaLongitude = longitude2Rad - longitude1Rad;

            double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                       Math.Cos(latitude1Rad) * Math.Cos(latitude2Rad) *
                       Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }
        private class LocationIqDirectionsResponse
        {
            public Route[] Routes { get; set; }
        }
        private class Route
        {
            public double Distance { get; set; }
        }
        private class LocationIqResponse
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
        }
    }


    /// <summary>
    /// returns the riskrange
    /// </summary>
    public static TimeSpan RiskTime(IConfig config)
    {
        if (config == null)
            return TimeSpan.Zero;

        return config.RiskRange;
    }

}