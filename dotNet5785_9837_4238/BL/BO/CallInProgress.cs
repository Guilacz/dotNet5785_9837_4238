namespace BO;

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType;

    public string Adress { get; set; }

    DateTime OpenTime { get; init; }
    DateTime StartTime { get; set; }
    DateTime? MaxTime { get; set; }

    public string? Details { get; set; }
    public double distance { get; init; }
    CallInProgressStatus CallInProgressStatus;


}
