using System.Text;

namespace Helpers;
/// <summary>
/// class for the help methods
/// </summary>

internal static class Tools
{
    /// <summary>
    /// explanation to the function : for all elements in T, we take its value, and add it with the name of the property
    /// and apply the basic toString function on our element wich is type stringbuilder
    /// </summary>

    public static string ToStringProperty<T>(this T t)
    {
        StringBuilder sb = new StringBuilder();

        // for all elements in T
        foreach (var property in typeof(T).GetProperties())
        {
            // take its value
            object value = property.GetValue(t, null);

            // add the name of the property and its value together
            sb.AppendLine($"{property.Name}: {value}");
        }
        return sb.ToString();
    }

    /// <summary>
    /// function to check if a value is int
    /// </summary>
    public static bool CheckInt(object value)
    {
        return value is int;
    }


    /// <summary>
    /// function to check if a value is double
    /// </summary>
    public static bool CheckDouble(object value)
    {
        return value is double;
    }


    //check address



}
