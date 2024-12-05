namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ChoiceOfCallToCare(int volId, int callId)
    {
        try
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
                throw new BO.BlInvalidValueException($"Call with ID {callId} has already been handled.");
            bool isCallInProcess = assignments.Any(a => a.CallId == callId && a.FinishTime == null);
            if (isCallInProcess)
                throw new BO.BlInvalidValueException($"Call with ID {callId} is already in process by another volunteer.");
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
        catch (DO.DalInvalidValueException ex)
        {
            // טיפול בחריגה של InvalidValue (למשל, קריאה או מתנדב שלא קיימים)
            throw new BO.BlInvalidValueException(ex.Message);
        }
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
        catch (DO.DalDeletionImpossible ex)
        {
            throw new BO.BlDeletionImpossible(ex.Message);
        }
        catch (DO.DalInvalidValueException ex) 
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }

    }
    public BO.Role EnterSystem(string name, int password)
    {
        try
        {
            var volunteersFromDal = _dal.Volunteer.ReadAll();
            string passwordAsString = password.ToString();
            passwordAsString = Helpers.VolunteerManager.EncryptPassword(passwordAsString);

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
                Password = Helpers.VolunteerManager.DecryptPassword(vol.Password),
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
        catch (DO.DalArgumentNullException ex)
        {
            // This handles the specific case where the volunteer is not found or password is incorrect
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (DO.DalInvalidValueException ex)
        {
            // This handles the specific case where the volunteer data is invalid
            throw new BO.BlInvalidValueException(ex.Message);
        }
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
                throw new BO.BlAlreadyExistException("Incorrect call.");
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
            throw new BO.BlInvalidValueException(ex.Message);
        }
    }
    public BO.Volunteer? Read(int volId)
    {
        try
        {
            DO.Volunteer volu1 = _dal.Volunteer.Read(volId);
            DO.Volunteer volu = volu1 with { Password = Helpers.VolunteerManager.DecryptPassword(volu1.Password) };
            BO.Volunteer volunteer = Helpers.VolunteerManager.ConvertVolToBO(volu);
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
                DistanceType = volunteer.DistanceType,
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
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }
    }
    public void Update(int volId, BO.Volunteer vol)
    {
        try
        {
            vol.Password = Helpers.VolunteerManager.DecryptPassword(vol.Password);
            var volun = _dal.Volunteer.Read(volId);
            if (!Helpers.VolunteerManager.CheckPassword(volun.Password))
                throw new BO.BlInvalidValueException("Volunteer not found or incorrect password.");

            if (!(volun.VolunteerId == volId || volun.RoleType == 0))
                throw new BO.BlInvalidValueException("Volunteer not found or incorrect password.");
            else
            {
                if (!Helpers.VolunteerManager.CheckVolunteer(vol))
                    throw new BO.BlInvalidValueException("Invalid volunteer data.");
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

        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }

    }
    public void Create(BO.Volunteer vol)
    {
        if (!Helpers.VolunteerManager.CheckVolunteer(vol))
        {
            throw new BO.BlInvalidValueException("Invalid volunteer data.");
        }
        if (!Helpers.VolunteerManager.CheckPassword(vol.Password))
            throw new BO.BlInvalidValueException("Volunteer not found or incorrect password.");
        vol.Password = Helpers.VolunteerManager.EncryptPassword(vol.Password);
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
        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }
    }
}