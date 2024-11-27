namespace Dal;
using DalApi;


sealed internal class DalList : IDal
{
    /// <summary>
    /// DalList is type Singleton
    /// </summary>
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void resetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
