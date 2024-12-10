namespace BlApi;


/// <summary>
/// BL interface
/// elements : volunteer, call, admin
/// </summary>
public interface IBl
{
    IVolunteer Volunteer {  get; }
    ICall Call { get; }
    IAdmin Admin { get; }
}
