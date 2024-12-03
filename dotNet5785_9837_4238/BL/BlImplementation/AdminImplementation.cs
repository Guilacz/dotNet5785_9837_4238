namespace BlImplementation;
using Dal;
using BlApi;
using BO;
using Helpers;
using System;
using DalApi;


internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void ForwardClock(TimeUnit unit)
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
                throw new ArgumentException("Invalid time unit");
        }
        ClockManager.UpdateClock(newTime);

    }

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetMaxRange()
    {
        IConfig config = _dal.Config;
        return config.RiskRange;
    }



    public void InitializeDB()
    {
        try
        {
            ResetDB();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

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

    public void SetMaxRange(TimeSpan maxRange)
    {
        IConfig config = _dal.Config;
        config.RiskRange = maxRange;
        Tools.RiskTime(config);
    }

}