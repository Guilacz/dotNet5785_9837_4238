namespace Helpers;
using DalApi;
using DO;

internal class CallManager
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    internal static bool CheckCall(BO.Call c)
    {
        if(CheckTime(c) == false)
            return false;
       // if (CheckAddress(c) == false)
         //   return false;
        return true;
    }
    internal static bool CheckTime(BO.Call c)
    {
        if(c.MaxTime < c.OpenTime)
            return false;
        return true;
    }

    /*
internal static async Task<bool> ValidateVolunteerAddressAsync(Volunteer vol)
{
    // שימוש בערך ברירת מחדל אם Latitude או Longitude הם null
    double latitude = vol.Latitude ?? 0;
    double longitude = vol.Longitude ?? 0;

    return await AreAddressDetailsMatching(vol.Adress, latitude, longitude);
}

internal static async Task<bool> AreAddressDetailsMatching(string address, double latitude, double longitude)
{
    // שלב 1: אימות הכתובת
    string geocodingUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}";
    using (var httpClient = new HttpClient())
    {
        var response = await httpClient.GetAsync(geocodingUrl);
        if (response.IsSuccessStatusCode)
        {
            // שלב 2: קבלת קו אורך וקו רוחב מהשירות
            var json = await JsonSerializer.DeserializeAsync<JsonObject>(await response.Content.ReadAsStreamAsync());
            double serviceLatitude = json["results"]?[0]?["geometry"]?["location"]?["lat"]?.GetValue<double>() ?? 0;
            double serviceLongitude = json["results"]?[0]?["geometry"]?["location"]?["lng"]?.GetValue<double>() ?? 0;

            // שלב 3: השוואה בין הערכים
            return Math.Abs(latitude - serviceLatitude) < 0.00001 && Math.Abs(longitude - serviceLongitude) < 0.00001;
        }
    }

    return false;
}


*/


}
