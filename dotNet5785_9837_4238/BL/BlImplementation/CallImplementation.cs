namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Text;

internal class CallImplementation : ICall

{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ChoiceOfCallToCare(int volId, int callId)
    {
        try
        {
            // קריאה לשכבת הנתונים עבור הקריאות, ההקצאות והמתנדבים
            var calls = _dal.Call.ReadAll();
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            // בדיקת קיום הקריאה
            var call = calls.FirstOrDefault(c => c.CallId == callId);
            if (call == null)
            {
                throw new BO.BlArgumentNullException($"Call with ID {callId} does not exist.");
            }

            // בדיקת קיום המתנדב
            var volunteer = volunteers.FirstOrDefault(v => v.VolunteerId == volId);
            if (volunteer == null)
            {
                throw new BO.BlArgumentNullException($"Volunteer with ID {volId} does not exist.");
            }

            // בדיקה שהקריאה לא טופלה ואין הקצאה פתוחה
            bool isCallAssigned = assignments.Any(a => a.CallId == callId && a.FinishTime == null);
            if (isCallAssigned)
            {
                throw new BO.BlInvalidValueException("This call is already assigned to a volunteer.");
            }

            // בדיקה שסטטוס הקריאה לא "Expired" או "Closed"
            if (call.Status == DO.CallStatus.Closed || call.Status == DO.CallStatus.Expired)
            {
                throw new BO.BlInvalidValueException("This call is either closed or expired and cannot be assigned.");
            }

            // יצירת ישות הקצאה חדשה
            var newAssignment = new DO.Assignment
            {
                Id = _dal.Assignment.GetNextId(), // יצירת מזהה ייחודי חדש
                VolunteerId = volId,
                CallId = callId,
                StartTime = DateTime.Now,
                FinishTime = null,
                TypeOfEnd = null
            };

            // הוספת ההקצאה לשכבת הנתונים
            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DalArgumentNullException ex)
        {
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }



    public void Create(BO.Call c)
    {

        if (!Helpers.CallManager.CheckCall(c))
        {
            throw new BO.BlInvalidValueException("Invalid call data");
        }
        try
        {
            _dal.Call.Create(new DO.Call
            {
                CallId = c.CallId,
                CallType = (DO.CallType)c.CallType,
                Address = c.Address,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                OpenTime = c.OpenTime,
                MaxTime = c.MaxTime,
                Details = c.Details,
            });
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlAlreadyExistException(ex.Message);
        }

    }
   

    public void Delete(int callId)
    {
        BO.Call? c = Read(callId);
        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
        if (c.CallStatus != 0 || (c.callAssignInLists != null && c.callAssignInLists.Any(x => assignments.Any(a => a.VolunteerId == x.VolunteerId))))
            throw new BO.BlDeletionImpossible("cant delete this Call.");

        if (!Helpers.CallManager.CheckCall(c))
        {
            throw new BO.BlInvalidValueException("Invalid Call data.");
        }
        try
        {
            _dal.Call.Delete(callId);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    public Call GetCallDetails(int callId)
    {
        try
        {
            BO.Call call = Read(callId);
            return call;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public IEnumerable<CallInList> GetListOfCalls(CallType? filterType = null, object? filterValue = null, CallType? sortType = null)
    {
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
        IEnumerable<DO.Assignment> assigments = _dal.Assignment.ReadAll();
        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();

        IEnumerable<CallInList> callList = from call in calls
                                           let assigment = assigments.First(x => x.CallId == call.CallId)
                                           let volunteer = volunteers.First(x => x.VolunteerId == assigment.VolunteerId)
                                           select new CallInList()
                                           {
                                               Id = assigment.Id,
                                               CallId = assigment.CallId,
                                           };

        return callList;
    }

    public ClosedCallInList GetListOfClosedCall(int volId, CallType? type = null)
    {
        throw new NotImplementedException();
    }

    public OpenCallInList GetListOfOpenCall(int volId, CallType? type = null, OpenCallInList? openCall = null)
    {
        throw new NotImplementedException();
    }

    public Call? Read(int callId)
    {
        try
        {
            BO.Call call = Read(callId);

            if (call == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {callId} was not found.");
            }
            return new BO.Call
            {
                CallId = call.CallId,
                CallType = call.CallType,
                Address = call.Address,
                OpenTime = call.OpenTime,
                MaxTime = call.MaxTime,
                CallStatus = call.CallStatus,
                Details = call.Details,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
            };

        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public int[] SumOfCalls()
    {
        try
        {
            var calls = _dal.Call.ReadAll();

            int[] countOfCalls = calls
                .GroupBy(call => call.CallStatus)
                .Aggregate(
                    new int[5],
                    (arr, group) =>
                    {
                        arr[group.Key] = group.Count();
                        return arr;
                    });

            return countOfCalls;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public void Update(Call c)
    {
        try
        {
            if (!Helpers.CallManager.CheckCall(c))
            {
                throw new BO.BlInvalidValueException("Invalid call data");
            }
            else
            {
                try
                {
                    _dal.Call.Update(new DO.Call
                    {
                        CallId = c.CallId,
                        CallType = (DO.CallType)c.CallType,
                        Address = c.Address,
                        Latitude = c.Latitude,
                        Longitude = c.Longitude,
                        OpenTime = c.OpenTime,
                        MaxTime = c.MaxTime,
                        Details = c.Details,
                    });
                }
                
                catch (DO.DalAlreadyExistException ex)
                {
                    throw new BO.BlAlreadyExistException(ex.Message);
                }
            }
        }


        catch (Exception)
        {

            throw;
        }
    }

    public void UpdateCallCancelled(int id, int assiId)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            var assignment = assignments.FirstOrDefault(a => a.Id == assiId);
            if (assignment == null)
            {
                throw new BO.BlArgumentNullException($"Assignment with ID {assiId} does not exist.");
            }

            var requester = volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (requester == null)
            {
                throw new BO.BlArgumentNullException($"Volunteer with ID {id} does not exist.");
            }

            bool isManager = volunteers.Any(v => v.VolunteerId == id && v.RoleType == DO.Role.Manager);
            bool isAssignedToVolunteer = assignment.VolunteerId == id;

            if (!isManager && !isAssignedToVolunteer)
            {
                throw new Exception("Requester does not have permission to cancel this assignment.");
            }

            if (assignment.FinishTime != null)
            {
                throw new BO.BlInvalidValueException("Assignment has already been closed or expired.");
            }

            assignment = assignment with
            {
                FinishTime = DateTime.Now,
                TypeOfEnd = (DO.TypeOfEnd)((id == requester.VolunteerId)
          ? BO.TypeOfEnd.CancelledByVolunteer
          : BO.TypeOfEnd.CancelledByManager)
            };

            _dal.Assignment.Update(assignment);

        }
        catch (DO.DalArgumentNullException ex)
        {
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }



    public void UpdateCallFinished(int id, int assiId)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            var assignment = assignments.FirstOrDefault(a => a.Id == assiId);
            if (assignment == null)
            {
                throw new BO.BlArgumentNullException($"Assignment with ID {assiId} does not exist.");
            }

            var requester = volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (requester == null)
            {
                throw new BO.BlArgumentNullException($"Volunteer with ID {id} does not exist.");
            }

            bool isAssignedToVolunteer = assignment.VolunteerId == id;

            if (!isAssignedToVolunteer)
            {
                throw new Exception("Requester does not have permission to cancel this assignment.");
            }

            if (assignment.FinishTime != null)
            {
                throw new BO.BlInvalidValueException("Assignment has already been closed or expired.");
            }

            assignment = assignment with
            {
                FinishTime = DateTime.Now,
                TypeOfEnd = (DO.TypeOfEnd) BO.TypeOfEnd.Fulfilled
            };

            _dal.Assignment.Update(assignment);

        }
        catch (DO.DalArgumentNullException ex)
        {
            throw new BO.BlArgumentNullException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
