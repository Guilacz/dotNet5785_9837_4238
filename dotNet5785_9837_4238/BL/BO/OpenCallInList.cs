using Helpers;

namespace BO;

/// <summary>
/// Represents information about a call, including its location and timing details.
/// </summary>
/// <param name="CallId">Unique identifier of the call.</param>
/// <param name="CallType">Type of the call.</param>
/// <param name="Adress">Address associated with the call.</param>
/// <param name="OpenTime">The time when the call was opened.</param>
/// <param name="MaxTime">Maximum time allowed for the call to be resolved, if applicable.</param>
/// <param name="Details">Additional details about the call.</param>
/// <param name="distance">Distance related to the call, possibly to its location.</param>

public class OpenCallInList
{
    public int CallId { get; init; }
    CallType CallType;
    public string Adress { get; set; }

    DateTime OpenTime { get; init; }
    DateTime? MaxTime { get; set; }

    public string? Details { get; set; }
    public double distance { get; init; }

    /// <summary>
    /// toString function 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToStringProperty();

}
