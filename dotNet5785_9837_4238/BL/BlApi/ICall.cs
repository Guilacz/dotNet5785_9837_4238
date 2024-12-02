namespace BlApi;

using BO;


public interface ICall
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    int[] SumOfCalls();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filterType"></param>
    /// <param name="filterValue"></param>
    /// <param name="sortType"></param>
    /// <returns></returns>
    IEnumerable<CallInList> GetListOfCalls(CallType? filterType = null, object? filterValue = null, CallType? sortType = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    Call GetCallDetails(int callId);

    void Update(Call c);

    void Delete(int callId);

    void Create(Call c);

    ClosedCallInList GetListOfClosedCall(int volId, CallType? type = null);

    OpenCallInList GetListOfOpenCall(int volId, CallType? type = null, OpenCallInList? openCall = null);

    void UpdateCallFinished(int volId, int callId);

    void UpdateCallCancelled(int id, int assiId);

    void ChoiceOfCallToCare(int volId, int callId);

    Call? Read(int callId);
}
