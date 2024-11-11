namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    /// <summary>
    /// function create new item :
    /// checks if the item already exists in the list, if not, create it
    /// </summary>

    public void Create(Call item)
    {
        if (Read(item.CallId) != null)
            throw new ArgumentException($"A call with ID {item.CallId} already exists.");

        else
            DataSource.Calls.Add(item);
    }


    /// <summary>
    /// delete function : delete if the item is in the list
    /// </summary>

    public void Delete(int id)
    {
        Call? CallToDelete = Read(id);

        if (CallToDelete == null)
            throw new ArgumentException($"A call with ID {id} does not exist.");
  
        DataSource.Calls.Remove(CallToDelete);
    }

    /// <summary>
    /// function deleteAll: clears all the list
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }


    /// <summary>
    /// function read: checks and return the wanted element from the list
    /// </summary>

    public Call? Read(int id)
    {
        return DataSource.Calls.Find(Cal => Cal.CallId == id);
    }


    /// <summary>
    /// function readall: returns the list
    /// </summary>
    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }


    /// <summary>
    /// update function : checks if the element exists, delete it, create it with new details
    /// </summary>

    public void Update(Call item)
    {
        Call? CallToUpdate = Read(item.CallId);

        if (CallToUpdate == null)
            throw new ArgumentException($"A call with ID {item.CallId} does not exist.");
        
        else
        {
            Delete(item.CallId);
            Create(item);
        }

    }
    string ICall.ToString(Call item)
    {
        var details = new System.Text.StringBuilder();
        details.AppendLine("Reading Details:");
        details.AppendLine($"ID: {item.CallId}");
        details.AppendLine($"CallType: {item.CallType}");

        details.AppendLine($"Address: {item.Adress}");
        details.AppendLine($"Latitude: {item.Latitude}");
        details.AppendLine($"Longitude: {item.Longitude}");
        details.AppendLine($"Open Time: {item.OpenTime}");
        details.AppendLine($"Max Time to Finish: {item.MaxTime}");
        details.AppendLine($"Details: {item.Details}");

        return details.ToString();
    }
}


