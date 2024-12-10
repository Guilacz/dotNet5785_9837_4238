namespace BlImplementation;
using BlApi;

/// <summary>
/// BL Implementation : implementation of all the elements of the BL Interface
/// </summary>
internal class Bl: IBl
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();
}
