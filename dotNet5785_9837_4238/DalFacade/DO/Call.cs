namespace DO;

/// <summary>
/// all the details of a call
/// </summary>
/// <param name="CallId">unique id of the call</param>
/// <param name="CallType">type of the lesson needeed</param>
/// <param name="Adress">adress of the call</param>
/// <param name="Latitude">Latitude</param>
/// <param name="Longitude">Longitude</param>
/// <param name="OpenTime">when the call was open</param>
/// <param name="Details">details of the call</param>
/// <param name="MaxTime">date maximum to fulfill the demand</param>
public record Call
(
    int CallId,
    CallType CallType,
    string Adress,
    double Latitude,
    double Longitude,
    DateTime OpenTime,
    string? Details = null,
    DateTime? MaxTime = null
)

{
    /// <summary>
    /// default constructor of the call
    /// </summary>
    public Call() : this (0,0,"",0,0,DateTime.Now) { }
}




