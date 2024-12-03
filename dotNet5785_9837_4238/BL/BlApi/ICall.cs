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
    IEnumerable<CallInList> GetListOfCalls(BO.CallType? filterType = null, object? filterValue = null, BO.CallInListSort? sortType = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
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
