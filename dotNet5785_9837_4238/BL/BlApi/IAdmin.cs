namespace BlApi;

/// <summary>
/// Admin interface
/// functions : InitializeDB,ResetDB, GetMaxRange, SetMaxRange, GetClock, ForwardClock
/// </summary>
public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    TimeSpan GetMaxRange();
    void SetMaxRange(TimeSpan maxRange);
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);//uses the enum 

    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5
}
