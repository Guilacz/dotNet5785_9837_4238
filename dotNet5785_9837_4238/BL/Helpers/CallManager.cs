namespace Helpers;

using BO;
using DalApi;
using DO;

internal class CallManager
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    internal static bool CheckCall(BO.Call c)
    {
        if(CheckTime(c) == false)
            return false;
        if (!Tools.CheckAddressCall(c))
        {
            return false;
        }
        return true;
    }
    internal static bool CheckTime(BO.Call c)
    {
        if(c.MaxTime < c.OpenTime)
            return false;
        return true;
    }

    /*
    public static BO.CallStatus GetCallStatus(int callId)
    {
        // Validate call ID
        if (callId <= 0)
        {
            throw new ArgumentException("Invalid Call ID provided.");
        }

        // Retrieve the call from the DAL
        DO.Call doCall;
        try
        {
            doCall = DalFactory.Instance.GetCall(callId);
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Call with ID {callId} does not exist.");
        }

        // Convert DO.Call to BO.Call
        BO.Call boCall = new BO.Call
        {
            CallId = doCall.CallId,
            CallType = doCall.CallType,
            Address = doCall.Address,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            OpenTime = doCall.OpenTime,
            MaxTime = doCall.MaxTime,
            CallStatus = BO.CallStatus.Open, // Default, will be recalculated
            Details = doCall.Details
        };

        // Use the current date and time
        DateTime currentTime = DateTime.Now;

        // Determine the call status based on OpenTime and MaxTime
        if (boCall.MaxTime.HasValue && boCall.MaxTime.Value <= currentTime)
        {
            return BO.CallStatus.Expired; // Call has exceeded its maximum time
        }
        else if (currentTime >= boCall.OpenTime &&
                 (!boCall.MaxTime.HasValue || currentTime <= boCall.MaxTime.Value))
        {
            return BO.CallStatus.InCare; // Call is currently active within its allowed time
        }
        else if (currentTime < boCall.OpenTime)
        {
            return BO.CallStatus.Open; // Call is scheduled to start in the future
        }
        else
        {
            return BO.CallStatus.OpenAtRisk; // Call is in an undefined or risky state
        }
    }
    */

    /* public static CallInListStatus CalculateCallStatus(BO.Call call, IDal dal)
     {
         DateTime currentTime = ClockManager.Now; // הזמן הנוכחי
         TimeSpan riskRange = dal.Config.RiskRange; // טווח סיכון מתוך משתני התצורה

         // אם הקריאה עברה את הזמן המקסימלי שלה
         if (call.MaxTime.HasValue && currentTime > call.MaxTime.Value)
             return CallInListStatus.Expired;

         // קריאה בטיפול כרגע
         if (call.CallStatus == CallStatus.InCare)
         {
             // בדיקה אם הקריאה בטיפול בסיכון
             if (call.MaxTime.HasValue && (call.MaxTime.Value - currentTime) <= riskRange)
                 return CallInListStatus.InCareAtRisk;

             // קריאה בטיפול רגיל
             return CallInListStatus.InCare;
         }

         // קריאה פתוחה בסיכון
         if (call.MaxTime.HasValue && (call.MaxTime.Value - currentTime) <= riskRange)
             return CallInListStatus.OpenAtRisk;

         // קריאה פתוחה רגילה
         return CallInListStatus.Open;
     }
     */

    public static BO.CallStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var now = DateTime.Now;

        // בדיקה אם הקריאה בטיפול פעיל
        var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime == null);
        if (activeAssignment != null)
        {
            return BO.CallStatus.InCare;
        }

        // בדיקה אם הקריאה פגה
        if (call.MaxTime.HasValue && now > call.MaxTime.Value)
        {
            return BO.CallStatus.Expired;
        }

        // בדיקה אם הקריאה כבר טופלה
        var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime.HasValue);
        if (finishedAssignment != null)
        {
            return BO.CallStatus.Closed;
        }

        // בדיקה אם הקריאה פתוחה אך בסיכון (למשל זמן עבר מזמן פתיחה אבל עדיין פתוחה)
        if (call.MaxTime.HasValue && now > call.OpenTime.AddHours(1))
        {
            return BO.CallStatus.OpenAtRisk;
        }

        // הקריאה פשוט פתוחה
        return BO.CallStatus.Open;
    }
    public static BO.Call ConvertCallToBO(DO.Call call, IDal dal)
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
            CallStatus = Helpers.CallManager.GetCallStatus(call, dal.Assignment.ReadAll())
        };
    }



}
