using BO;

namespace BlApi;

public interface ICall
{
    int[] SumOfCalls();

    IEnumerable<BO.CallInList> GetListOfCalls(CallType? filterType=null, object?filterValue=null , CallType? sortType = null);

    IEnumerable<BO.Call> GetCallDetails(int callId);

    void Update( BO.Call c);

    void Delete(int callId);

    void Create (BO.Call c);

    BO.ClosedCallInList GetListOfClosedCall(int volId, BO.CallType? type = null);

    BO.OpenCallInList GetListOfOpenCall(int volId, BO.CallType? type = null, BO.OpenCallInList? openCall = null);

    void UpdateCallFinished (int volId, int callId);

    void UpdateCallCancelled(int id, int callId);

    void choiceOfCallToCare(int volId, int callId);

    //other function 
    BO.Call? Read(int callId);




}
