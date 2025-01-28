namespace Helpers;

using BO;
using DalApi;
using System.Xml.Linq;

/// <summary>
/// Class of all the help function that we will use in the Call Implementation
/// </summary>
internal static class CallManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;
    internal static ObserverManager Observers = new();

    public static void Update(Call c)
    {
        //Helpers.AdminManager.ThrowOnSimulatorIsRunning(); //stage 7

        try
        {
            // נעדכן את כל השדות שאינם דורשים חישוב רשת
            if (!Helpers.CallManager.CheckCall(c))
            {
                throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");
            }

            lock (AdminManager.BlMutex) //stage 7
            {
                var updatedCall = new DO.Call
                {
                    CallId = c.CallId,
                    CallType = (DO.CallType)c.CallType,
                    Address = c.Address,
                    Latitude = c.Latitude ?? 0, // ערך זמני אם לא מחושב עדיין
                    Longitude = c.Longitude ?? 0, // ערך זמני אם לא מחושב עדיין
                    OpenTime = c.OpenTime,
                    MaxTime = c.MaxTime,
                    Details = c.Details,
                };

                // עדכון ב-DAL ללא שדות Latitude ו-Longitude
                _dal.Call.Update(updatedCall);

                // נעדכן את התצפיתנים
               
            }

            CallManager.Observers.NotifyItemUpdated(c.CallId);
            CallManager.Observers.NotifyListUpdated();

            // הפעלה של המתודה C (אסינכרונית)
            UpdateCoordinatesAsyncCall(c);
        }
        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException($"Invalid call data: {ex.Message}");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }
    private static  async Task UpdateCoordinatesAsyncCall(Call c)
    {
        try
        {
            // חישוב ערכי Latitude ו-Longitude באמצעות מתודה אסינכרונית
            var coordinates = await Helpers.Tools.GetAddressCoordinatesAsync(c.Address);
            c.Latitude = coordinates.Latitude;
            c.Longitude = coordinates.Longitude;
            if (!await Helpers.Tools.CheckAddressCall(c))// בדיקת תקינות כתובת
                throw new BO.BlInvalidValueException("The call data provided is invalid. Please check the input and try again.");

            // עדכון מחדש של הקריאה ב-DAL עם כל הערכים
            lock (AdminManager.BlMutex) //stage 7
            {
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

                _dal.Call.Update(updatedCall);
            }

            // עדכון תצפיתנים מחדש
            CallManager.Observers.NotifyItemUpdated(c.CallId);
            CallManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidValueException($"Invalid call data: {ex.Message}");

        }
    }

    public static Call? Read(int callId)
    {
        try
        {
            DO.Call? call;
            lock (AdminManager.BlMutex)  //stage 7
                call = _dal.Call.Read(callId);

            if (call == null)
            {
                throw new BO.BlDoesNotExistException($"Call with ID {callId} was not found.");
            }

            lock (AdminManager.BlMutex)  //stage 7

                return Helpers.CallManager.ConvertCallToBO(call, _dal);

        }
        // in case of unexpected errors during processing
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException(ex.Message);
        }
    }












    /// <summary>
    /// function to check the validity of a call: checks the maxtime and the address
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static bool CheckCall(BO.Call c)
    {
        if (CheckTime(c) == false)
            return false;
        //if (!Tools.CheckAddressCall(c))
        //    return false;

        return true;
    }

    /// <summary>
    /// function to check if the maxTime of a call is valid
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static bool CheckTime(BO.Call c)
    {
        if (c.MaxTime < c.OpenTime)
            return false;
        return true;
    }

    //internal static BO.CallInListStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    //{
    //    var now = DateTime.Now;

    //    // מקבל את כל ההקצאות של הקריאה הנוכחית, ממוין לפי זמן התחלה מהחדש לישן
    //    var callAssignments = assignments
    //        .Where(a => a.CallId == call.CallId)
    //        .OrderByDescending(a => a.StartTime);

    //    var lastAssignment = callAssignments.FirstOrDefault();

    //    // אם אין הקצאות בכלל
    //    if (lastAssignment == null)
    //    {
    //        // אם עבר יותר משעה מפתיחת הקריאה
    //        if (now > call.OpenTime.AddHours(1))
    //            return BO.CallInListStatus.OpenAtRisk;
    //        return BO.CallInListStatus.Open;
    //    }

    //    // אם ההקצאה האחרונה פתוחה (בטיפול)
    //    if (lastAssignment.FinishTime == null && !lastAssignment.TypeOfEnd.HasValue)
    //    {
    //        // אם עבר יותר משעה מתחילת הטיפול
    //        if (now > lastAssignment.StartTime.AddHours(1))
    //            return BO.CallInListStatus.InCareAtRisk;
    //        return BO.CallInListStatus.InCare;
    //    }

    //    // אם הקריאה הסתיימה
    //    if (lastAssignment.TypeOfEnd.HasValue && lastAssignment.TypeOfEnd.Value.Equals(TypeOfEnd.Fulfilled))
    //        return BO.CallInListStatus.Closed;


    //    //// אם הקריאה בוטלה או פג תוקפה
    //    //if (lastAssignment.TypeOfEnd.HasValue)
    //    //    return BO.CallInListStatus.Expired;
    //    if (lastAssignment.TypeOfEnd.HasValue)
    //    {
    //        // אם הזמן המקסימלי עבר
    //        if (DateTime.Now> call.MaxTime)
    //            return BO.CallInListStatus.Expired;
    //    }
    //    // מקרה ברירת מחדל - פתוח
    //    return BO.CallInListStatus.Open;
    //}
    internal static BO.CallInListStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var now = DateTime.Now;

        if (assignments.Any(a =>
     a.CallId == call.CallId &&
     a.TypeOfEnd.HasValue && // בדיקה אם יש ערך
     (BO.TypeOfEnd)a.TypeOfEnd == BO.TypeOfEnd.Fulfilled))

        {
            return BO.CallInListStatus.Closed;
        }


        lock (AdminManager.BlMutex)  //stage 7
        {
            // מקבל את כל ההקצאות של הקריאה הנוכחית, ממוין לפי זמן התחלה מהחדש לישן
            var callAssignments = assignments
            .Where(a => a.CallId == call.CallId)
            .OrderByDescending(a => a.StartTime);

            var lastAssignment = callAssignments.FirstOrDefault();

            // אם אין הקצאות בכלל
            if (lastAssignment == null)
            {
                // אם עבר יותר משעה מפתיחת הקריאה
                if (now > call.OpenTime.AddHours(1))
                    return BO.CallInListStatus.OpenAtRisk;
                return BO.CallInListStatus.Open;
            }

            // אם ההקצאה האחרונה פתוחה (בטיפול)
            if (lastAssignment.FinishTime == null && !lastAssignment.TypeOfEnd.HasValue)
            {
                // אם עבר יותר משעה מתחילת הטיפול
                if (now > lastAssignment.StartTime.AddHours(1))
                    return BO.CallInListStatus.InCareAtRisk;
                return BO.CallInListStatus.InCare;
            }

            // אם הקריאה הסתיימה
            if (lastAssignment.TypeOfEnd.HasValue && lastAssignment.TypeOfEnd.Value.Equals(TypeOfEnd.Fulfilled))
                return BO.CallInListStatus.Closed;


            //// אם הקריאה בוטלה או פג תוקפה
            //if (lastAssignment.TypeOfEnd.HasValue)
            //    return BO.CallInListStatus.Expired;
            if (lastAssignment.TypeOfEnd.HasValue)
            {
                // אם הזמן המקסימלי עבר
                if (DateTime.Now > call.MaxTime)
                    return BO.CallInListStatus.Expired;
            }


            // מקרה ברירת מחדל - פתוח
            return BO.CallInListStatus.Open;

        }
    }
    /// <summary>
    /// function to get the status of a call : in care, expired, closed, open, OpenAtRisk
    /// </summary>
    /// <param name="call"></param>
    /// <param name="assignments"></param>
    /// <returns></returns>
    //internal static BO.CallInListStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    //{
    //    var now = DateTime.Now;

    //    var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime == null);
    //    if (activeAssignment != null)
    //    {
    //        return BO.CallInListStatus.InCare;
    //    }

    //    if (call.MaxTime.HasValue && now > call.MaxTime.Value)
    //    {
    //        return BO.CallInListStatus.Expired;
    //    }

    //    var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime.HasValue);
    //    if (finishedAssignment != null)
    //    {
    //        return BO.CallInListStatus.Closed;
    //    }

    //    if (call.MaxTime.HasValue && now > call.OpenTime.AddHours(1))
    //    {
    //        return BO.CallInListStatus.OpenAtRisk;
    //    }

    //    return BO.CallInListStatus.Open;
    //}

    //internal static BO.CallInListStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    //{
    //    var now = DateTime.Now;

    //    var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime == null);

    //    // תנאי לקריאה שהיא גם בטיפול וגם בסיכון
    //    if (activeAssignment != null && call.MaxTime.HasValue && now > call.OpenTime.AddHours(1))
    //    {
    //        return BO.CallInListStatus.InCareAtRisk;
    //    }

    //    if (activeAssignment != null)
    //    {
    //        return BO.CallInListStatus.InCare;
    //    }

    //    if (call.MaxTime.HasValue && now > call.MaxTime.Value)
    //    {
    //        return BO.CallInListStatus.Expired;
    //    }

    //    var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime.HasValue);
    //    if (finishedAssignment != null)
    //    {
    //        return BO.CallInListStatus.Closed;
    //    }

    //    if (call.MaxTime.HasValue && now > call.OpenTime.AddHours(1))
    //    {
    //        return BO.CallInListStatus.OpenAtRisk;
    //    }

    //    return BO.CallInListStatus.Open;
    //}

    /// <summary>
    /// function to convert a DO call to a BO call
    /// </summary>
    /// <param name="call"></param>
    /// <param name="dal"></param>
    /// <returns></returns>

    internal static BO.Call ConvertCallToBO(DO.Call call, IDal dal)
        {
        return new BO.Call
        {
            CallId = call.CallId,
            CallType = (BO.CallType)call.CallType,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxTime = call.MaxTime,
            Details = call.Details,
            CallStatus = Helpers.CallManager.GetCallStatus(call, dal.Assignment.ReadAll()),
            callAssignInLists = dal.Assignment.ReadAll()
        .Where(a => a.CallId == call.CallId)
        .Select(a =>
        {
            var volunteer = dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.VolunteerId == a.VolunteerId);

            return new BO.CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                Name = volunteer?.Name, 
                StartTime = a.StartTime,
                TypeOfEnd = 0,
                FinishTime = a.FinishTime
            };
        }).ToList()
        };
    }


    public static void UpdateCallCancelled(int id, int assiId)
    {

        try
        {

            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)  //stage 7
                assignments = _dal.Assignment.ReadAll();

            IEnumerable<DO.Volunteer> volunteers;
            lock (AdminManager.BlMutex)  //stage 7
                volunteers = _dal.Volunteer.ReadAll();


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
            lock (AdminManager.BlMutex)  //stage 7
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


    public static void UpdateCallFinished(int id, int assiId, int callId)
    {

        try
        {

            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)  //stage 7
                assignments = _dal.Assignment.ReadAll();

            IEnumerable<DO.Volunteer> volunteers;
            lock (AdminManager.BlMutex)  //stage 7
                volunteers = _dal.Volunteer.ReadAll();

            var call = Read(callId);

            DO.Call? call2;
            lock (AdminManager.BlMutex)  //stage 7
                call2 = _dal.Call.Read(callId);

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
            lock (AdminManager.BlMutex)  //stage 7
                _dal.Assignment.Update(assignment);

            call.CallStatus = Helpers.CallManager.GetCallStatus(call2, assignments);

            if (call != null)
            {
                //   call.CallStatus = CallInListStatus.Closed; 
                Update(call);
            }

            CallManager.Observers.NotifyItemUpdated(callId);
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

    public static void ChoiceOfCallToCare(int volId, int callId)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                DO.Call? call = _dal.Call.ReadAll().FirstOrDefault(c => c.CallId == callId);
                if (call == null)
                    throw new BO.BlDoesNotExistException($"Call with ID {callId} does not exist or could not be found in the database.");

                DO.Volunteer? volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.VolunteerId == volId);
                if (volunteer == null)
                    throw new BO.BlDoesNotExistException($"Volunteer with ID {volId} does not exist. ");

                IEnumerable<DO.Assignment> assignments = _dal.Assignment.ReadAll();
                bool isCallAlreadyHandled = assignments.Any(a => a.CallId == callId && a.TypeOfEnd == DO.TypeOfEnd.Fulfilled);
                if (isCallAlreadyHandled)
                    throw new BO.BlInvalidValueException($"Call with ID {callId} has already been handled and cannot be reassigned.");

                var status = Helpers.CallManager.GetCallStatus(call, assignments);
                if (status == BO.CallInListStatus.InCare || status == BO.CallInListStatus.InCareAtRisk)
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
            CallManager.Observers.NotifyItemUpdated(callId);
            CallManager.Observers.NotifyListUpdated();
            // Async coordinate and address validation
            //UpdateCallDetailsAsync(callId);
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

    public static IEnumerable<BO.OpenCallInList> GetListOfOpenCall(int volId, BO.CallType? type = null, OpenCallInListSort? openCall = null)
    {
        try
        {
            IEnumerable<DO.Call> calls;
            lock (AdminManager.BlMutex)  //stage 7
                calls = _dal.Call.ReadAll();

            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)  //stage 7
                assignments = _dal.Assignment.ReadAll();

            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex)  //stage 7
                volunteer = _dal.Volunteer.Read(volId);

            string volunteerAddress = Helpers.VolunteerManager.ConvertVolToBO(volunteer).Address;

            calls = calls.Where(call =>
            {
                var status = Helpers.CallManager.GetCallStatus(call, assignments);
                return status == BO.CallInListStatus.Open || status == BO.CallInListStatus.OpenAtRisk;
            });

            if (type.HasValue && type != BO.CallType.None)
            {
                calls = calls.Where(c => c.CallType == (DO.CallType)type);
            }

            var openCalls = new List<BO.OpenCallInList>();

            foreach (var call in calls)
            {
                var distance = Helpers.Tools.CalculateDistanceBetweenAddresses(volunteer, call).GetAwaiter().GetResult();
                openCalls.Add(new BO.OpenCallInList
                {
                    CallId = call.CallId,
                    CallType = (BO.CallType)call.CallType,
                    Address = call.Address,
                    OpenTime = call.OpenTime,
                    MaxTime = call.MaxTime,
                    Details = call.Details,
                    Distance = distance
                });
            }




            //var openCalls = from call in calls
            //                select new BO.OpenCallInList
            //                {
            //                    CallId = call.CallId,
            //                    CallType = (BO.CallType)call.CallType,
            //                    Address = call.Address,
            //                    OpenTime = call.OpenTime,
            //                    MaxTime = call.MaxTime,
            //                    Details = call.Details,
            //                    Distance = distance
            //                }; 

            var result = openCall switch
            {
                OpenCallInListSort.CallId => openCalls.OrderBy(c => c.CallId).ToList(),
                OpenCallInListSort.CallType => openCalls.OrderBy(c => c.CallType).ToList(),
                OpenCallInListSort.Address => openCalls.OrderBy(c => c.Address).ToList(),
                OpenCallInListSort.OpenTime => openCalls.OrderBy(c => c.OpenTime).ToList(),
                OpenCallInListSort.MaxTime => openCalls.OrderBy(c => c.MaxTime).ToList(),
                OpenCallInListSort.Details => openCalls.OrderBy(c => c.Details).ToList(),
                _ => openCalls
            };

            return result;
        }
        catch (Exception ex)
        {
            throw new BO.BlDoesNotExistException($"Error in GetListOfOpenCall: {ex.Message}");
        }
    }

    /// <summary>
    /// Static helper method to update all expired open calls.
    /// This method should be invoked from ClockManager whenever the system clock is updated.
    /// It closes all open calls whose maximum time has passed and their treatment hasn't been completed.
    /// </summary>
    internal static void PeriodicCallUpdates()
    {
        
        IEnumerable<DO.Call> allCalls;
        lock (AdminManager.BlMutex)  //stage 7
            allCalls = _dal.Call.ReadAll();

        foreach (var call in allCalls)
        {
            var CurrentCall = ConvertCallToBO(call, _dal);
            if (call.MaxTime != null && call.MaxTime.Value < AdminManager.Now)
            {
                if (CurrentCall.CallStatus != CallInListStatus.Closed)
                {

                    List<DO.Assignment> assignments;
                    lock (AdminManager.BlMutex)  //stage 7
                        assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == call.CallId).ToList();

                    
                    if (assignments.Count == 0)
                    {
                        // No assignment exists, create a new one with "Expired Cancellation"


                        DO.Assignment newAssignment;
                        lock (AdminManager.BlMutex)  //stage 7

                        {
                            newAssignment = new DO.Assignment
                            {
                                CallId = call.CallId,
                                VolunteerId = null,
                                StartTime = AdminManager.Now,
                                FinishTime = AdminManager.Now,
                                TypeOfEnd = DO.TypeOfEnd.CancelledAfterTime
                            };
                            assignments.Add(newAssignment);
                        }
                        Observers.NotifyListUpdated(); 
                    }
                    else
                    {
                        // Update existing assignment with "Expired Cancellation"
                        var assignment = assignments.LastOrDefault(a => !a.FinishTime.HasValue);
                        if (assignment != null)
                        {
                            lock (AdminManager.BlMutex)  //stage 7
                            {
                                var updatedAssignment = assignment with
                                {
                                    FinishTime = AdminManager.Now,
                                    TypeOfEnd = DO.TypeOfEnd.CancelledAfterTime
                                };
                                //stage 7
                                _dal.Assignment.Update(updatedAssignment);
                            }
                            Observers.NotifyItemUpdated(call.CallId);
                        }
                    }
                    lock (AdminManager.BlMutex)  //stage 7
                    _dal.Call.Update(call);
                    Observers.NotifyItemUpdated(call.CallId);
                }

            }

        }
    }
}
