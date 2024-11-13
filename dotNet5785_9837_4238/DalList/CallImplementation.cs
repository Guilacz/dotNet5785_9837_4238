namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
///Implement the properties and methods defined in the `ICall` interface to manage configurations related to the "call" entity.
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// function create new item :
    /// checks if the item already exists in the list, if not, create it
    /// </summary>

    public void Create(Call item)//The new ID of the new object added to the list should be returned as a return value.

    {
        int currentId = Config.NextCallId;//Type a new running number type for the next call
        Call currentItem = new Call(currentId, item.CallType, item.Adress, item.Latitude, item.Longitude, item.OpenTime, item.MaxTime, item.Details);
        DataSource.Calls.Add(currentItem);//Added the new item to the database
        //return currentId;

    }
  


    /// <summary>
    /// delete function : delete if the item is in the list
    /// </summary>

    public void Delete(int id)
    {
        Call? CallToDelete = Read(id);

        if (CallToDelete == null)
            throw new DalDeletionImpossible($"A call with ID {id} does not exist.");
  
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
    /// function read: checks and return the wanted element
    /// </summary>

    public Call? Read(int callId)
    {
        return DataSource.Calls.FirstOrDefault(call => call.CallId == callId);
    }



    /// <summary>
    /// new function Read which goes according to a delegate function
    /// </summary>

    /// <exception cref="NotImplementedException"></exception>
    public Call? Read(Func<Call, bool>? filter)
    {
        return DataSource.Calls?.FirstOrDefault(filter);
    }


    /// <summary>
    /// function readall: returns the items
    /// </summary>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        if (filter != null)
        {
            return from item in DataSource.Calls
                   where filter(item)
                   select item;
        }
        return from item in DataSource.Calls
               select item;
    }


    /// <summary>
    /// update function : checks if the element exists, delete it, create it with new details
    /// </summary>

    public void Update(Call item)
    {
        Call? CallToUpdate = Read(item.CallId);

        if (CallToUpdate == null)
            throw new DalDoesNotExistException($"A call with ID {item.CallId} does not exist.");
        
        else
        {
            Delete(item.CallId);
            Create(item);
        }

    }

    /// <summary>
    /// toString : takes call object as input and creates a textual representation of this object
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
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


