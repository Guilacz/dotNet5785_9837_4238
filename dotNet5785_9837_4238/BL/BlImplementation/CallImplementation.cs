namespace BlImplementation;
using BlApi;
using Helpers;
using System.Collections.Generic;

internal class CallImplementation: ICall

{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void choiceOfCallToCare(int volId, int callId)
    {
        throw new NotImplementedException();
    }

    public void Create(Call c)
    {
        throw new NotImplementedException();
    }

    public void Delete(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Call> GetCallDetails(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> GetListOfCalls(CallType? filterType = null, object? filterValue = null, CallType? sortType = null)
    {
        throw new NotImplementedException();
    }

    public ClosedCallInList GetListOfClosedCall(int volId, CallType? type = null)
    {
        throw new NotImplementedException();
    }

    public OpenCallInList GetListOfOpenCall(int volId, CallType? type = null, OpenCallInList? openCall = null)
    {
        throw new NotImplementedException();
    }

    public Call? Read(int callId)
    {
        throw new NotImplementedException();
    }

    public int[] SumOfCalls()
    {
        throw new NotImplementedException();
    }

    public void Update(Call c)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallCancelled(int id, int callId)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallFinished(int volId, int callId)
    {
        throw new NotImplementedException();
    }
}
