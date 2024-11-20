namespace DO;
[Serializable]

public class DalDoesNotExistException: Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}

public class DalAlreadyExistException : Exception
{
    public DalAlreadyExistException(string? message) : base(message) { }
}

public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}


/// <summary>
/// we added another exception for all the times we try to get to a not valid value
/// </summary>
public class DalInvalidValueException : Exception
{
    public DalInvalidValueException(string? message) : base(message) { }
}

/// <summary>
/// we added another exception when the object cannot be null
/// </summary>
public class DalArgumentNullException : Exception
{
    public DalArgumentNullException(string? message) : base(message) { }
}



/// <summary>
/// new exception from xml tools
/// </summary>
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}




