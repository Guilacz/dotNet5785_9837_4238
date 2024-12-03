

namespace BO;
[Serializable]

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
}

[Serializable]

public class BlAlreadyExistException : Exception
{
    public BlAlreadyExistException(string? message) : base(message) { }
}

[Serializable]

public class BlDeletionImpossible : Exception
{
    public BlDeletionImpossible(string? message) : base(message) { }
}


/// <summary>
/// we added another exception for all the times we try to get to a not valid value
/// </summary>
[Serializable]

public class BlInvalidValueException : Exception
{
    public BlInvalidValueException(string? message) : base(message) { }
}

/// <summary>
/// we added another exception when the object cannot be null
/// </summary>
[Serializable]

public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }
}





