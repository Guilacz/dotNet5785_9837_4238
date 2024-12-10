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
}
