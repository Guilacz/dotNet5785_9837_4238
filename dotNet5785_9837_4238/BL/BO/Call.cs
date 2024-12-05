namespace BO;
using Helpers;



/// <summary>
/// Represents a call with detailed information, including location and status.
/// </summary>
/// <param name="CallId">Unique identifier of the call.</param>
/// <param name="CallType">Type of the call.</param>
/// <param name="Address">Address associated with the call.</param>
/// <param name="Latitude">Latitude coordinate of the call's location.</param>
/// <param name="Longitude">Longitude coordinate of the call's location.</param>
/// <param name="OpenTime">The time when the call was initiated.</param>
/// <param name="MaxTime">Maximum time allowed for the call to be resolved.</param>
/// <param name="CallStatus">Current status of the call.</param>
/// <param name="Details">Additional details about the call.</param>
/// <param name="callAssignInLists">List of assignments associated with the call.</param>

public class Call
{
    public int CallId { get; init; }

    public CallType CallType { get; init; }

    public string? Address { get; set; }

    public double Latitude { get; set; } 

    public double Longitude { get; set; }

    public DateTime OpenTime { get; init; }

    public DateTime? MaxTime { get; set; } 

    public CallStatus CallStatus { get; set; }

    public string? Details { get; set; }

    public List<BO.CallAssignInList>? callAssignInLists { get; set; }


    public override string ToString() => this.ToStringProperty();
}
