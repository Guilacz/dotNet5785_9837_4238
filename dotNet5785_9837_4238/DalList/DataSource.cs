namespace Dal;

/// <summary>
/// Provides data collections of Volunteers, Calls, and Assignments.
/// </summary>
internal static class DataSource
{
    internal static List<DO.Volunteer> Volunteers { get; } = new();
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();
}
