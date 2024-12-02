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
        throw new NotImplementedException();
    }

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetMaxRange()
    {

        var riskRange = 
    }


    public void InitializeDB()
    {
        throw new NotImplementedException();
    }

    public void ResetDB()
    {
        throw new NotImplementedException();
    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        IConfig config = new DalApi.IConfig { RiskRange = maxRange };
        Tools.RiskTime(config);
    }
}