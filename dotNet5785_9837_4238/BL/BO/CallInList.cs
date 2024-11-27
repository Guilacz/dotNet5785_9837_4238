namespace BO;

public class CallInList
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType;
    DateTime OpenTime { get; init; }
    public TimeSpan? TimeToEnd { get; set; }
    public string? LastName { get; set; }

    public TimeSpan? TimeToCare { get; set; }

    CallInListStatus CallInListStatus;

    public int numberOfAssignment { get; set; }
}
