using Helpers;

namespace BO;

/// <summary>
/// Represents a volunteer with basic details and call-related statistics.
/// </summary>
/// <param name="VolunteerId">Unique identifier of the volunteer.</param>
/// <param name="Name">Full name of the volunteer.</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
/// <param name="sumOfCaredCall">Total number of calls successfully handled by the volunteer.</param>
/// <param name="sumOfCancelledCall">Total number of calls canceled by the volunteer.</param>
/// <param name="sumOfCallExpired">Total number of calls that expired while assigned to the volunteer.</param>
/// <param name="CallId">Identifier of the current call assigned to the volunteer, if any.</param>

public class VolunteerInList
{
    public int VolunteerId { get; init; }
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public int sumOfCaredCall { get; set; }
    public int sumOfCancelledCall { get; set; }

    public int sumOfCallExpired { get; set; }

    public int? CallId { get; init; }

    /// <summary>
    /// toString function 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToStringProperty();


}
