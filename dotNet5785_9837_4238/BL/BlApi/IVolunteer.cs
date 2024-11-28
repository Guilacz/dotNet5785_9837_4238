namespace BlApi;

public interface IVolunteer
{
    BO.Role EnterSystem(string name, int password);

    IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? isActive =null, BO.VolunteerInList? vol = null);

    IEnumerable<BO.Volunteer> GetVolunteerDetails(int volId);

    void Update(int volId, BO.Volunteer vol);

    void Delete(int volId);

    void Create(BO.Volunteer vol);

    //other function 
    BO.Volunteer? Read (int volId);


}
