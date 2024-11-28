namespace BlApi;

/// <summary>
/// iadmin interface
/// </summary>
public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    int GetMaxRange();
    void SetMaxRange(int maxRange);
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);//uses the enum 
}
