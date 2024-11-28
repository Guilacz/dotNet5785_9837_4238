namespace BlImplementation;
using BlApi;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Volunteer vol)
    {
        throw new NotImplementedException();
    }

    public void Delete(int volId)
    {
        throw new NotImplementedException();
    }

    public BO.Role EnterSystem(string name, int password)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.Volunteer> GetVolunteerDetails(int volId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? isActive = null, BO.VolunteerInList? vol = null)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer? Read(int volId)
    {
        throw new NotImplementedException();
    }





    public void Update(int volId, BO.Volunteer vol)
    {
        //check validity of all elements
        if (VolunteerManager.IsValidID(vol.VolunteerId) && (VolunteerManager.CheckMail(vol.Email)  {


    }
}
