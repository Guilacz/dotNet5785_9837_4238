using Helpers;

namespace BO;

/// <summary>
/// Represents detailed information about a volunteer, including their contact details, role, and activity statistics.
/// </summary>
/// <param name="VolunteerId">Unique identifier of the volunteer.</param>
/// <param name="Name">Full name of the volunteer.</param>
/// <param name="Phone">Phone number of the volunteer.</param>
/// <param name="Email">Email address of the volunteer.</param>
/// <param name="RoleType">Role assigned to the volunteer.</param>
/// <param name="DistanceType">Distance type relevant to the volunteer's role or location.</param>
/// <param name="Password">Password for the volunteer's account, if applicable.</param>
/// <param name="Adress">Address of the volunteer.</param>
/// <param name="Distance">Distance of the volunteer from a central point or call location.</param>
/// <param name="Latitude">Latitude coordinate of the volunteer's location.</param>
/// <param name="Longitude">Longitude coordinate of the volunteer's location.</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
/// <param name="sumOfCaredCall">Total number of calls successfully cared for by the volunteer.</param>
/// <param name="sumOfCancelledCall">Total number of calls canceled by the volunteer.</param>
/// <param name="sumOfCallExpired">Total number of calls that expired while assigned to the volunteer.</param>
/// <param name="callInCaring">The current call in progress being handled by the volunteer, if any.</param>

public class Volunteer
{
    public int VolunteerId { get; init; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    Role RoleType;
    DistanceType DistanceType;

    public string? Password { get; set; }
    public string? Adress { get; set; }
    public double? Distance { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsActive { get; set; }

    public int sumOfCaredCall { get; set; }
    public int sumOfCancelledCall { get; set; }

    public int sumOfCallExpired { get; set ; }
    public BO.CallInProgress? callInCaring { get; set; }

    /// <summary>
    /// toString function 
    /// </summary>
    /// <returns></returns>
    public override string ToString() => this.ToStringProperty();

}
