namespace Helpers;

using BO;
using DalApi;
using System.Xml.Linq;

/// <summary>
/// Class of all the help function that we will use in the Call Implementation
/// </summary>
internal class CallManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;
    internal static ObserverManager Observers = new();


    /// <summary>
    /// function to check the validity of a call: checks the maxtime and the address
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static bool CheckCall(BO.Call c)
    {
        if (CheckTime(c) == false)
            return false;
        if (!Tools.CheckAddressCall(c))
            return false;

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
      
   

    /// <summary>
    /// Static helper method to update all expired open calls.
    /// This method should be invoked from ClockManager whenever the system clock is updated.
    /// It closes all open calls whose maximum time has passed and their treatment hasn't been completed.
    /// </summary>
    internal static void PeriodicCallUpdates()
    {
        lock (AdminManager.BlMutex) ; //stage 7
        var allCalls = _dal.Call.ReadAll();

        foreach (var call in allCalls)
        {
            var CurrentCall = ConvertCallToBO(call, _dal);
            if (call.MaxTime != null && call.MaxTime.Value < AdminManager.Now)
            {
                if (CurrentCall.CallStatus != CallInListStatus.Closed)
                {
                    lock (AdminManager.BlMutex) ; //stage 7
                    var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == call.CallId).ToList();

                    if (assignments.Count == 0)
                    {
                        // No assignment exists, create a new one with "Expired Cancellation"
                        lock (AdminManager.BlMutex) ; //stage 7
                        var newAssignment = new DO.Assignment
                        {
                            CallId = call.CallId,
                            VolunteerId = null,
                            StartTime = AdminManager.Now,
                            FinishTime = AdminManager.Now,
                            TypeOfEnd = DO.TypeOfEnd.CancelledAfterTime
                        };
                        assignments.Add(newAssignment);
                        Observers.NotifyListUpdated(); 
                    }
                    else
                    {
                        // Update existing assignment with "Expired Cancellation"
                        var assignment = assignments.LastOrDefault(a => !a.FinishTime.HasValue);
                        if (assignment != null)
                        {
                            lock (AdminManager.BlMutex) ; //stage 7
                            var updatedAssignment = assignment with
                            {
                                FinishTime = AdminManager.Now,
                                TypeOfEnd = DO.TypeOfEnd.CancelledAfterTime
                            };
                            lock (AdminManager.BlMutex) ; //stage 7
                            _dal.Assignment.Update(updatedAssignment);
                            Observers.NotifyItemUpdated(call.CallId);
                        }
                    }
                    lock (AdminManager.BlMutex) ; //stage 7
                    _dal.Call.Update(call);
                    Observers.NotifyItemUpdated(call.CallId);
                }

            }

        }
    }
}
