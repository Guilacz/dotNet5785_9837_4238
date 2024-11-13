namespace DalApi;
using DO;

/// <summary>
/// Interface: sort of "contract " that an call entity has to respect
/// </summary>
public interface ICall : ICrud<Call>
{
    string ToString(Call c);
}
