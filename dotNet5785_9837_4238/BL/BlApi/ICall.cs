namespace BlApi;

using BO;

/// <summary>
/// Call Interface
/// functions : GetListOfCalls, GetCallDetails, Update, Delete, Create, GetListOfClosedCall, GetListOfOpenCall,
/// UpdateCallFinished, UpdateCallCancelled, ChoiceOfCallToCare, Read
/// array : SumOfCalls
/// </summary>
public interface ICall : IObservable
{
    int[] SumOfCalls();

 
    IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListSort? filterType = null, object? filterValue = null, BO.CallInListSort? sortType = null);

    Call GetCallDetails(int callId);

    void Update(Call c);

    void Delete(int callId);

    void Create(Call c);

    IEnumerable<BO.ClosedCallInList> GetListOfClosedCall(int volId, BO.CallType? type = null, BO.CloseCallInListSort? end = null);

    IEnumerable<BO.OpenCallInList> GetListOfOpenCall(int volId, BO.CallType? type = null, OpenCallInListSort? openCall = null);

    void UpdateCallFinished(int volId, int callId);

    void UpdateCallCancelled(int id, int assiId);

    void ChoiceOfCallToCare(int volId, int callId);

    Call? Read(int callId);
}
