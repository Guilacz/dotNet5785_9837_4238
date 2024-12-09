namespace Helpers;

using BO;
using DalApi;
using System.Xml.Linq;

internal class CallManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;


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


    /// <summary>
    /// function to get the status of a call : in care, expired, closed, open, OpenAtRisk
    /// </summary>
    /// <param name="call"></param>
    /// <param name="assignments"></param>
    /// <returns></returns>
    internal static BO.CallStatus GetCallStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
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
            CallStatus = Helpers.CallManager.GetCallStatus(call, dal.Assignment.ReadAll())
        };
    }

    /// <summary>
    /// Static helper method to update all expired open calls.
    /// This method should be invoked from ClockManager whenever the system clock is updated.
    /// It closes all open calls whose maximum time has passed and their treatment hasn't been completed.
    /// </summary>
    internal static void PeriodicVolunteersUpdates()
    {

        var allCalls = _dal.Call.ReadAll();

        foreach (var call in allCalls)
        {
            var CurrentCall = ConvertCallToBO(call, _dal);
            if (call.MaxTime != null && call.MaxTime.Value < ClockManager.Now)
            {
                if (CurrentCall.CallStatus != CallStatus.Closed)
                {
                    var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == call.CallId).ToList();

                    if (assignments.Count == 0)
                    {
                        // No assignment exists, create a new one with "Expired Cancellation"
                        var newAssignment = new DO.Assignment
                        {
                            CallId = call.CallId,
                            VolunteerId = null,
                            StartTime = ClockManager.Now,
                            FinishTime = ClockManager.Now,
                            TypeOfEnd = DO.TypeOfEnd.CancelledAfterTime
                        };
                        assignments.Add(newAssignment);

                        //s_dal.Assignment.Add(newAssignment);
                    }
                    else
                    {
                        // Update existing assignment with "Expired Cancellation"
                        var assignment = assignments.LastOrDefault(a => !a.FinishTime.HasValue);
                        if (assignment != null)
                        {
                            var updatedAssignment = assignment with
                            {
                                FinishTime = ClockManager.Now,
                                TypeOfEnd = DO.TypeOfEnd.CancelledAfterTime
                            };

                            _dal.Assignment.Update(updatedAssignment);
                        }
                    }


                    // Update the call status to closed
                    // call.StatusCall = StatusCall.Closed;
                    _dal.Call.Update(call);
                }

            }

        }
    }
}
