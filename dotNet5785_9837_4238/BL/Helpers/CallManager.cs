namespace Helpers;
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




}
