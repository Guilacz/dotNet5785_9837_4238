namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ChoiceOfCallToCare(int volId, int callId)
    {
        DO.Call? call = _dal.Call.ReadAll().FirstOrDefault(c => c.CallId == callId);
        if (call == null)
            throw new BO.BlInvalidValueException($"Call with ID {callId} does not exist.");
        DO.Volunteer? volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.VolunteerId == volId);
        if (volunteer == null)
            throw new BO.BlInvalidValueException($"Volunteer with ID {volId} does not exist.");
        BO.Call boCall = Helpers.CallManager.ConvertCallToBO(call, _dal);
        if (!Helpers.CallManager.CheckCall(boCall))
            throw new BO.BlInvalidValueException($"Call with ID {callId} is invalid.");
        BO.Volunteer boVolunteer = Helpers.VolunteerManager.ConvertVolToBO(volunteer);
        if (!Helpers.VolunteerManager.CheckVolunteer(boVolunteer))
            throw new BO.BlInvalidValueException($"Volunteer with ID {volId} is invalid.");
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
        bool isCallAlreadyHandled = assignments.Any(a => a.CallId == callId && a.FinishTime != null);
        if (isCallAlreadyHandled)
            throw new InvalidOperationException($"Call with ID {callId} has already been handled.");
        bool isCallInProcess = assignments.Any(a => a.CallId == callId && a.FinishTime == null);
        if (isCallInProcess)
            throw new InvalidOperationException($"Call with ID {callId} is already in process by another volunteer.");
        var newAssignment = new DO.Assignment
        (
            Id: assignments.Any()
                ? assignments.Max(a => a.Id) + 1
                : 1, 
            CallId: callId,
            VolunteerId: volId,
            StartTime: DateTime.Now,
            TypeOfEnd: null, 
            FinishTime: null 
        );
        _dal.Assignment.Create(newAssignment);
    }


    public void Delete(int volId)
    {
        BO.Volunteer? vol = Read(volId);

        if (vol.SumOfCaredCall != 0 || vol.callInCaring != null)
            throw new BO.BlDeletionImpossible("cant delete this volunteer.");

        if (!Helpers.VolunteerManager.CheckVolunteer(vol))
        {
            throw new BO.BlInvalidValueException("Invalid volunteer data.");
        }
        try
        {
            _dal.Volunteer.Delete(volId);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }

    }

    public BO.Role EnterSystem(string name, int password)
    {
        var volunteersFromDal = _dal.Volunteer.ReadAll();
        string passwordAsString = password.ToString();

        var vol = volunteersFromDal.FirstOrDefault(v => v.Name == name && v.Password == passwordAsString);

        if (vol == null)
        {
            throw new BO.BlArgumentNullException("Volunteer not found or incorrect password.");
        }

        if (!Helpers.VolunteerManager.CheckVolunteer(new BO.Volunteer
        {
            VolunteerId = vol.VolunteerId,
            Name = vol.Name,
            Phone = vol.Phone,
            Email = vol.Email,
            RoleType = (BO.Role)vol.RoleType,
            DistanceType = (BO.DistanceType)vol.DistanceType,
            Password = vol.Password,
            Adress = vol.Adress,
            Distance = vol.Distance,
            Latitude = vol.Latitude,
            Longitude = vol.Longitude,
            IsActive = vol.IsActive,
        }))
        {
            throw new BO.BlInvalidValueException("Volunteer not found or incorrect password.");
        }

        return (BO.Role)vol.RoleType;
    }

    public BO.Volunteer GetVolunteerDetails(int volId)
    {
        try
        {
            var vol = Read(volId);

            BO.Volunteer boVolunteer = new BO.Volunteer
            {
                VolunteerId = vol.VolunteerId,
                Name = vol.Name,
                Phone = vol.Phone,
                Email = vol.Email,
                RoleType = (BO.Role)vol.RoleType,
                DistanceType = (BO.DistanceType)vol.DistanceType,
                Password = vol.Password,
                Adress = vol.Adress,
                Distance = vol.Distance,
                Latitude = vol.Latitude,
                Longitude = vol.Longitude,
                IsActive = vol.IsActive,
                SumOfCaredCall = vol.SumOfCaredCall,
                SumOfCancelledCall = vol.SumOfCancelledCall,
                SumOfCallExpired = vol.SumOfCallExpired,
                callInCaring = vol.callInCaring
            };

            if (!Helpers.VolunteerManager.CheckVolunteer(boVolunteer))
                throw new BO.BlAlreadyExistException("Volunteer not found or incorrect password.");
            return boVolunteer;

        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlAlreadyExistException(ex.Message);
        }
    }
    
    public IEnumerable<BO.VolunteerInList> GetVolunteerInLists(bool? isActive = null, BO.VolunteerSortField? sort = null)
    {
        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
            IEnumerable<DO.Assignment> assignment = _dal.Assignment.ReadAll();
            var volunteerInLists = volunteers.Select(v => new BO.VolunteerInList
            {
                VolunteerId = v.VolunteerId,
                Name = v.Name,
                IsActive = v.IsActive,
                SumOfCaredCall = assignment.Count(call => call.VolunteerId == v.VolunteerId && call.TypeOfEnd == DO.TypeOfEnd.Fulfilled),
                SumOfCancelledCall = assignment.Count(call => call.VolunteerId == v.VolunteerId && call.TypeOfEnd == DO.TypeOfEnd.CancelledByVolunteer),
                SumOfCallExpired = assignment.Count(call => call.VolunteerId == v.VolunteerId && call.TypeOfEnd == DO.TypeOfEnd.CancelledAfterTime),
                CallId = assignment.Where(call => call.VolunteerId == v.VolunteerId).Select(call => call.CallId).FirstOrDefault()
            });

            if (isActive.HasValue)
            {
                volunteerInLists = volunteerInLists.Where(v => v.IsActive == isActive.Value);
            }

            volunteerInLists = sort switch
            {
                BO.VolunteerSortField.VolunteerId => volunteerInLists.OrderBy(v => v.VolunteerId),
                BO.VolunteerSortField.Name => volunteerInLists.OrderBy(v => v.Name),
                BO.VolunteerSortField.IsActive => volunteerInLists.OrderBy(v => v.IsActive),
                BO.VolunteerSortField.SumOfCaredCall => volunteerInLists.OrderBy(v => v.SumOfCaredCall),
                BO.VolunteerSortField.SumOfCancelledCall => volunteerInLists.OrderBy(v => v.SumOfCancelledCall),
                BO.VolunteerSortField.SumOfCallExpired => volunteerInLists.OrderBy(v => v.SumOfCallExpired),
                BO.VolunteerSortField.CallId => volunteerInLists.OrderBy(v => v.CallId),
                BO.VolunteerSortField.CallType => volunteerInLists.OrderBy(v => v.CallType),
                _ => volunteerInLists.OrderBy(v => v.VolunteerId)
            };

            return volunteerInLists.ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while getting the volunteer list.", ex);
        }
    }

    public BO.Volunteer? Read(int volId)
    {
        try
        {
            BO.Volunteer volunteer = Read(volId);

            if (volunteer == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} was not found.");
            }
            BO.Volunteer vol = new BO.Volunteer
            {
                VolunteerId = volunteer.VolunteerId,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Email,
                RoleType = volunteer.RoleType,
                DistanceType =volunteer.DistanceType,
                Password = volunteer.Password,
                Adress = volunteer.Adress,
                Distance = volunteer.Distance,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                IsActive = volunteer.IsActive,
                SumOfCaredCall = volunteer.SumOfCaredCall,
                SumOfCancelledCall = volunteer.SumOfCancelledCall,
                SumOfCallExpired = volunteer.SumOfCallExpired,
                callInCaring = volunteer.callInCaring
            };
            return vol;
        }

        catch (Exception ex)
        {
            // Optionally, log the exception or handle additional scenarios
            throw new Exception(ex.Message);
        }
    }

    public void Update(int volId, BO.Volunteer vol)
    {
        try
        {
            var volun = _dal.Volunteer.Read(volId);
            if (!Helpers.VolunteerManager.CheckPassword(volun.Password))
                throw new BlInvalidValueException("Volunteer not found or incorrect password.");

            if (!(volun.VolunteerId == volId || volun.RoleType == 0))

                throw new BlInvalidValueException("Volunteer not found or incorrect password.");
            else
            {
                if (!Helpers.VolunteerManager.CheckVolunteer(vol))
                    throw new BO.BlInvalidValueException("Volunteer not found or incorrect password.");
                else
                {
                    if (vol.RoleType == 0)
                    {
                        DO.Volunteer DOvolunteer = VolunteerManager.DOManeger(vol);
                        _dal.Volunteer.Update(DOvolunteer);
                    }
                    else
                    {
                        DO.Volunteer DOvolunteer = VolunteerManager.DOVolunteer(volun, vol);
                        _dal.Volunteer.Update(DOvolunteer);
                    }

                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Create(BO.Volunteer vol)
    {
        if (!Helpers.VolunteerManager.CheckVolunteer(vol))
        {
            throw new BO.BlInvalidValueException("Invalid call data");
        }
        if (!Helpers.VolunteerManager.CheckPassword(vol.Password))
            throw new BlInvalidValueException("Volunteer not found or incorrect password.");
        try
        {
            _dal.Volunteer.Create(new DO.Volunteer
            {
                VolunteerId = vol.VolunteerId,
                RoleType = (DO.Role)vol.RoleType,
                DistanceType = (DO.Distance)vol.DistanceType,
                Name = vol.Name,
                Latitude = vol.Latitude,
                Longitude = vol.Longitude,
                Phone = vol.Phone,
                Email = vol.Email,
                Password = vol.Password,
                Adress = vol.Adress,
                IsActive = vol.IsActive,
                Distance = vol.Distance,

            });
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlAlreadyExistException(ex.Message);
        }
    }
}

