namespace Dal; 
using DalApi;
using DO;
using System.Collections.Generic;


/// <summary>
///Implement the properties and methods defined in the `IAssignment` interface to manage configurations related to the "Assignment" entity.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// function create new item :
    /// checks if the item already exists in the list, if not, create it
    /// </summary>

    public void Create(Assignment item)
    {
        int currentId = Config.NextAssignmentId; // Get the next unique Id for the Assignment
        Assignment currentItem = new Assignment(currentId, item.CallId, item.VolunteerId, item.StartTime, item.TypeOfEnd, item.FinishTime);
        DataSource.Assignments.Add(currentItem); // Add the new Assignment to the data source
    }


    /// <summary>
    /// delete function : delete if the item is in the list
    /// </summary>

    public void Delete(int id)
    {
        Assignment? AssignmentToDelete = Read(id);

        if (AssignmentToDelete == null)
            throw new DalDeletionImpossible($"A assignment with ID {id} does not exist.");

        else
            DataSource.Assignments.Remove(AssignmentToDelete);
    }

    /// <summary>
    /// function deleteAll: clears all the list
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }


    /// <summary>
    /// function read: checks and return the wanted element
    /// </summary>

    public Assignment? Read(int Id)
    {
        return DataSource.Assignments.FirstOrDefault(assignment => assignment.Id == Id);
    }

    /// <summary>
    /// new function Read which goes according to a delegate function
    /// </summary>

    /// <exception cref="NotImplementedException"></exception>
    public Assignment? Read(Func<Assignment, bool>? filter)
    {
        return DataSource.Assignments?.FirstOrDefault(filter);
    }




    /// <summary>
    /// function readall: returns the items
    /// </summary>
    public IEnumerable<Assignment> ReadAll(Func <Assignment, bool>? filter = null)
    {
        if (filter!= null)
        {
            return from item in DataSource.Assignments
                   where filter(item)
                   select item;
        }
        return from item in DataSource.Assignments
               select item;
    }


    /// <summary>
    /// update function : checks if the element exists, delete it, create it with new details
    /// </summary>

    public void Update(Assignment item)
    {
        Assignment? AssignmentToUpdate = Read(item.Id);

        if (AssignmentToUpdate == null)
            throw new DalDoesNotExistException($"A assignment with ID {item.Id} does not exist.");

        else
        {
            Delete(item.Id);
            Create(item);
        }

    }


    /// <summary>
    /// toString : takes Assignment object as input and creates a textual representation of this object
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public string ToString(Assignment item)
    {

        var details = new System.Text.StringBuilder();
        details.AppendLine("Assignment Details:");
        details.AppendLine($"ID: {item.Id}");
        details.AppendLine($"Call ID: {item.CallId}");
        details.AppendLine($"Volunteer ID: {item.VolunteerId}");
        details.AppendLine($"Entry Time: {item.StartTime}");
        details.AppendLine($"End Time: {item.TypeOfEnd}");
        details.AppendLine($"Type of End Time: {item.FinishTime}");

        return details.ToString();

    }
}



