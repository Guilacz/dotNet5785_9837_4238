namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Net;


/// <summary>
/// Volunteer Implementation : implementation of all the elements of the Volunteer Interface
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Stage 5
    public void AddObserver(Action listObserver) => VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) => VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) => VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) => VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


    /// <summary>
    /// function to associate a call with a volunteer 
    /// check that both exists and are correct
    /// check that the call has not been completed already and is not in process by another volunteer
    /// Creates a new assignment and stores it in the database
    /// Throws exceptions for invalid data or improper call/volunteer state.
    /// <param name="volId">The ID of the volunteer.</param>
    /// <param name="callId">The ID of the call.</param>
    /// </summary>
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
                throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");
            BO.Volunteer boVolunteer = Helpers.VolunteerManager.ConvertVolToBO(volunteer);
            if (!Helpers.VolunteerManager.CheckVolunteer(boVolunteer))
                throw new BO.BlInvalidValueException("The volunteer data provided is invalid. Please check the input and try again.");
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
            throw new BO.BlInvalidValueException(ex.Message);
        }
    }


    /// <summary>
    /// function to delete a volunteer 
    /// first verify that he is not in care of a call or has not taken care already
    /// check that the volunteer data is valid and send exceptions for invalid data or deletion failure
    /// delete it with the DO function (to delete in the database)
    /// </summary>
    /// <param name="volId">The ID of the volunteer to delete.</param>
    public void Delete(int volId)
    {
        BO.Volunteer? vol = Read(volId);

        if (vol.SumOfCaredCall != 0 || vol.callInCaring != null)
            throw new BO.BlDeletionImpossible("cant delete this volunteer.");
        if (vol.Latitude == null || vol.Longitude == null)
        {
            var coordinates = Helpers.Tools.GetAddressCoordinates(vol.Address);
            vol.Latitude = coordinates.Latitude;
            vol.Longitude = coordinates.Longitude;
        }
        if (!Helpers.VolunteerManager.CheckVolunteer(vol))
        {
            throw new BO.BlInvalidValueException("Invalid volunteer data.");
        }
        try
        {
            _dal.Volunteer.Delete(volId);
            CallManager.Observers.NotifyListUpdated();
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


    /// <summary>
    /// function to enter the system : Authenticates a volunteer by verifying his name and password. 
    /// If valid, returns the role of the volunteer.
    /// Throws exceptions if the volunteer is not found, the password is incorrect, or the volunteer data is invalid.
    /// </summary>
    /// <param name="name">The name of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role of the authenticated volunteer.</returns>
    public BO.Role EnterSystem(string name, string password)
    {
        try
        {
            var volunteersFromDal = _dal.Volunteer.ReadAll();
            string passwordAsString = password.ToString();

            var vol = volunteersFromDal.FirstOrDefault(v => v.Name == name && v.Password == passwordAsString);

            if (vol == null)
            {
                throw new BO.BlArgumentNullException("Volunteer not found or incorrect password.");
            }
            if (vol.Latitude == null || vol.Longitude == null)
            {
                var coordinates = Helpers.Tools.GetAddressCoordinates(vol.Address);
                vol = vol with { Latitude = coordinates.Latitude, Longitude = coordinates.Longitude };
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
                //Password = vol.Password,
                Address = vol.Address,
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


    /// <summary>
    /// function to get the details of a volunteer from his id : 
    /// Converts the dal volunteer  to a bo volunteer and validates the data.
    /// Throws an exception if the data is invalid or if the volunteer does not exist.
    /// </summary>
    /// <param name="volId">The ID of the volunteer.</param>
    /// <returns>An bo object containing detailed information about the volunteer.</returns>
    public BO.Volunteer GetVolunteerDetails(int volId)
    {
        try
        {
            var vol = Read(volId);
            if (vol == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} not found.");
            }
            if (vol.Latitude == null || vol.Longitude == null)
            {
                var coordinates = Helpers.Tools.GetAddressCoordinates(vol.Address);
                vol.Latitude = coordinates.Latitude;
                vol.Longitude = coordinates.Longitude;
            }
            BO.Volunteer boVolunteer = new BO.Volunteer
            {
                VolunteerId = vol.VolunteerId,
                Name = vol.Name,
                Phone = vol.Phone,
                Email = vol.Email,
                RoleType = (BO.Role)vol.RoleType,
                DistanceType = (BO.DistanceType)vol.DistanceType,
                Password = vol.Password,
                Address = vol.Address,
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
            {
                throw new BO.BlInvalidValueException("Invalid volunteer data.");
            }
            return boVolunteer;
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


    /// <summary>
    /// function to retrieve a list of volunteers with detailed information, including the number of fulfilled, 
    /// cancelled, and expired calls. Optionally filters by activity status and sorts the result 
    /// based on a specified field.
    /// </summary>
    /// <param name="isActive">Optional filter by active status (true/false).</param>
    /// <param name="sort">Optional sorting field (VolunteerId, Name, IsActive, etc.).</param>
    /// <returns>A sorted and filtered list of volunteers in the form of VolunteerInList objects.</returns>
    /// <exception cref="BO.BlArgumentNullException">Thrown when data retrieval fails.</exception>
    /// <exception cref="BO.BlInvalidValueException">Thrown when an error occurs during processing.</exception>

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


    /// <summary>
    /// Reads a volunteer by ID.
    /// Steps:
    /// 1. Retrieve the volunteer from the DAL.
    /// 2. Decrypt the volunteer's password.
    /// 3. Convert the data to a bo
    /// 4. Validate the volunteer.
    /// 5. Return the bo volunteer
    /// </summary>
    /// <param name="volId">The ID of the volunteer to retrieve.</param>
    /// <returns>A BO.Volunteer object containing the volunteer's details.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when no volunteer with the specified ID exists.</exception>
    /// <exception cref="BO.BlInvalidValueException">Thrown when invalid data is encountered in the DAL.</exception>
    public BO.Volunteer? Read(int volId)
    {
        try
        {
            DO.Volunteer volu = _dal.Volunteer.Read(volId);
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
                Address = volunteer.Address,
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


    /// <summary>
    /// Updates the details of a volunteet
    /// Steps:
    /// 1. Decrypt the password
    /// 2. take the volunteer from the dal
    /// 3. Validate the password and check the ID or role matches
    /// 4. Validate the updated volunteer data
    /// 5. Update the volunteer details in the DAL 
    /// </summary>
    /// <param name="volId">The ID of the volunteer to update.</param>
    /// <param name="vol">A BO.Volunteer object containing the updated details.</param>
    /// <exception cref="BO.BlInvalidValueException"/>
    public void Update(int volId, BO.Volunteer vol)
    {
        try
        {
            vol.Password = Helpers.VolunteerManager.DecryptPassword(vol.Password);
            var volun = _dal.Volunteer.Read(volId);
            if (!Helpers.VolunteerManager.CheckValidityOfPassword(volun.Password))
                throw new BO.BlInvalidValueException("Password not strong enough.");

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
                        CallManager.Observers.NotifyItemUpdated(vol.VolunteerId);
                        CallManager.Observers.NotifyListUpdated();
                    }
                }
            }
        }

        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }

    }


    /// <summary>
    /// Creates a new volunteer
    ///  Steps:
    /// 1. Validate the volunteer's data
    /// 2. Validate the volunteer's password 
    /// 3. Encrypt the password
    /// 4. Create the volunteer in DAL
    /// 5. Handle exceptions from the DAL.
    /// </summary>
    /// <param name="vol">A BO.Volunteer object containing the details of the new volunteer.</param>
    /// <exception cref="BO.BlInvalidValueException"/>    
    /// <exception cref="BO.BlAlreadyExistException"/>
    public void Create(BO.Volunteer vol)
    {
        var coordinates = Helpers.Tools.GetAddressCoordinates(vol.Address);
        vol.Latitude = coordinates.Latitude;
        vol.Longitude = coordinates.Longitude;
        if (!Helpers.VolunteerManager.CheckVolunteer(vol))
        {
            throw new BO.BlInvalidValueException("Invalid volunteer data.");
        }
        if (!Helpers.VolunteerManager.CheckValidityOfPassword(vol.Password))
            throw new BO.BlInvalidValueException("Password not strong enough .");
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
                Address = vol.Address,
                IsActive = vol.IsActive,
                Distance = vol.Distance,
            });
            CallManager.Observers.NotifyListUpdated();
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