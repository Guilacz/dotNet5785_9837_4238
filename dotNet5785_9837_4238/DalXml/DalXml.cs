using DalApi;
using System.Diagnostics;
namespace Dal;

sealed internal class DalXml : IDal
{
    /// <summary>
    /// DalXml is singleton type
    /// </summary>
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }
    public IVolunteer Volunteer {  get; } = new VolunteerImplementation();

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
