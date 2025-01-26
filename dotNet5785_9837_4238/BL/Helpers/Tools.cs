namespace Helpers;
using BO;
using DalApi;
using System.Net;
using System.Net.Mail;
using System.Text;


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
    //public static (double Latitude, double Longitude) GetAddressCoordinates(string address)
    //{
    //    if (string.IsNullOrWhiteSpace(address))
    //    {
    //        throw new ArgumentException("Address cannot be null or empty.", nameof(address));
    //    }

    //    //braha
    //   // const string LocationIqApiKey = "pk.ddce0bbd11edfee17d07cb35922321f7";
    //    ///guila
    //  const string LocationIqApiKey = "pk.ff579c3ac84dedc53e60bd54521cc03e";
    //    const string BaseUrl = "https://us1.locationiq.com/v1/search.php";

    //    string requestUrl = $"{BaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";

    //    using (var client = new HttpClient())
    //    {
    //        HttpResponseMessage response;
    //        try
    //        {
    //            response = client.GetAsync(requestUrl).Result;
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception("Error sending request to LocationIQ API.", ex);
    //        }

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            throw new Exception($"Error fetching data from LocationIQ: {response.ReasonPhrase}");
    //        }

    //        string responseContent = response.Content.ReadAsStringAsync().Result;


    //        var locationData = System.Text.Json.JsonSerializer.Deserialize<List<LocationIqResponse>>(responseContent);

    //        if (locationData == null || locationData.Count == 0)
    //        {
    //            throw new Exception("No coordinates found for the provided address.");
    //        }

    //        var coordinates = locationData[0];


    //        //bool isLatValid = double.TryParse(coordinates.lat, out double latitude);
    //        //bool isLonValid = double.TryParse(coordinates.lon, out double longitude);

    //        bool isLonValid = double.TryParse(coordinates.lon,
    //                              System.Globalization.NumberStyles.Float,
    //                              System.Globalization.CultureInfo.InvariantCulture,
    //                              out double longitude);
    //        bool isLatValid = double.TryParse(coordinates.lat,
    //                                          System.Globalization.NumberStyles.Float,
    //                                          System.Globalization.CultureInfo.InvariantCulture,
    //                                          out double latitude);




    //        if (isLatValid && isLonValid)
    //        {
    //            return (latitude, longitude);
    //        }
    //        else
    //        {
    //            throw new Exception($"Invalid coordinate data. Latitude valid: {isLatValid}, Longitude valid: {isLonValid}");
    //        }
    //    }
    //}

    private static readonly SemaphoreSlim _throttler = new SemaphoreSlim(1);

    public static async Task <(double Latitude, double Longitude)> GetAddressCoordinatesAsync(string address)
    {
        await _throttler.WaitAsync();
        try
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be null or empty.", nameof(address));
            }

            //braha
           // const string LocationIqApiKey = "pk.ddce0bbd11edfee17d07cb35922321f7";
            ///guila
             const string LocationIqApiKey = "pk.ff579c3ac84dedc53e60bd54521cc03e";
            const string BaseUrl = "https://us1.locationiq.com/v1/search.php";
            string requestUrl = $"{BaseUrl}?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";

            using (var client = new HttpClient())
            {
                //await Task.Delay(1000); 

                HttpResponseMessage response;
                try
                {
                    // response = client.GetAsync(requestUrl).Result;
                    response = await client.GetAsync(requestUrl);

                }
                catch (Exception ex)
                {
                    throw new Exception("Error sending request to LocationIQ API.", ex);
                }

                if (!response.IsSuccessStatusCode)
                {
                    //var content = response.Content.ReadAsStringAsync().Result;
                    var content = await response.Content.ReadAsStringAsync();

                    throw new Exception($"Error fetching data from LocationIQ: {response.ReasonPhrase}, Content: {content}");
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                var locationData = System.Text.Json.JsonSerializer.Deserialize<List<LocationIqResponse>>(responseContent);

                if (locationData == null || locationData.Count == 0)
                {
                    throw new Exception("No coordinates found for the provided address.");
                }

                var coordinates = locationData[0];
                bool isLonValid = double.TryParse(coordinates.lon,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double longitude);
                bool isLatValid = double.TryParse(coordinates.lat,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double latitude);

                if (isLatValid && isLonValid)
                {
                    return (latitude, longitude);
                }
                else
                {
                    throw new Exception($"Invalid coordinate data. Latitude valid: {isLatValid}, Longitude valid: {isLonValid}");
                }
            }
        }
        finally
        {
            _throttler.Release();
        }
    }

    /// <summary>
    /// class to get the latitude and longitude of a LocationIqResponse
    /// </summary>
    private class LocationIqResponse
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }

    /// <summary>
    /// function that checks if the coordinates of a volunteer match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static async Task<bool> CheckAddressVolunteer(Volunteer vol)
    {
        if (vol.Latitude == null || vol.Longitude == null)
        {
            throw new Exception("Latitude or Longitude is null.");
        }

        var (expectedLatitude, expectedLongitude) =await Tools.GetAddressCoordinatesAsync(vol.Address);

        const double tolerance = 0.0001;
        bool isLatitudeMatch = Math.Abs(vol.Latitude.Value - expectedLatitude) < tolerance;
        bool isLongitudeMatch = Math.Abs(vol.Longitude.Value - expectedLongitude) < tolerance;

        return isLatitudeMatch && isLongitudeMatch;
    }

    /// <summary>
    /// function that checks if the coordinates of a call match the coordinates based on his address. 
    /// we use the function GetAddressCoordinates to compare the expected coordinates with the received , allowing a small tolerance
    /// </summary>
    public static async Task< bool> CheckAddressCall(Call c)
    {
        if (!c.Latitude.HasValue || !c.Longitude.HasValue)
        {
            throw new Exception("Latitude or Longitude is null for the call.");
        }

        var (expectedLatitude, expectedLongitude) =await Tools.GetAddressCoordinatesAsync(c.Address);
        const double tolerance = 0.0001;
        bool isLatitudeMatch = Math.Abs(c.Latitude.Value - expectedLatitude) < tolerance;
        bool isLongitudeMatch = Math.Abs(c.Longitude.Value - expectedLongitude) < tolerance;

        return isLatitudeMatch && isLongitudeMatch;
    }

    /// <summary>
    ///  function to calculate the distance between two addresses
    ///  we use Haversine formula
    /// </summary>
    public static async Task<double> CalculateDistanceBetweenAddresses(DO.Volunteer? vol, DO.Call call)
    {
        //var (latitude1, longitude1) =await GetAddressCoordinatesAsync(address1);
        //var (latitude2, longitude2) =await GetAddressCoordinatesAsync(address2);

        if (vol.Latitude == null || vol.Longitude == null || call.Latitude == null || call.Longitude == null)
        {
            throw new ArgumentException("There is a problem with the coordinates.");
        }

        var latitude1 = vol.Latitude ?? 0.0;
        var longitude1 = vol.Longitude ?? 0.0;
        var latitude2 = call.Latitude;
        var longitude2 = call.Longitude;

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
        public static async Task <double> CalculateDistance(string address1, string address2, DistanceType distanceType)
        {
            if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(address2))
            {
                throw new ArgumentException("Addresses cannot be null or empty.");
            }

            switch (distanceType)
            {
                case DistanceType.AirDistance:
                    return await CalculateAirDistance(address1, address2);


                default:
                    throw new ArgumentOutOfRangeException(nameof(distanceType), "Invalid distance type.");
            }
        }

        /// <summary>
        /// calulate the air distance with the coordinates
        /// </summary>
        private static async Task<double> CalculateAirDistance(string address1, string address2)
        {
            var (latitude1, longitude1) =await GetAddressCoordinatesAsync(address1);
            var (latitude2, longitude2) =await GetAddressCoordinatesAsync(address2);

            return CalculateDistanceBetweenCoordinates(latitude1, longitude1, latitude2, longitude2);
        }


        public class RouteResponse
        {
            public Route[] Routes { get; set; }
        }

        public class Route
        {
            public double Distance { get; set; }
        }
        /// <summary>
        /// calculate the distances between coordinates
        /// </summary>
        public static double CalculateDistanceBetweenCoordinates(double latitude1, double longitude1, double latitude2, double longitude2)
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

    public static async Task SendEmail(string toAddress, string subject, string body)
    {
        // כתובת המייל והשדות של השולח
        string fromAddress = "brachakal2225@gmail.com"; // כתובת המייל שלך
        string fromPassword = "xgba ufsc kkhd zdzg";  // הסיסמה שלך (או App Password במקרה של Gmail)
        // יצירת הודעת המייל
        MailMessage mail = new MailMessage
        {
            From = new MailAddress(fromAddress),
            Subject = subject,
            Body = body,
            IsBodyHtml = false // אם את רוצה HTML, שימי true
        };
        mail.To.Add(toAddress);

        // הגדרת שרת ה-SMTP
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587) // יש להחליף את שם שרת ה-SMTP
        {
            Credentials = new NetworkCredential(fromAddress, fromPassword),
            EnableSsl = true // אם נדרש SSL (ב-Gmail, למשל)
        };

        try
        {

            //   smtp.Send(mail); // שליחת המייל
            await smtp.SendMailAsync(mail); // אסינכרוני

            Console.WriteLine($"Email was sended to: {toAddress}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {toAddress}: {ex.Message}");
        }
    }


}