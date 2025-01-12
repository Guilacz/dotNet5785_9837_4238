using DO;
using Helpers;

namespace BO;

/// <summary>
/// Represents a volunteer's participation details.
/// </summary>
/// <param name="VolunteerId">Unique identifier of the volunteer.</param>
/// <param name="Name">Name of the volunteer.</param>
/// <param name="StartTime">The time when the volunteer started their task.</param>
/// <param name="TypeOfEnd">Type of end status for the task.</param>
/// <param name="FinishTime">The time when the task was completed, if applicable.</param>

public class CallAssignInList
{
    public int? VolunteerId {  get; init; }
    public string? Name { get; set; }

   
    public DateTime StartTime { get; init; }
    public TypeOfEnd? TypeOfEnd { get; set; }
    public DateTime? FinishTime { get; set; }

    /// <summary>
    /// toString function 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToStringProperty();
}
