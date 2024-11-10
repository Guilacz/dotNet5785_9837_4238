using DalApi;

namespace Dal;

public class ConfigImplementation : IConfig
{
    // public int NextCallId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
   // public int NextAssignmentId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public int nextCallId
    { get => Config.NextCallId; }

    public int nextAsignmentId => throw new NotImplementedException();

    public void Reset()
    {
        Config.Reset();
    }
}
