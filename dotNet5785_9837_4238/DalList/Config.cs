namespace Dal;
/// <summary>
/// configuration of the Running identifier (for call and assignment) , clok, riskrange and reset function
/// </summary>
internal static class Config
{
    internal const int StartCallId = 1;
    private static int nextCallId = StartCallId;
    internal static int NextCallId { get => nextCallId++; }


    internal const int StartAssignmentId = 0;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }


    internal static DateTime Clock { get; set; } = DateTime.Now ;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;


    /// <summary>
    /// reset puts all the values of the config to "0"
    /// </summary>
    internal static void Reset()
    {
        nextCallId = StartCallId; 
        nextAssignmentId = NextAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero ;
    }
}
