namespace BO;

/// <summary>
/// enum of the different lessons possible from call, callInList,CallInProgress
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
    Grammary_High
}

/// <summary>
/// status from call
/// </summary>
public enum CallStatus
{
    Open,
    InCare,
    Closed,
    Expired,
    OpenAtRisk
}

/// <summary>
/// enum of the possible types of end of the call from CallAssignInList
/// </summary>
public enum TypeOfEnd
{
    Fulfilled,
    CancelledByVolunteer,
    CancelledByManager,
    CancelledAfterTime
}


/// <summary>
/// status from callInlist
/// </summary>
public enum CallInListStatus
{
    Open,
    InCare,
    Closed,
    Expired,
    OpenAtRisk,
    InCareAtRisk
}

/// <summary>
/// starus from CallInProgress
/// </summary>
public enum CallInProgressStatus
{
    InCare,
    InCareAtRisk
}




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
public enum DistanceType
{
    AirDistance,
    WalkDistance,
    CarDistance
}

public enum TimeUnit
{
    minute,
    hour,
    day,
    month,
    year
}

public enum VolunteerSortField
{
    VolunteerId,
    Name,
    IsActive,
    SumOfCaredCall,
    SumOfCancelledCall,
    SumOfCallExpired,
    CallId,
    RoleType,
    DistanceType
}
