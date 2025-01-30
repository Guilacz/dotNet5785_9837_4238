namespace BlImplementation;
using BlApi;
using BO;
 
using Helpers;


using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// Call Implementation : implementation of all the elements of the Call Interface
/// </summary>
internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Stage 5
    public void AddObserver(Action listObserver) => CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) => CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) => CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) => CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


    /// <summary>
    /// Assigns a specific call to a specific volunteer, check that all conditions are ok:
    /// 1.Verifies that the call and volunteer exist in the database.
    /// 2.Validates the data of the call and volunteer.
    /// 3. check that the call has not already been cared or is not in progress.
    /// 4.Creates a new assignment between the call and volunteer 
    /// </summary>
    /// <param name="volId">The ID of the volunteer to assign.</param>
    /// <param name="callId">The ID of the call to be assigned.</param>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    /// <exception cref="BO.BlInvalidValueException"></exception>

    public void ChoiceOfCallToCare(int volId, int callId)
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7

        CallManager.ChoiceOfCallToCare(volId, callId);
    }

 
  
    


    /// <summary>
    /// function to delete a call
    /// 1.find the call by its ID and checks its status and assignments
    /// 2.check that the call is not in progress or associated with active assignments
    /// 3.check the data of the call
    /// 4.if everything is ok,deletes the call from the database
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    /// <exception cref="BO.BlDeletionImpossible"></exception>
    /// <exception cref="BO.BlInvalidValueException"></exception>

    public void Delete(int callId)
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        lock (AdminManager.BlMutex)  //stage 7
            CallManager.Delete (callId);

    }


    /// <summary>
    /// function to get the details of a call
    /// Note:this function doesnt need to be in a try-catch, because the ReadAll method does not throw exceptions.
    /// </summary>
    /// <param name="callId"></param>
    public Call GetCallDetails(int callId)
    {
       
            BO.Call call = Read(callId);
            return call;
        
    }



    /// <summary>
    /// function to get a list of calls as BO.CallInList objects with optional filtering and sorting:
    /// 1.takes all calls and assignments from the databas
    /// 2. Applies an optional filter if received one
    /// 3.Converts the calls into BO.CallInList objects 
    /// 4.if their is a received sort:  Applies it, if not :sort by ID
    /// </summary>
    /// <param name="filterType">Optional: The type of filter to apply.</param>
    /// <param name="filterValue">Optional: The value to filter by.</param>
    /// <param name="sortType">Optional: The type of sorting to apply .</param>
    /// <returns>A sorted and filtered list of calls as BO.CallInList objects.</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>


    internal static BO.CallInListStatus ConvertCallStatusToCallInListStatus(BO.CallInListStatus status, bool isAtRisk)
    {
        return status switch
        {
            BO.CallInListStatus.Open => isAtRisk ? BO.CallInListStatus.OpenAtRisk : BO.CallInListStatus.Open,
            BO.CallInListStatus.InCare => isAtRisk ? BO.CallInListStatus.InCareAtRisk : BO.CallInListStatus.InCare,
            BO.CallInListStatus.Closed => BO.CallInListStatus.Closed,
            BO.CallInListStatus.Expired => BO.CallInListStatus.Expired,
            _ => BO.CallInListStatus.Open // ברירת מחדל
        };
    }


    /// <summary>
    /// function to get a list of calls as BO.CallInList objects with optional filtering and sorting:
    /// 1.takes all calls and assignments from the databas
    /// 2. Applies an optional filter if received one
    /// 3.Converts the calls into BO.CallInList objects 
    /// 4.if their is a received sort:  Applies it, if not :sort by ID
    /// </summary>
    /// <param name="filterType">Optional: The type of filter to apply.</param>
    /// <param name="filterValue">Optional: The value to filter by.</param>
    /// <param name="sortType">Optional: The type of sorting to apply .</param>
    /// <returns>A sorted and filtered list of calls as BO.CallInList objects.</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    /// 

    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListSort? filterType = null, object? filterValue = null, BO.CallInListSort? sortType = null)
    {
        try
        {


            IEnumerable<DO.Assignment> assignments;
            IEnumerable<DO.Volunteer> volunteers;
            IEnumerable<DO.Call> calls;
            lock (AdminManager.BlMutex)
            { 
                calls = _dal.Call.ReadAll();
                assignments = _dal.Assignment.ReadAll();
                volunteers = _dal.Volunteer.ReadAll();
            }
                


            if (calls == null || !calls.Any())
            {
                return Enumerable.Empty<BO.CallInList>();
            }

            if (assignments == null)
            {
                lock (AdminManager.BlMutex) //stage 7
                    assignments = Enumerable.Empty<DO.Assignment>();
            }

            if (filterType != null && filterValue != null)
            {
                calls = filterType switch
                {
                    BO.CallInListSort.CallId =>
                        int.TryParse(filterValue.ToString(), out int filterId) ?
                        calls.Where(c => c.CallId == filterId) : calls,

                    BO.CallInListSort.CallType =>
                        Enum.TryParse(typeof(DO.CallType), filterValue.ToString(), out var callType) ?
                        calls.Where(c => c.CallType == (DO.CallType)callType) : calls,

                    BO.CallInListSort.OpenTime =>
                        filterValue is DateTime filterDate ?
                        calls.Where(c => c.OpenTime.Date == filterDate.Date) : calls,

                    BO.CallInListSort.LastName =>
                        filterValue is string volunteerName ?
                        calls.Where(c =>
                            assignments.Any(a => a.CallId == c.CallId &&
                                                 volunteers.Any(v => v.VolunteerId == a.VolunteerId &&
                                                                     v.Name.Contains(volunteerName, StringComparison.OrdinalIgnoreCase)))) :
                        calls,

                    BO.CallInListSort.CallInListStatus =>
                        Enum.TryParse(typeof(BO.CallInListStatus), filterValue.ToString(), out var status) ?
                        calls.Where(c =>
                        {
                            // קבלת סטטוס הקריאה לפי ההמרה
                            var boCall = Helpers.CallManager.ConvertCallToBO(c, _dal);
                            var isAtRisk = boCall.CallStatus == BO.CallInListStatus.Open && boCall.MaxTime.HasValue && boCall.MaxTime.Value < DateTime.Now;
                            var convertedStatus = ConvertCallStatusToCallInListStatus(boCall.CallStatus, isAtRisk);
                            return convertedStatus == (BO.CallInListStatus)status;
                            // return status;

                        }) :
                        calls,

                    _ => calls
                };
            }

            var callInList = calls.Select(c =>
            {
                var boCall = Helpers.CallManager.ConvertCallToBO(c, _dal); // המרה מ-DO ל-BO

                // חיפוש ההקצאה האחרונה של הקריאה
                var lastAssignment = assignments
                    .Where(a => a.CallId == c.CallId)
                    .OrderByDescending(a => a.FinishTime ?? a.StartTime) // מסדר לפי FinishTime אם קיים, אחרת לפי StartTime
                    .FirstOrDefault();

                string? lastVolunteerName;
                // אם מצאנו הקצאה אחרונה, נחפש את המתנדב שקשור לה
                lock (AdminManager.BlMutex)  //stage 7
                {
                    lastVolunteerName = lastAssignment != null
                   ? _dal.Volunteer.ReadAll().FirstOrDefault(v => v.VolunteerId == lastAssignment.VolunteerId)?.Name
                   : null;
                }

                // חישוב TimeToCare
                var timeToCare = boCall.MaxTime.HasValue && lastAssignment?.StartTime != null
      ? boCall.MaxTime.Value.Subtract(lastAssignment.StartTime) // חישוב MaxTime פחות StartTime
      : (TimeSpan?)null; // אם אחד מהם חסר, משאירים null
                         // var timeToCare = boCall.MaxTime.Value.Subtract(lastAssignment.StartTime);
                return new BO.CallInList
                {
                    CallId = boCall.CallId,
                    CallType = boCall.CallType,
                    OpenTime = boCall.OpenTime,
                    LastName = lastVolunteerName, // שם המתנדב שטיפל בקריאה
                    TimeToEnd = boCall.MaxTime.HasValue
                                 ? boCall.MaxTime.Value.Subtract(DateTime.Now) // MaxTime פחות השעה הנוכחית
                             : (TimeSpan?)null, // אם אין MaxTime, משאירים null
                    TimeToCare = timeToCare, // חישוב הזמן שנותר עד לסיום הקריאה
                    CallInListStatus = (BO.CallInListStatus)Helpers.CallManager.GetCallStatus(c, assignments),
                    NumberOfAssignment = assignments.Count(a => a.CallId == boCall.CallId)
                };
            }).ToList();

            if (sortType != null)
            {
                callInList = sortType switch
                {
                    BO.CallInListSort.CallType => callInList.OrderBy(c => c.CallType).ToList(),
                    BO.CallInListSort.OpenTime => callInList.OrderBy(c => c.OpenTime).ToList(),
                    BO.CallInListSort.TimeToCare => callInList.OrderBy(c => c.TimeToCare ?? TimeSpan.MaxValue).ToList(),
                    BO.CallInListSort.CallInListStatus => callInList.OrderBy(c => c.CallInListStatus).ToList(),
                    BO.CallInListSort.LastName => callInList.OrderBy(c => c.LastName).ToList(),
                    _ => callInList.OrderBy(c => c.CallId).ToList()
                };
            }
            else
            {
                callInList = callInList.OrderBy(c => c.CallId).ToList();
            }

            return callInList;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }

    




    /// <summary>
    /// function to get a list of closed calls 
    /// 1. gets all assignments for the specified volunteer and the related calls from the database
    /// 2. Matches assignments with their corresponding calls and filters by call type if specified.
    /// 3. Converts the data into BO.ClosedCallInList objects containing the call's ID, type, and type of closure.
    /// 4. Sorts the list based on the provided sorting criterion or defaults to sorting by CallId.
    /// 5. Returns the final filtered and sorted list of closed calls.
    /// Note: The ReadAll methods for assignments and calls do not throw exceptions.
    /// </summary>
    /// <param name="volId">The ID of the volunteer whose closed calls should be retrieved.</param>
    /// <param name="type">Optional: The type of calls to filter by.</param>
    /// <param name="sort">Optional: The sorting criterion to apply </param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown in case of unexpected errors during processing.</exception>

    

    public IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? sort = null)
    {
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex)  //stage 7
            volunteer = _dal.Volunteer.Read(volId)
                       ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} does not exist.");

        IEnumerable<DO.Assignment?> assignments;
        lock (AdminManager.BlMutex)  //stage 7
            assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.VolunteerId);
        if (assignments == null || !assignments.Any())
            return Enumerable.Empty<BO.ClosedCallInList>();



        lock (AdminManager.BlMutex)  //stage 7
        {
            var closedCallsOfVolunteer =
            from assignment in assignments
            let call = _dal.Call.Read(assignment.CallId)
            where call != null && (type == null || (BO.CallType)call.CallType == type.Value)
            select new BO.ClosedCallInList
            {
                CallId = call.CallId,
                CallType = (BO.CallType)call.CallType,
                Adress = call.Address,
                OpenTime = call.OpenTime,
                StartTime = assignment.StartTime,
                TypeOfEnd = assignment.TypeOfEnd.HasValue ? (BO.TypeOfEnd?)assignment.TypeOfEnd.Value : null,
                FinishTime = assignment.FinishTime ?? null
            };



            return sort switch
            {
                BO.CloseCallInListSort.CallId => closedCallsOfVolunteer.OrderBy(c => c.CallId),
                BO.CloseCallInListSort.CallType => closedCallsOfVolunteer.OrderBy(c => c.CallType),
                BO.CloseCallInListSort.TypeOfEnd => closedCallsOfVolunteer.OrderBy(c => c.TypeOfEnd),
                _ => closedCallsOfVolunteer
            };

        }
    }


    //public IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? sort = null)
    //{
    //    var volunteer = _dal.Volunteer.Read(volId);
    //    //??
    //    //    throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} does not exist.");

    //    var closedCallsOfVolunteer =
    //        from assignment in _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.VolunteerId)
    //        let call = _dal.Call.Read(assignment.CallId)
    //        where type == null || (BO.CallType)call.CallType == type.Value
    //        select new BO.ClosedCallInList()
    //        {
    //            CallId = call.CallId,
    //            CallType = (BO.CallType)call.CallType,
    //            Adress = call.Address,
    //            OpenTime = call.OpenTime,
    //            StartTime = assignment.StartTime,
    //            TypeOfEnd = assignment.TypeOfEnd.HasValue  ? (BO.TypeOfEnd?)assignment.TypeOfEnd.Value  : null,
    //            FinishTime = assignment.FinishTime ?? null
    //        };

    //    return sort switch
    //    {
    //        BO.CloseCallInListSort.CallId => from closedCall in closedCallsOfVolunteer
    //                                         orderby closedCall.CallId
    //                                         select closedCall,
    //        BO.CloseCallInListSort.CallType => from closedCall in closedCallsOfVolunteer
    //                                           orderby closedCall.CallType
    //                                           select closedCall,
    //        BO.CloseCallInListSort.TypeOfEnd => from closedCall in closedCallsOfVolunteer
    //                                            orderby closedCall.TypeOfEnd
    //                                            select closedCall,
    //        _ => from closedCall in closedCallsOfVolunteer
    //             orderby closedCall.CallId
    //             select closedCall
    //    };
    //}


    /// <summary>
    /// function to get a list of open calls 
    /// 1. get all calls, assignments, and the volunteer's data from the database.
    /// 2. Filters calls to include only those with an open or open-at-risk status.
    /// 3. Filters the calls further by the specified call type if provided.
    /// 4. Converts the data into BO.OpenCallInList objects, calculating the distance between the volunteer's address and the call's address.
    /// 5. Sorts the list based on the provided sorting criterion  or defaults to sorting by CallId.
    /// 6. Returns the final filtered and sorted list of open calls.
    /// Note: The ReadAll methods for calls and assignments, and the Read method for the volunteer, do not throw exceptions.
    /// </summary>
    /// <param name="volId">The ID of the volunteer whose open calls should be retrieved.</param>
    /// <param name="type">Optional: The type of calls to filter by.</param>
    /// <param name="openCall">Optional: The sorting criterion to apply (e.g., by CallId, CallType, Address, etc.).</param>
    /// <returns>A list of open calls as BO.OpenCallInList objects.</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    

    public void Create(BO.Call c)
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        lock (AdminManager.BlMutex)  //stage 7

            CallManager.Create(c);
    }

    
    

   

    public IEnumerable<BO.OpenCallInList> GetListOfOpenCall(int volId, BO.CallType? type = null, OpenCallInListSort? openCall = null)
    {
        return CallManager.GetListOfOpenCall(volId, type, openCall);
    }
    

    private async Task ValidateOpenCallAddressesAsync(IEnumerable<DO.Call> calls)
    {
        foreach (var call in calls)
        {
            try
            {
                var boCall = Helpers.CallManager.ConvertCallToBO(call, _dal);
                var coordinates = await Helpers.Tools.GetAddressCoordinatesAsync(call.Address);

                boCall.Latitude = coordinates.Latitude;
                boCall.Longitude = coordinates.Longitude;

                if (!await Helpers.Tools.CheckAddressCall(boCall))
                {
                    // Log or handle invalid address
                    // Potentially mark call as invalid or remove from processing
                }
            }
            catch (Exception)
            {
                // Log or handle coordinate/validation errors
            }
        }
    }
    ///////// <summary>
    /// function to read a call
    /// steps : 
    /// 1.read the call data using the provided call ID.
    /// 2. If the call is found, it creates and returns a BO.Call object with the relevant details (CallId, CallType, Address, etc.).
    /// 3. If the call is not found, it throws a BO.BlDoesNotExistException with a message indicating the call was not found.
    /// Exceptions:Throws BO.BlDoesNotExistException if the call is not found or if an error occurs during processing.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>A BO.Call object containing the details of the call, or null if the call doesn't exist.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call with the given ID is not found.</exception>
    public  Call? Read(int callId)
    {
        lock (AdminManager.BlMutex)  //stage 7

            return CallManager.Read(callId);
    }



    /// <summary>
    /// returns the count of calls grouped by their status 
    /// steps : 
    /// 1. gets all call and assignment data from the database.
    /// 2. Groups the calls by their status
    /// 3. Aggregates the grouped calls, counting the number of calls in each status group and storing the results in an array.
    /// 4. Returns an array of int where each index represents a call status, and the value represents the number of calls in that status.
    /// Exceptions:Throws BO.BlDoesNotExistException if an error occurs during data processing or aggregation.
    /// </summary>
    /// <returns>An array of integers, where each element represents the number of calls for a specific status.</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public int[] SumOfCalls()
    {
        try
        {

            IEnumerable<DO.Call> calls;
            lock (AdminManager.BlMutex)  //stage 7
                calls = _dal.Call.ReadAll();

            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)  //stage 7
                assignments = _dal.Assignment.ReadAll();

            int[] countOfCalls = calls
                .GroupBy(call => Helpers.CallManager.GetCallStatus(call, assignments))
                .Aggregate(
                    new int[Enum.GetValues(typeof(BO.CallInListStatus)).Length],
                    (arr, group) =>
                    {
                        arr[(int)group.Key] = group.Count();
                        return arr;
                    });

            return countOfCalls;
        }
        // in case of unexpected errors during processing
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }



    /// <summary>
    /// Updates the details of an existing call in the database:
    /// 1. check the call data using 
    /// 2. If the data is invalid, throws a BO.BlInvalidValueException.
    /// 3. If the data is valid, attempts to update the call in the database.
    /// </summary>
    /// <param name="c">The call object containing updated data.</param>
    /// <exception cref="BO.BlInvalidValueException">Thrown if the provided call data is invalid.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown for any other errors that occur during the update process.</exception>
    public void Update(Call c)
    {
        //stage 7
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7

        CallManager.Update(c);
    }



    /// <summary>
    /// Updates the assignment status to "Cancelled"
    /// 1. takes all assignments and volunteers from the database.
    /// 2. Finds the assignment by its ID  and checks if it exists.
    /// 3. Finds the volunteer by his ID  and checks if he exists.
    /// 4. Verifies if the requester has the right to cancel the assignment :manager or the volunteer assigned to the task
    /// 5. Checks if the assignment has already been closed or expired.
    /// 6. If valid, updates the assignment's `FinishTime` and `TypeOfEnd` to reflect the cancellation.
    /// 7. Saves the updated assignment back to the database.
    /// </summary>
    /// <param name="id">The volunteer ID of the requester.</param>
    /// <param name="assiId">The assignment ID to be cancelled.</param>
    public void UpdateCallCancelled(int id, int assiId)
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        CallManager.UpdateCallCancelled(id, assiId);
    }




    /// <summary>
    /// Updates the assignment status to "Finished"
    /// 1. takes all assignments and volunteers from the database.
    /// 2. Finds the assignment by its ID and checks if it exists.
    /// 3. Finds the volunteer by their ID and checks if they exist.
    /// 4. Verifies if the requester is the volunteer assigned to the assignment.
    /// 5. Checks if the assignment has already been closed or expired.
    /// 6. If valid, updates the assignment's `FinishTime` and `TypeOfEnd` to reflect that the assignment is fulfilled.
    /// 7. Saves the updated assignment back to the database.
    /// </summary>
    /// <param name="id">The volunteer ID of the requester.</param>
    /// <param name="assiId">The assignment ID to be marked as finished.</param>
    public void UpdateCallFinished(int id, int assiId, int callId)
    {
        Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        CallManager.UpdateCallFinished(id, assiId, callId);
    }

}
