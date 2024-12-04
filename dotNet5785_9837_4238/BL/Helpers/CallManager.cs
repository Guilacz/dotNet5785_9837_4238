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
