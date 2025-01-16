using BO;

namespace BlApi;

/// <summary>
/// Volunteer Interface
/// functions : EnterSystem, GetVolunteerInLists, GetVolunteerDetails, Update, Delete, Create, Read
/// </summary>
public interface IVolunteer : IObservable
{
    BO.Role EnterSystem(string name, string password);
    BO.Role EnterSystemWithId(int id, string password);
    string NewPassword();

    int sumOfCalls(int id);



    IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? isActive = null, BO.VolunteerSortField? sort = null);

    IEnumerable<VolunteerInList> GetVolunteersListByCallType(BO.CallType callType);

    BO.Volunteer GetVolunteerDetails(int volId);

    void Update(int volId, BO.Volunteer vol);

    void Delete(int volId);

    void Create(BO.Volunteer vol);

    //other function 
    BO.Volunteer? Read(int volId);


}
