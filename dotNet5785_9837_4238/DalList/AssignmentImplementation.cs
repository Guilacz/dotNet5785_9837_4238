namespace Dal; 
using DalApi;
using DO;
using System.Collections.Generic;


/// <summary>
///Implement the properties and methods defined in the `IAssignment` interface to manage configurations related to the "Assignment" entity.
/// </summary>
public class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// function create new item :
    /// checks if the item already exists in the list, if not, create it
    /// </summary>

    /*public void Create(Assignment item)
    {
        if (Read(item.Id) != null)
            throw new ArgumentException($"A call with ID {item.CallId} already exists.");

        else
            DataSource.Assignments.Add(item);
    }*/
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
            throw new ArgumentException($"A assignment with ID {id} does not exist.");

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
    /// function read: checks and return the wanted element from the list
    /// </summary>

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(Assign => Assign.Id == id);
    }


    /// <summary>
    /// function readall: returns the list
    /// </summary>
    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }


    /// <summary>
    /// update function : checks if the element exists, delete it, create it with new details
    /// </summary>

    public void Update(Assignment item)
    {
        Assignment? AssignmentToUpdate = Read(item.Id);

        if (AssignmentToUpdate == null)
            throw new ArgumentException($"A assignment with ID {item.Id} does not exist.");

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



