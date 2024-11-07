namespace DO;

/// <summary>
/// Volunter Entity reprensents a volunteer with all his props
/// </summary>
/// <param name="VolunteerId">Personal unique ID of the volunteer</param>
/// <param name="Name">Private LastName and Firstname of the volunteer</param>
/// <param name="Phone">Private Number of the volunteer</param>
/// <param name="Email">Private Mail of the volunteer</param>
/// <param name="Password">Password of the volunteer</param>
/// <param name="Adress">Private adress of the volunteer</param>
/// <param name="Latitude">latitude</param>
/// <param name="Longitude">Longitude</param>
/// <param name="IsActive">is the volunteer active or not</param>
/// <param name="Distance">distance between the volunteer's adress and the adress of the demand</param>
/// <param name="RoleType">volunteer or manager</param>
/// <param name="DistanceType">AirDistance ,WalkDistance or CarDistance</param>
public record Volunteer
(
    int VolunteerId,
    string Name,
    string Phone,
    string Email,
    Role RoleType,
    Distance DistanceType,

    string? Password = null,
    string? Adress = null,
    double? Distance = null,
    double? Latitude = null,
    double? Longitude = null,
    bool IsActive = true
    
    
)
{
    /// <summary>
    /// Default constructor for chapter 3 stage 1
    /// </summary>
    public Volunteer() : this(0,"","","",0,0) { } 
}





