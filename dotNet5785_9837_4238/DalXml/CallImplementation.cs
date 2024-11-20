namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
  
    /// <summary>
    /// Function to create a new call in the XML file.
    /// </summary>
    /// <param name="item">Call object to create.</param>
    public void Create(Call item)
    {
        
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        if (calls.Any(call => call.CallId == item.CallId))
            throw new DalAlreadyExistException($"A call with ID {item.CallId} already exists.");

        var newCall = new Call
        {
            CallId = Config.NextCallId, 
            CallType = item.CallType,
            Adress = item.Adress,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            OpenTime = item.OpenTime,
            MaxTime = item.MaxTime,
            Details = item.Details
        };
        calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// delete the elements with this id(just one) from the xml file
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.CallId == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// <summary>
    /// delete all elements from the xml file
    /// </summary>
    public void DeleteAll()
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Calls.Clear();
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// <summary>
    /// read a call by its ID from the XML file.
    /// </summary>
    /// <returns>The requested call if found, or null.</returns>
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(call => call.CallId == id);
    }

    /// <summary>
    /// read a call based on a filter delegate
    /// </summary>
    /// <returns>The first call matching the filter, or null.</returns>
    public Call? Read(Func<Call, bool>? filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Function to read all calls with filter.
    /// </summary>
    /// <returns>All calls matching the filter, or all calls if no filter is provided.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter != null ? calls.Where(filter) : calls;
    }

    /// <summary>
    /// to string function 
    /// </summary>
    
    string ICall.ToString(Call item)
    {
        var details = new System.Text.StringBuilder();
        details.AppendLine("Call Details:");
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

    /// <summary>
    /// Function to update a call in the XML file.
    /// </summary>
    
    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        if (Calls.RemoveAll(it => it.CallId == item.CallId) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.CallId} does Not exist");
        //Add
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
}
