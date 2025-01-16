namespace DO;



/// <summary>
/// enum of the roles , from Volunteer
/// </summary>
public enum Role
{
    Manager,
    Volunteer
}

/// <summary>
/// enum of the types of distance , from Volunteer
/// </summary>
public enum Distance
{
    AirDistance,
    WalkDistance,
    CarDistance
}


/// <summary>
/// enum of the possible types of end of the call, from Assignment
/// </summary>
public enum TypeOfEnd
{
    Fulfilled,
    CancelledByVolunteer,
    CancelledByManager,
    CancelledAfterTime
}



/// <summary>
/// enum of the different lessons possible, from Call
/// </summary>
public enum CallType
{
    Math_Primary,
    Math_Middle,
    Math_High,
    English_Primary,
    English_Middle,
    English_High,
    Grammary_Primary,
    Grammary_Middle,
    Grammary_High,
    None
}
