using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// configuration of the Running identifier (for call and assignment) , clok, riskrange and reset function
/// </summary>
/// 



internal static class Config
{

    internal const int StartCallId = 1;
    private static int nextCallId = StartCallId;


    internal static int NextCallId 
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextCallId++;
    }


    internal const int StartAssignmentId = 0;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId 
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => nextAssignmentId++;
    }


    internal static DateTime Clock 
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set;
    } = DateTime.Now;

    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get;
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set; 
    } = TimeSpan.Zero;


    /// <summary>
    /// reset puts all the values of the config to "0"
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    internal static void Reset()
    {
        nextCallId = StartCallId;
        nextAssignmentId = NextAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
    }
}
