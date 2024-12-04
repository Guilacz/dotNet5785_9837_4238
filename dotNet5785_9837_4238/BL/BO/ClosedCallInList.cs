using Helpers;

namespace BO;

/// <summary>
/// Represents detailed information about a call, including its lifecycle and status.
/// </summary>
/// <param name="CallId">Unique identifier of the call.</param>
/// <param name="CallType">Type of the call.</param>
/// <param name="Adress">Address associated with the call.</param>
/// <param name="OpenTime">The time when the call was opened.</param>
/// <param name="StartTime">The time when the call started.</param>
/// <param name="TypeOfEnd">Type of end status for the call, if applicable.</param>
/// <param name="FinishTime">The time when the call was finished, if applicable.</param>

public class ClosedCallInList
{
    public int CallId { get; init; }

    public CallType CallType;
    public string Adress { get; set; }
    public DateTime OpenTime { get; init; }
    public DateTime StartTime { get; set; }
    public TypeOfEnd? TypeOfEnd;
    public DateTime? FinishTime { get; init; }

    /// <summary>
    /// toString function 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToStringProperty();
}
