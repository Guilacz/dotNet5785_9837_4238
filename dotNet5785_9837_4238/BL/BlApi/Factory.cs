namespace BlApi;

/// <summary>
///Factory Class
/// </summary>
public static class Factory
{
    public static IBl Get() => new BlImplementation.Bl();
}
