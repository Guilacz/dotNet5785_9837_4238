namespace BO;

/// <summary>
/// all the details of a call assignment
/// </summary>
/// <param name="CallId">unique id of the call</param>
/// <param name="CallType">type of the lesson needeed</param>
/// <param name="Adress">adress of the call</param>
/// <param name="Latitude">Latitude</param>
/// <param name="Longitude">Longitude</param>
/// <param name="OpenTime">when the call was open</param>
/// <param name="Details">details of the call</param>
/// <param name="MaxTime">date maximum to fulfill the demand</param>

public class Call
{
    public int CallId { get; init; }
    public CallType CallType;
    public string? Adress { get; set; }
    public double? Latitude { get; set; } 
    public double? Longitude { get; set; }
    DateTime OpenTime { get; init; }
    public DateTime? MaxTime { get; set; } 

    public CallStatus CallStatus;

    public string? Details { get; set; }

    public List<BO.CallAssignInList>? callAssignInLists { get; set; }
}
