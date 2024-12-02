namespace BlImplementation;
using Dal;
using BlApi;
using BO;
using Helpers;
using System;


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

    public int GetMaxRange()
    {

        return Config.RiskRange; 
    }


    public void InitializeDB()
    {
        throw new NotImplementedException();
    }

    public void ResetDB()
    {
        throw new NotImplementedException();
    }

    public void SetMaxRange(int maxRange)
    {
        throw new NotImplementedException();
    }
}
