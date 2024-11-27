namespace BO
{
    public class ClosedCallInList
    {
    public int CallId { get; init; }
    CallType CallType;
    public string Adress { get; set; }
    DateTime OpenTime { get; init; }
    DateTime StartTime { get; set; }
    TypeOfEnd? TypeOfEnd;
    DateTime? FinishTime { get; init; }
    }
}
