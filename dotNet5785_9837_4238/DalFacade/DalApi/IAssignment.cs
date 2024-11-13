namespace DalApi;
using DO;

/// <summary>
/// Interface: sort of "contract " that an assignment entity has to respect
/// </summary>
public interface IAssignment : ICrud<Assignment>
{
    string ToString(Assignment a);
}
