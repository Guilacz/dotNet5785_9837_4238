namespace DalApi;

using DO;

/// <summary>
/// Interface: sort of "contract " that an volunteer entity has to respect
/// </summary>
public interface IVolunteer : ICrud<Volunteer>
{
    string ToString(Volunteer v);
}
