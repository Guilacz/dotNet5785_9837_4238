namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    private const string AssignmentsXmlFile = Config.s_assignments_xml;

    [MethodImpl(MethodImplOptions.Synchronized)]
    private XElement createAssignmentElement(Assignment assignment)
    {

        return new XElement("Assignment",
            new XElement("Id", assignment.Id),
            new XElement("CallId", assignment.CallId),
            new XElement("VolunteerId", assignment.VolunteerId),
            new XElement("StartTime", assignment.StartTime.ToString("o")), 
            new XElement("TypeOfEnd", assignment.TypeOfEnd?.ToString()),
            new XElement("FinishTime", assignment.FinishTime?.ToString("o"))
        );
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    static Assignment getAssignment(XElement a)
    {
        return new DO.Assignment()
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("can't convert id " + a.ToString()),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("can't convert callid " + a.ToString()),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("can't convert id " + a.ToString()),
            StartTime = a.ToDateTimeNullable("StartTime") ?? throw new FormatException("can't convert StartTime " + a.ToString()),
            TypeOfEnd = a.ToEnumNullable<TypeOfEnd>("TypeOfEnd"),
            FinishTime = a.ToDateTimeNullable("FinishTime")
        };
    }
    /*  public void Create(Assignment item)
      {
          XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(AssignmentsXmlFile);

          if (assignmentRootElem.Elements()
              .Any(assign => (int?)assign.Element("Id") == item.Id))
              throw new DalAlreadyExistException($"Assignment with ID={item.Id} already exists");

          XElement newAssignment = createAssignmentElement(item);
          assignmentRootElem.Add(newAssignment);

          XMLTools.SaveListToXMLElement(assignmentRootElem, AssignmentsXmlFile);
      }

      */
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        // אם ID הוא 0, נדרש לקבל מזהה חדש מ-Config
        if (item.Id == 0)
            item = item.WithId(Config.NextAssignmentId);

        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(AssignmentsXmlFile);

        if (assignmentRootElem.Elements()
            .Any(assign => (int?)assign.Element("Id") == item.Id))
            throw new DalAlreadyExistException($"Assignment with ID={item.Id} already exists");

        XElement newAssignment = createAssignmentElement(item);
        assignmentRootElem.Add(newAssignment);

        XMLTools.SaveListToXMLElement(assignmentRootElem, AssignmentsXmlFile);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(AssignmentsXmlFile);

        XElement? assignmentToDelete = assignmentRootElem.Elements()
            .FirstOrDefault(assign => (int?)assign.Element("Id") == id);

        if (assignmentToDelete == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        assignmentToDelete.Remove();
        XMLTools.SaveListToXMLElement(assignmentRootElem, AssignmentsXmlFile);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XElement assignmentRootElem = new XElement("Assignments"); 
        XMLTools.SaveListToXMLElement(assignmentRootElem, AssignmentsXmlFile);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().FirstOrDefault(assign =>(int?)assign.Element("Id") == id);
        return assignmentElem is null ? null : getAssignment(assignmentElem);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool>? filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(assign =>
        getAssignment(assign)).FirstOrDefault(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {

            if (filter == null)
            {
                return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(assign => getAssignment(assign));
            }
            else
            {
                return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(assign => getAssignment(assign)).Where(filter);
            }
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    public string ToString(Assignment a)
    {
        return $"Assignment Details:\n" +
          $"ID: {a.Id}\n" +
          $"Call ID: {a.CallId}\n" +
          $"Volunteer ID: {a.VolunteerId}\n" +
          $"Start Time: {a.StartTime}\n" +
          $"Type Of End: {a.TypeOfEnd}\n" +
          $"Finish Time: {a.FinishTime?.ToString() ?? "Not Finished"}";
    }

    //public void Update(Assignment item)
    //{
    //    XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
    //    (assignmentRootElem.Elements().FirstOrDefault(assign => (int?)assign.Element("Id") == item.Id)
    //    ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
    //    .Remove();
    //    assignmentRootElem.Add(new XElement("Assignment", createAssignmentElement(item)));
    //    XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    //}
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        (assignmentRootElem.Elements().FirstOrDefault(assign => (int?)assign.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
        .Remove();
        // מוסיף את האלמנט החדש בלי קינון כפול
        assignmentRootElem.Add(createAssignmentElement(item));
        XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
    }
}
