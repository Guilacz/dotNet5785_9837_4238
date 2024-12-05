namespace Helpers;

using BO;
using DalApi;
using DO;

internal class CallManager
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// function to check the validity of a call: checks the maxtime and the address
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    internal static bool CheckCall(BO.Call c)
    {
        if(CheckTime(c) == false)
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
        if(c.MaxTime < c.OpenTime)
            return false;
        return true;
    }


    /// <summary>
    /// function to get the status of a call : in care, expired, closed, open, OpenAtRisk
    /// </summary>
    /// <param name="call"></param>
    /// <param name="assignments"></param>
    /// <returns></returns>
    public static BO.CallStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        var now = DateTime.Now;

        var activeAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime == null);
        if (activeAssignment != null)
        {
            return BO.CallStatus.InCare;
        }

        if (call.MaxTime.HasValue && now > call.MaxTime.Value)
        {
            return BO.CallStatus.Expired;
        }

        var finishedAssignment = assignments.FirstOrDefault(a => a.CallId == call.CallId && a.FinishTime.HasValue);
        if (finishedAssignment != null)
        {
            return BO.CallStatus.Closed;
        }

        if (call.MaxTime.HasValue && now > call.OpenTime.AddHours(1))
        {
            return BO.CallStatus.OpenAtRisk;
        }

        return BO.CallStatus.Open;
    }


    /// <summary>
    /// function to convert a DO call to a BO call
    /// </summary>
    /// <param name="call"></param>
    /// <param name="dal"></param>
    /// <returns></returns>
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

    internal static void PeriodicVolunteersUpdates()
    {
        var allCalls = _dal.Call.ReadAll().ToList();

        foreach (var call in allCalls)
        {
            var CurrentCall = ConvertDOToBO(call);
            if (call.MaxTimeToFinish.HasValue && call.MaxTimeToFinish.Value < ClockManager.Now)
            {
                if (CurrentCall.StatusCall != StatusCall.Close)
                {
                    var assignments = s_dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).ToList();

                    if (assignments.Count == 0)
                    {
                        // No assignment exists, create a new one with "Expired Cancellation"
                        var newAssignment = new DO.Assignment
                        {
                            CallId = call.Id,
                            VolunteerId = null,
                            EntryTime = ClockManager.Now,
                            EndTime = ClockManager.Now,
                            TypeEndTime = DO.TypeEndTime.CancellationExpired
                        };
                        assignments.Add(newAssignment);
                        //s_dal.Assignment.Add(newAssignment);
                    }
                    else
                    {
                        // Update existing assignment with "Expired Cancellation"
                        var assignment = assignments.LastOrDefault(a => !a.EndTime.HasValue);
                        if (assignment != null)
                        {

                            assignment.EndTime = ClockManager.Now;
                            assignment.TypeEndTime = DO.TypeEndTime.ExpiredCancellation;
                            s_dal.Assignment.Update(assignment);
                        }
                    }

                    // Update the call status to closed
                    // call.StatusCall = StatusCall.Closed;
                    s_dal.Call.Update(call);
                }

            }
