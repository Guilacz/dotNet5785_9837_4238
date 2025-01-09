using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
internal class Enums
{


}
