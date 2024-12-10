namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Function to create a new volunteer in the XML file.
    /// </summary>
    /// <param name="item">Call object to create.</param>
    public void Create(Volunteer item)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        if (volunteers.Any(volunteer => volunteer.VolunteerId == item.VolunteerId))
            throw new DalAlreadyExistException($"A volunteer with ID {item.VolunteerId} already exists.");

        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
    }

    /// <summary>
    /// delete the elements with this id(just one) from the xml file
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="DalDoesNotExistException"></exception>
    public void Delete(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.RemoveAll(it => it.VolunteerId == id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }


    /// <summary>
    /// delete all elements from the xml file
    /// </summary>
    public void DeleteAll()
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        Volunteers.Clear();
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }


    /// <summary>
    /// read a volunteer by its ID from the XML file.
    /// </summary>
    /// <returns>The requested call if found, or null.</returns>
    /*public Volunteer? Read(int volunteerId)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (volunteers == null || volunteers.Count == 0)
        {
            return null;
        }
        return volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == volunteerId);
    }*/
    public Volunteer? Read(int volunteerId)
    {
      //  Console.WriteLine($"Searching for VolunteerId: {volunteerId}");

        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (volunteers == null || volunteers.Count == 0)
        {
         //   Console.WriteLine("The list of volunteers is empty or null.");
            return null;
        }

        //Console.WriteLine("Volunteers loaded:");
        //foreach (var volunteer in volunteers)
        //{
        //    Console.WriteLine($"VolunteerId: {volunteer.VolunteerId}, Name: {volunteer.Name}");
        //}

        var result = volunteers.FirstOrDefault(volunteer => volunteer.VolunteerId == volunteerId);
        //if (result == null)
        //{
        //    Console.WriteLine($"No volunteer found with ID: {volunteerId}");
        //}
        return result;
    }


    /// <summary>
    /// read a volunteer based on a filter delegate
    /// </summary>
    /// <returns>The first volunteer matching the filter, or null.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        return filter != null
            ? volunteers.Where(filter)
            : volunteers;
    }

    /// <summary>
    /// Read a specific volunteer using a filter delegate.
    /// </summary>
    public Volunteer? Read(Func<Volunteer, bool>? filter)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return volunteers.FirstOrDefault(filter);
    }


    /// <summary>
    /// to string function 
    /// </summary>

    public string ToString(Volunteer item)
    {
        var details = new System.Text.StringBuilder();
        details.AppendLine("Volunteer Details:");
        details.AppendLine($"ID: {item.VolunteerId}");
        details.AppendLine($"Full Name: {item.Name}");
        details.AppendLine($"Phone: {item.Phone}");
        details.AppendLine($"Email: {item.Email}");
        details.AppendLine($"Role: {item.RoleType}");
        details.AppendLine($"Type of Distance: {item.DistanceType}");
        details.AppendLine($"Password: {item.Password}");
        details.AppendLine($"Address: {item.Address}");
        details.AppendLine($"Distance: {item.Distance}");
        details.AppendLine($"Latitude: {item.Latitude}");
        details.AppendLine($"Longitude: {item.Longitude}");
        details.AppendLine($"IsActive: {item.IsActive}");

        return details.ToString();
    }

    /// <summary>
    /// Function to update a volunteer in the XML file.
    /// </summary>
    public void Update(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        if (Volunteers.RemoveAll(it => it.VolunteerId == item.VolunteerId) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={item.VolunteerId} does Not exist");
        //Add
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }
}
