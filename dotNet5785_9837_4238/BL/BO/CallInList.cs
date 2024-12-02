using Helpers;

namespace BO;

/// <summary>
/// Represents a call in a list with its associated details.
/// </summary>
/// <param name="Id">Unique identifier of the call in the list.</param>
/// <param name="CallId">Identifier of the related call.</param>
/// <param name="CallType">Type of the call.</param>
/// <param name="OpenTime">Time when the call was opened.</param>
/// <param name="TimeToEnd">Estimated time until the call ends.</param>
/// <param name="LastName">Last name associated with the call.</param>
/// <param name="TimeToCare">Estimated time until the call is taken care of.</param>
/// <param name="CallInListStatus">Status of the call in the list.</param>
/// <param name="numberOfAssignment">Number of assignments related to the call.</param>

public class CallInList
{

    public int Id { get; init; }

    public int CallId { get; init; }

    public CallType CallType;

    public DateTime OpenTime { get; init; }

    public TimeSpan? TimeToEnd { get; set; }

    public string? LastName { get; set; }

    public TimeSpan? TimeToCare { get; set; }

    public CallInListStatus CallInListStatus;

    public int numberOfAssignment { get; set; }


    public override string ToString() => this.ToStringProperty();
}
