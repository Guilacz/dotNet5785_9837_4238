namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System;
using DalApi;


internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// function to Advance the clock by a specified time unit (minute, hour, day, month, or year).
    /// 1. Determines the new time based on the given `TimeUnit`.
    /// 2. Updates the system clock using `ClockManager.UpdateClock`.
    /// </summary>
    /// <param name="unit">The time unit to advance</param>
    public void ForwardClock(TimeUnit unit)
    {
        try
        {
            DateTime newTime;
            switch (unit)
            {
                case TimeUnit.minute:
                    newTime = ClockManager.Now.AddMinutes(1);
                    break;
                case TimeUnit.hour:
                    newTime = ClockManager.Now.AddHours(1);
                    break;
                case TimeUnit.day:
                    newTime = ClockManager.Now.AddDays(1);
                    break;
                case TimeUnit.month:
                    newTime = ClockManager.Now.AddMonths(1);
                    break;
                case TimeUnit.year:
                    newTime = ClockManager.Now.AddYears(1);
                    break;
                default:
                    throw new BO.BlInvalidValueException("Invalid time unit");
            }
            ClockManager.UpdateClock(newTime);
        }
        catch (DO.DalInvalidValueException ex)
        {
            throw new BO.BlInvalidValueException(ex.Message);
        }
    }


    /// <summary>
    /// function to return the time according to the clockManager
    /// </summary>
    /// <returns></returns>
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }


    /// <summary>
    /// function to return the value of the configuration variable "Risk Range".
    /// </summary>
    /// <returns></returns>
    public TimeSpan GetMaxRange()
    {
        IConfig config = _dal.Config;
        return config.RiskRange;
    }


    /// <summary>
    /// function to initialize the database, calls ResetDB and then initialize it
    /// </summary>
    public void InitializeDB()
    {
        try
        {
            ResetDB();
            DalTest.Initialization.Do();
        }
        // Thrown in case of unexpected errors during processing
        catch (Exception ex)
        {
            throw new BO.BlArgumentNullException(ex.Message);
        }
    }


    /// <summary>
    /// Function to reset the database : reset the data of the configurations, all the assignments, calls and volunteers
    /// </summary>
    public void ResetDB()
    {
        _dal.Config.Reset();
        var allAssignments = _dal.Assignment.ReadAll().ToList();
        _dal.Assignment.DeleteAll(); 
        var allCalls = _dal.Call.ReadAll().ToList();
        _dal.Call.DeleteAll(); 
        var allVolunteers = _dal.Volunteer.ReadAll().ToList();
        _dal.Volunteer.DeleteAll(); 
    }


    /// <summary>
    /// function to set the maxRange data
    /// </summary>
    /// <param name="maxRange"></param>
    public void SetMaxRange(TimeSpan maxRange)
    {
        IConfig config = _dal.Config;
        config.RiskRange = maxRange;
        Tools.RiskTime(config);
    }

}