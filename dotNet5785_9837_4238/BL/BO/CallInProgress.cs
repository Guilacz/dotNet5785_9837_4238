using Helpers;

namespace BO;

/// <summary>
/// Represents a call in progress with detailed information.
/// </summary>
/// <param name="Id">Unique identifier of the call in progress.</param>
/// <param name="CallId">Identifier of the related call.</param>
/// <param name="CallType">Type of the call.</param>
/// <param name="Adress">Address associated with the call.</param>
/// <param name="OpenTime">The time when the call was opened.</param>
/// <param name="StartTime">The time when the call started.</param>
/// <param name="MaxTime">Maximum time allowed for the call to be resolved, if applicable.</param>
/// <param name="Details">Additional details about the call in progress.</param>
/// <param name="distance">Distance related to the call, possibly from the responder to the location.</param>
/// <param name="CallInProgressStatus">Current status of the call in progress.</param>

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType;

    public string Adress { get; set; }

    public DateTime OpenTime { get; init; }
    public DateTime StartTime { get; set; }
    public DateTime? MaxTime { get; set; }

    public string? Details { get; set; }
    public double distance { get; set; }
    public CallInProgressStatus CallInProgressStatus;

    /// <summary>
    /// toString function 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToStringProperty();
}
