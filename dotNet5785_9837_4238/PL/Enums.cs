using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;
internal class CallTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallType> s_enums = (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class RoleTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums = (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallInProgressStatus> s_enums = (Enum.GetValues(typeof(BO.CallInProgressStatus)) as IEnumerable<BO.CallInProgressStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class TypeOfEndCollection : IEnumerable
{
    static readonly IEnumerable<BO.TypeOfEnd> s_enums = (Enum.GetValues(typeof(BO.TypeOfEnd)) as IEnumerable<BO.TypeOfEnd>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallInListStatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallInListStatus> s_enums = (Enum.GetValues(typeof(BO.CallInListStatus)) as IEnumerable<BO.CallInListStatus>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class OpenCallInListSortCollection : IEnumerable
{
    static readonly IEnumerable<BO.CloseCallInListSort> s_enums = (Enum.GetValues(typeof(BO.CloseCallInListSort)) as IEnumerable<BO.CloseCallInListSort>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CloseCallInListSortCollection : IEnumerable
{
    static readonly IEnumerable<BO.CloseCallInListSort> s_enums = (Enum.GetValues(typeof(BO.CloseCallInListSort)) as IEnumerable<BO.CloseCallInListSort>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

class ConvertCallTypeToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var callType = (BO.CallType)value;

        switch (callType)
        {
            case BO.CallType.Math_Primary:
                return Brushes.Blue;
            case BO.CallType.Math_Middle:
                return Brushes.Cyan;
            case BO.CallType.Math_High:
                return Brushes.DarkBlue;
            case BO.CallType.English_Primary:
                return Brushes.LightGreen;
            case BO.CallType.English_Middle:
                return Brushes.Lime;
            case BO.CallType.English_High:
                return Brushes.Green;
            case BO.CallType.Grammary_Primary:
                return Brushes.Pink;
            case BO.CallType.Grammary_Middle:
                return Brushes.Purple;
            case BO.CallType.Grammary_High:
                return Brushes.Magenta;
            default:
                return Brushes.White; // default color if none match
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
internal class Enums
{


}
