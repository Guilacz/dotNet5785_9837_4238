namespace DalApi;

public interface IConfig
{
    //int NextCallId { get; set; }
    //int NextAssignmentId { get; set; }
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
    void Reset();
}
