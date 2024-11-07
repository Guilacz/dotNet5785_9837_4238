namespace Dal;
internal static class Config
{
    internal const int StartCallId = 1;
    private static int nextCallId = 2;
    internal static int NextCallId { get => nextCallId++; }


    internal const int StartAssignmentId = 1;
    private static int nextAssignmentId = 2;
    internal static int NextAssignmentId { get => nextAssignmentId++; }


    internal static DateTime Clock { get; set; } = DateTime.Now ;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    internal static void Reset()
    {
        nextCallId = StartCallId; 
        nextAssignmentId = NextAssignmentId;
        Clock = DateTime.Now ;
        RiskRange = TimeSpan.Zero ;
    }
}
