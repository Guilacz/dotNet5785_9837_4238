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
    static readonly IEnumerable<BO.OpenCallInListSort> s_enums = (Enum.GetValues(typeof(BO.OpenCallInListSort)) as IEnumerable<BO.OpenCallInListSort>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CloseCallInListSortCollection : IEnumerable
{
    static readonly IEnumerable<BO.CloseCallInListSort> s_enums = (Enum.GetValues(typeof(BO.CloseCallInListSort)) as IEnumerable<BO.CloseCallInListSort>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}


internal class Enums
{


}
