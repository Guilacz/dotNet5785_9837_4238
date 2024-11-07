namespace DO;

/// <summary>
/// details of the file Assignment
/// </summary>
/// <param name="Id">id of the assignment</param>
/// <param name="CallId">id of the call</param>
/// <param name="VolunteerId">id of the volunteer</param>
/// <param name="TypeOfEnd">type of the end of the demand</param>
/// <param name="StartTime">when the call was taken in charge</param>
/// <param name="FinishTime">when the call was fulfilled/cancelled</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    TypeOfEnd TypeOfEnd,
    DateTime StartTime,
    DateTime? FinishTime = null
    
)
{
    /// <summary>
    /// default constructor of the assignment
    /// </summary>
    public Assignment () : this (0,0,0,0,DateTime.Now) { }
}


