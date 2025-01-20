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
        try
        {
            DO.Call? call = _dal.Call.ReadAll().FirstOrDefault(c => c.CallId == callId);
            if (call == null)
                throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist or could not be found in the database.");

            DO.Volunteer? volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.VolunteerId == volId);
            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} does not exist. ");

            var coordinates = Helpers.Tools.GetAddressCoordinates(call.Address);

            call = call with { Latitude = coordinates.Item1, Longitude = coordinates.Item2 };

            BO.Call boCall = Helpers.CallManager.ConvertCallToBO(call, _dal);

            if (!Helpers.CallManager.CheckCall(boCall))
                throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");


            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            bool isCallAlreadyHandled = assignments.Any(a => a.CallId == callId && a.FinishTime != null);
            if (isCallAlreadyHandled)
                throw new BO.BlInvalidValueException($"Call with ID {callId} has already been handled and cannot be reassigned.");

            bool isCallInProcess = assignments.Any(a => a.CallId == callId && a.FinishTime == null);
            if (isCallInProcess)
                throw new BO.BlInvalidValueException($"Call with ID {callId} is currently being handled by another volunteer.");

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
    /// Creates a new call :
    /// 1.check that the input data is valid
    /// 2.Converts the bo call to do call
    /// 3.Saves the new call to the do database
    /// </summary>
    /// <param name="c">The bo object representing the call to create.</param>
    /// <exception cref="BO.BlInvalidValueException"></exception>
    /// <exception cref="BO.BlAlreadyExistException"></exception>

    public void Create(BO.Call c)
    {
        if (c.Latitude == null || c.Longitude == null)
        {
            var coordinates = Helpers.Tools.GetAddressCoordinates(c.Address);
            //c = c with { Latitude = coordinates.Latitude, Longitude = coordinates.Longitude };
            c.Latitude = coordinates.Latitude;
            c.Longitude = coordinates.Longitude;

        }
        if (!Helpers.CallManager.CheckCall(c))
        {
            throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");
        }

        if (c.Latitude == null || c.Longitude == null)
        {
            throw new BO.BlInvalidValueException("Latitude and Longitude must not be null.");
        }

        try
        {
            //_dal.Call.Create(new DO.Call
            //{
            //    CallId = c.CallId,
            //    CallType = (DO.CallType)c.CallType,
            //    Address = c.Address,
            //    Latitude = c.Latitude.Value, 
            //    Longitude = c.Longitude.Value, 
            // // OpenTime = c.OpenTime,
            //    MaxTime = c.MaxTime,
            //    OpenTime = c.OpenTime,

            //    Details = c.Details,

            //});

            var newCall = new DO.Call
            {
                CallId = c.CallId,
                CallType = (DO.CallType)c.CallType,
                Address = c.Address,
                Latitude = c.Latitude.Value,
                Longitude = c.Longitude.Value,
                MaxTime = c.MaxTime,
                //OpenTime = c.OpenTime,
                OpenTime = DateTime.Now,
                Details = c.Details,
            };

            // הוספה ל-DAL
            _dal.Call.Create(newCall);

            // המרה ל-BO
            var convertedCall = Helpers.CallManager.ConvertCallToBO(newCall, _dal);
            CallManager.Observers.NotifyListUpdated();
            _dal.Volunteer.ReadAll()
                .Where(vol =>
                    !string.IsNullOrEmpty(vol.Email) &&
                    !string.IsNullOrEmpty(vol.Address) &&
                    Helpers.Tools.CalculateDistanceBetweenAddresses(vol.Address, c.Address) <= vol.Distance)
                .ToList()
                .ForEach(vol => Helpers.Tools.SendEmail(
                    toAddress: vol.Email,
                    subject: "new call in your area",
                    body: $"Hello {vol.Name},\n\nThere is a new call in your area at the address: {c.Address}.\nPlease check and respond if you are available to assist.\n\nThank you!"
                ));

        }



        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }
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
        try
        {
            BO.Call? c = Read(callId);

            if (c == null)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {callId} was not found.");
            }

            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            if (c.CallStatus != 0 ||
                (c.callAssignInLists != null && c.callAssignInLists.Any(x => assignments.Any(a => a.VolunteerId == x.VolunteerId))))
            {
                throw new BO.BlDeletionImpossible("Can't delete this call.");
            }

            //if (!Helpers.CallManager.CheckCall(c))
            //{
            //    throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");
            //}
            var volunteer = _dal.Volunteer.ReadAll()
                .Select(vol => Helpers.VolunteerManager.ConvertVolToBO(vol)) 
                .FirstOrDefault(vol => vol.callInCaring.CallId == callId); 

            // אם המתנדב נמצא, שלח לו מייל
            if (volunteer != null && !string.IsNullOrEmpty(volunteer.Email))
            {
                Helpers.Tools.SendEmail(
                    toAddress: volunteer.Email,
                    subject: "Your call has been canceled",
                    body: $"Hello {volunteer.Name},\n\nThe call with ID #{callId} that you were handling has been canceled.\nPlease contact us if you have any questions.\n\nThank you!"
                );
            }

            _dal.Call.Delete(callId);
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
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();


            if (calls == null || !calls.Any())
            {
                return Enumerable.Empty<BO.CallInList>();
            }

            if (assignments == null)
            {
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

                // אם מצאנו הקצאה אחרונה, נחפש את המתנדב שקשור לה
                var lastVolunteerName = lastAssignment != null
                    ? _dal.Volunteer.ReadAll().FirstOrDefault(v => v.VolunteerId == lastAssignment.VolunteerId)?.Name
                    : null;

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

    //public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListSort? filterType = null, object? filterValue = null, BO.CallInListSort? sortType = null)
    //{
    //    try
    //    {
    //        IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
    //        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
    //        IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();


    //        if (calls == null || !calls.Any())
    //        {
    //            return Enumerable.Empty<BO.CallInList>();
    //        }

    //        if (assignments == null)
    //        {
    //            assignments = Enumerable.Empty<DO.Assignment>();
    //        }

    //        if (filterType != null && filterValue != null)
    //        {
    //            calls = filterType switch
    //            {
    //                BO.CallInListSort.CallId =>
    //                    int.TryParse(filterValue.ToString(), out int filterId) ?
    //                    calls.Where(c => c.CallId == filterId) : calls,

    //                BO.CallInListSort.CallType =>
    //                    Enum.TryParse(typeof(DO.CallType), filterValue.ToString(), out var callType) ?
    //                    calls.Where(c => c.CallType == (DO.CallType)callType) : calls,

    //                BO.CallInListSort.OpenTime =>
    //                    filterValue is DateTime filterDate ?
    //                    calls.Where(c => c.OpenTime.Date == filterDate.Date) : calls,

    //                BO.CallInListSort.LastName =>
    //                    filterValue is string volunteerName ?
    //                    calls.Where(c =>
    //                        assignments.Any(a => a.CallId == c.CallId &&
    //                                             volunteers.Any(v => v.VolunteerId == a.VolunteerId &&
    //                                                                 v.Name.Contains(volunteerName, StringComparison.OrdinalIgnoreCase)))) :
    //                    calls,

    //                BO.CallInListSort.CallInListStatus =>
    //                    Enum.TryParse(typeof(BO.CallInListStatus), filterValue.ToString(), out var status) ?
    //                    calls.Where(c =>
    //                    {
    //                        // קבלת סטטוס הקריאה לפי ההמרה
    //                        var boCall = Helpers.CallManager.ConvertCallToBO(c, _dal);
    //                        var isAtRisk = boCall.CallStatus == BO.CallStatus.Open && boCall.MaxTime.HasValue && boCall.MaxTime.Value < DateTime.Now;
    //                        var convertedStatus = ConvertCallStatusToCallInListStatus(boCall.CallStatus, isAtRisk);
    //                        return convertedStatus == (BO.CallInListStatus)status;
    //                    }) :
    //                    calls,

    //                _ => calls
    //            };
    //        }





    //        var callInList = calls.Select(c => new BO.CallInList
    //        {
    //            CallId = c.CallId,
    //            CallType = (BO.CallType)c.CallType,
    //            OpenTime = c.OpenTime,
    //            LastName = null,
    //            TimeToEnd = c.MaxTime.HasValue ? c.MaxTime.Value.Subtract(c.OpenTime) : (TimeSpan?)null,
    //            TimeToCare = c.MaxTime.HasValue ? c.MaxTime.Value.Subtract(DateTime.Now) : (TimeSpan?)null,
    //            CallInListStatus = (BO.CallInListStatus)Helpers.CallManager.GetCallStatus(c, assignments)
    //        }).ToList();

    //        if (sortType != null)
    //        {
    //            callInList = sortType switch
    //            {
    //                BO.CallInListSort.CallType => callInList.OrderBy(c => c.CallType).ToList(),
    //                BO.CallInListSort.OpenTime => callInList.OrderBy(c => c.OpenTime).ToList(),
    //                BO.CallInListSort.TimeToCare => callInList.OrderBy(c => c.TimeToCare ?? TimeSpan.MaxValue).ToList(),
    //                BO.CallInListSort.CallInListStatus => callInList.OrderBy(c => c.CallInListStatus).ToList(),
    //                BO.CallInListSort.LastName => callInList.OrderBy(c => c.LastName).ToList(),
    //                _ => callInList.OrderBy(c => c.CallId).ToList()
    //            };
    //        }
    //        else
    //        {
    //            callInList = callInList.OrderBy(c => c.CallId).ToList();
    //        }

    //        return callInList;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlDoesNotExistException(ex.Message);
    //    }
    //}
    //public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListSort? filterType = null, object? filterValue = null, BO.CallInListSort? sortType = null)
    //{
    //    try
    //    {
    //        IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
    //        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

    //        if (calls == null || !calls.Any())
    //        {
    //            return Enumerable.Empty<BO.CallInList>();
    //        }

    //        if (assignments == null)
    //        {
    //            assignments = Enumerable.Empty<DO.Assignment>();
    //        }

    //        if (filterType != null && filterValue != null)
    //        {
    //            calls = filterType switch
    //            {
    //                BO.CallInListSort.CallId when int.TryParse(filterValue.ToString(), out int filterId)
    //                    => calls.Where(c => c.CallId == filterId),
    //                BO.CallInListSort.CallType when Enum.TryParse(typeof(DO.CallType), filterValue.ToString(), out var callType)
    //                    => calls.Where(c => c.CallType == (DO.CallType)callType),
    //                BO.CallInListSort.OpenTime when filterValue is DateTime filterDate
    //                    => calls.Where(c => c.OpenTime.Date == filterDate.Date),
    //                BO.CallInListSort.TimeToEnd when filterValue is DateTime endDate
    //                    => calls.Where(c => c.MaxTime.HasValue && c.MaxTime.Value.Date == endDate.Date),
    //                //BO.CallInListSort.TimeToCare when filterValue is DateTime endDate
    //                //    => calls.Where(c => c.MaxTime.HasValue && c.MaxTime.Value.Date == endDate.Date),
    //                _ => calls
    //            };
    //        }

    //        var callInList = calls.Select(c => new BO.CallInList
    //        {
    //            CallId = c.CallId,
    //            CallType = (BO.CallType)c.CallType,
    //            OpenTime = c.OpenTime,
    //            LastName = null, 
    //            TimeToEnd = c.MaxTime.HasValue ? c.MaxTime.Value.Subtract(c.OpenTime) : (TimeSpan?)null,
    //            TimeToCare = c.MaxTime.HasValue ? c.MaxTime.Value.Subtract(DateTime.Now) : (TimeSpan?)null,
    //            CallInListStatus = (BO.CallInListStatus)Helpers.CallManager.GetCallStatus(c, assignments)
    //        }).ToList();

    //        if (sortType != null)
    //        {
    //            callInList = sortType switch
    //            {
    //                //BO.CallInListSort.Id => callInList.OrderBy(c => c.CallId).ToList(),
    //                BO.CallInListSort.CallType => callInList.OrderBy(c => c.CallType).ToList(),
    //                BO.CallInListSort.OpenTime => callInList.OrderBy(c => c.OpenTime).ToList(),
    //                BO.CallInListSort.TimeToEnd => callInList.OrderBy(c => c.TimeToEnd ?? TimeSpan.MaxValue).ToList(),
    //                BO.CallInListSort.TimeToCare => callInList.OrderBy(c => c.TimeToCare ?? TimeSpan.MaxValue).ToList(),
    //                BO.CallInListSort.CallStatus => callInList.OrderBy(c => c.CallInListStatus).ToList(),
    //                _ => callInList.OrderBy(c => c.CallId).ToList()
    //            };
    //        }
    //        else
    //        {
    //            callInList = callInList.OrderBy(c => c.CallId).ToList();
    //        }

    //        return callInList;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlDoesNotExistException(ex.Message);
    //    }
    //}




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

    //public IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? sort = null)
    //{
    //    try
    //    {
    //        var assignments = _dal.Assignment.ReadAll().Where(a => a.VolunteerId == volId).ToList();
    //        var calls = _dal.Call.ReadAll().ToList();

    //        var assignmentWithCalls = from a in assignments
    //                                  let relatedCall = calls.FirstOrDefault(c => c.CallId == a.CallId)
    //                                  where relatedCall != null
    //                                  select new
    //                                  {
    //                                      Assignment = a,
    //                                      RelatedCall = relatedCall
    //                                  };

    //        if (type != null)
    //        {
    //            assignmentWithCalls = from ac in assignmentWithCalls
    //                                  where ac.RelatedCall.CallType == (DO.CallType)type
    //                                  select ac;
    //        }


    //        var closedCallInList = from ac in assignmentWithCalls
    //                               select new BO.ClosedCallInList
    //                               {
    //                                   CallId = ac.Assignment.CallId,
    //                                   CallType = (BO.CallType)ac.RelatedCall.CallType,
    //                                   Adress = ac.RelatedCall.Address,
    //                                   OpenTime = ac.RelatedCall.OpenTime,
    //                                   StartTime = ac.Assignment.StartTime,
    //                                   TypeOfEnd = ac.Assignment.TypeOfEnd.HasValue ? (BO.TypeOfEnd?)ac.Assignment.TypeOfEnd : null,
    //                                   FinishTime = ac.Assignment.FinishTime.HasValue ? ac.Assignment.FinishTime : null
    //                               };


    //        closedCallInList = sort switch
    //        {
    //            BO.CloseCallInListSort.CallId => closedCallInList.OrderBy(v => v.CallId),
    //            BO.CloseCallInListSort.CallType => closedCallInList.OrderBy(v => v.CallType),
    //            BO.CloseCallInListSort.TypeOfEnd => closedCallInList.OrderBy(v => v.TypeOfEnd),
    //            _ => closedCallInList.OrderBy(v => v.CallId)
    //        };

    //        return closedCallInList.ToList();
    //    }
    //    // in case of unexpected errors during processing
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlDoesNotExistException(ex.Message);
    //    }
    //}

    public IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? sort = null)
    {
        // Vérifier si le volontaire existe
        var volunteer = _dal.Volunteer.Read(volId)
                       ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} does not exist.");

        // Récupérer les affectations pour ce volontaire
        var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.VolunteerId);
        if (assignments == null || !assignments.Any())
            return Enumerable.Empty<BO.ClosedCallInList>();

        // Récupérer les appels fermés
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

        // Trier selon le critère spécifié
        return sort switch
        {
            BO.CloseCallInListSort.CallId => closedCallsOfVolunteer.OrderBy(c => c.CallId),
            BO.CloseCallInListSort.CallType => closedCallsOfVolunteer.OrderBy(c => c.CallType),
            BO.CloseCallInListSort.TypeOfEnd => closedCallsOfVolunteer.OrderBy(c => c.TypeOfEnd),
            _ => closedCallsOfVolunteer
        };
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
    //public IEnumerable<BO.OpenCallInList> GetListOfOpenCall(int volId, BO.CallType? type = null, OpenCallInListSort? openCall = null)
    //{
    //    try
    //    {
    //        IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
    //        IEnumerable<DO.Volunteer> volun = _dal.Volunteer.ReadAll();
    //        IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

    //        DO.Volunteer volunteer = _dal.Volunteer.Read(volId);

    //        BO.Volunteer boVolunteer = Helpers.VolunteerManager.ConvertVolToBO(volunteer);

    //        string volunteerAddress = boVolunteer.Address;

    //        calls = calls.Where(call =>
    //        {
    //            var status = Helpers.CallManager.GetCallStatus(call, assignments);
    //            return status == BO.CallInListStatus.Open || status == BO.CallInListStatus.OpenAtRisk ;
    //        });


    //        if (type.HasValue)
    //        {
    //            calls = calls.Where(c => c.CallType == (DO.CallType)type);
    //        }

    //        var openCalls = calls.Select(c => new BO.OpenCallInList
    //        {
    //            CallId = c.CallId,
    //            CallType = (BO.CallType)c.CallType,
    //            Address = c.Address,
    //            OpenTime = c.OpenTime,
    //            MaxTime = c.MaxTime,
    //            Details = c.Details,
    //            Distance = Helpers.Tools.CalculateDistanceBetweenAddresses(volunteerAddress, c.Address)
    //        }).ToList();

    //        var result = openCall switch
    //        {
    //            OpenCallInListSort.CallId => openCalls.OrderBy(c => c.CallId).ToList(),
    //            OpenCallInListSort.CallType => openCalls.OrderBy(c => c.CallType).ToList(),
    //            OpenCallInListSort.Address => openCalls.OrderBy(c => c.Address).ToList(),
    //            OpenCallInListSort.OpenTime => openCalls.OrderBy(c => c.OpenTime).ToList(),
    //            OpenCallInListSort.MaxTime => openCalls.OrderBy(c => c.MaxTime).ToList(),
    //            OpenCallInListSort.Details => openCalls.OrderBy(c => c.Details).ToList(),
    //            _ => openCalls.OrderBy(c => c.CallId).ToList() 
    //        };

    //        return result;
    //    }
    //    // in case of unexpected errors during processing
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlDoesNotExistException(ex.Message);
    //    }
    //}

    public IEnumerable<BO.OpenCallInList> GetListOfOpenCall(int volId, BO.CallType? type = null, OpenCallInListSort? openCall = null)
    {
        try
        {
            // Lire toutes les données nécessaires
            IEnumerable<DO.Call> calls = _dal.Call.ReadAll();
            IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();

            // Obtenir l'adresse du volontaire
            DO.Volunteer volunteer = _dal.Volunteer.Read(volId);
            string volunteerAddress = Helpers.VolunteerManager.ConvertVolToBO(volunteer).Address;

            // Filtrer les appels ouverts ou à risque
            calls = calls.Where(call =>
            {
                var status = Helpers.CallManager.GetCallStatus(call, assignments);
                return status == BO.CallInListStatus.Open || status == BO.CallInListStatus.OpenAtRisk;
            });

            // Filtrage par type d'appel
            if (type.HasValue && type != BO.CallType.None)
            {
                calls = calls.Where(c => c.CallType == (DO.CallType)type);
            }

            // Transformer les appels en BO
            var openCalls = calls.Select(c => new BO.OpenCallInList
            {
                CallId = c.CallId,
                CallType = (BO.CallType)c.CallType,
                Address = c.Address,
                OpenTime = c.OpenTime,
                MaxTime = c.MaxTime,
                Details = c.Details,
                Distance = Helpers.Tools.CalculateDistanceBetweenAddresses(volunteerAddress, c.Address)
            }).ToList();

            // Appliquer le tri
            var result = openCall switch
            {
                OpenCallInListSort.CallId => openCalls.OrderBy(c => c.CallId).ToList(),
                OpenCallInListSort.CallType => openCalls.OrderBy(c => c.CallType).ToList(),
                OpenCallInListSort.Address => openCalls.OrderBy(c => c.Address).ToList(),
                OpenCallInListSort.OpenTime => openCalls.OrderBy(c => c.OpenTime).ToList(),
                OpenCallInListSort.MaxTime => openCalls.OrderBy(c => c.MaxTime).ToList(),
                OpenCallInListSort.Details => openCalls.OrderBy(c => c.Details).ToList(),
                _ => openCalls // Aucun tri si None
            };

            return result;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException($"Error in GetListOfOpenCall: {ex.Message}");
        }
    }


    /// <summary>
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
    public Call? Read(int callId)
    {
        try
        {
            DO.Call call = _dal.Call.Read(callId);

            if (call == null)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {callId} was not found.");
            }
         
            return Helpers.CallManager.ConvertCallToBO(call, _dal);

        }
        // in case of unexpected errors during processing
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
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
            var calls = _dal.Call.ReadAll();
            var assignments = _dal.Assignment.ReadAll();

            int[] countOfCalls = calls
                .GroupBy(call => Helpers.CallManager.GetCallStatus(call, assignments))
                .Aggregate(
                    new int[Enum.GetValues(typeof(BO.CallStatus)).Length],
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
        try
        {
            var coordinates = Helpers.Tools.GetAddressCoordinates(c.Address);
            double latitude = coordinates.Latitude;
            double longitude = coordinates.Longitude;
            c.Latitude = latitude;
            c.Longitude = longitude;
            if (!Helpers.CallManager.CheckCall(c))
            {
                throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");
            }

            if (c.Latitude == null || c.Longitude == null)
            {
                throw new BO.BlInvalidValueException("Latitude and Longitude must not be null.");
            }

            try
            {
                //_dal.Call.Update(new DO.Call
                //{
                //    CallId = c.CallId,
                //    CallType = (DO.CallType)c.CallType,
                //    Address = c.Address,
                //    Latitude = c.Latitude.Value, 
                //    Longitude = c.Longitude.Value, 
                //    OpenTime = c.OpenTime,
                //    MaxTime = c.MaxTime,
                //    Details = c.Details,
                //});
                var updatedCall = new DO.Call
                {
                    CallId = c.CallId,
                    CallType = (DO.CallType)c.CallType,
                    Address = c.Address,
                    Latitude = c.Latitude.Value,
                    Longitude = c.Longitude.Value,
                    OpenTime = c.OpenTime,
                    MaxTime = c.MaxTime,
                    Details = c.Details,
                };

                // עדכון ב-DAL
                _dal.Call.Update(updatedCall);

                // המרה ל-BO
                var convertedCall = Helpers.CallManager.ConvertCallToBO(updatedCall, _dal);
                CallManager.Observers.NotifyItemUpdated(c.CallId);
                CallManager.Observers.NotifyListUpdated(); 


            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException(ex.Message);
            }
        }
        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException($"Invalid call data: {ex.Message}");
        }
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
        try
        {
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            var assignment = assignments.FirstOrDefault(a => a.Id == assiId);
            if (assignment == null)
            {
                throw new BO.BlDoesNotExistException($"Assignment with ID {assiId} does not exist.");
            }

            var requester = volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (requester == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.");
            }

            bool isManager = volunteers.Any(v => v.VolunteerId == id && v.RoleType == DO.Role.Manager);
            bool isAssignedToVolunteer = assignment.VolunteerId == id;

            if (!isManager && !isAssignedToVolunteer)
            {
                throw new BO.BlInvalidValueException("Requester does not have permission to cancel this assignment.");
            }

            if (assignment.FinishTime != null)
            {
                throw new BO.BlInvalidValueException("Assignment has already been closed or expired.");
            }

            assignment = assignment with
            {
                // FinishTime = DateTime.Now,
                FinishTime = null,

                TypeOfEnd = (DO.TypeOfEnd)((id == requester.VolunteerId)
                    ? DO.TypeOfEnd.CancelledByVolunteer
                    : DO.TypeOfEnd.CancelledByManager)
            };

            _dal.Assignment.Update(assignment);
            CallManager.Observers.NotifyItemUpdated(assiId);
            CallManager.Observers.NotifyListUpdated();
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
    public void UpdateCallFinished(int id, int assiId)
    {
        try
        {
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            var assignment = assignments.FirstOrDefault(a => a.Id == assiId);
            if (assignment == null)
            {
                throw new BO.BlDoesNotExistException($"Assignment with ID {assiId} does not exist.");
            }

            var requester = volunteers.FirstOrDefault(v => v.VolunteerId == id);
            if (requester == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with ID {id} does not exist.");
            }

            bool isAssignedToVolunteer = assignment.VolunteerId == id;

            if (!isAssignedToVolunteer)
            {
                throw new BO.BlInvalidValueException("Requester does not have permission to finish this assignment.");
            }

            if (assignment.FinishTime != null)
            {
                throw new DO.DalInvalidValueException("Assignment has already been closed or expired.");
            }

            assignment = assignment with
            {
                FinishTime = DateTime.Now,
                TypeOfEnd = (DO.TypeOfEnd)DO.TypeOfEnd.Fulfilled
            };

            _dal.Assignment.Update(assignment);
            CallManager.Observers.NotifyItemUpdated(assiId);
            CallManager.Observers.NotifyListUpdated();
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

}
