namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
///Implement the properties and methods defined in the `IVolunteer` interface to manage configurations related to the "Volunteer" entity.
/// </summary>
public class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// function create new item :
    /// checks if the item already exists in the list, if not, create it
    /// </summary>

    public void Create(Volunteer item)
    {
        if (Read(item.VolunteerId) != null)
            throw new ArgumentException($"A volunteer with ID {item.VolunteerId} already exists.");

        DataSource.Volunteers.Add(item);
    }


    /// <summary>
    /// delete function : delete if the item is in the list
    /// </summary>

    public void Delete(int id)
    {
        Volunteer? volunteerToDelete = Read(id);

        if (volunteerToDelete == null)
            throw new ArgumentException($"A volunteer with ID {id} does not exist.");
        
        DataSource.Volunteers.Remove(volunteerToDelete);
    }

    /// <summary>
    /// function deleteAll: clears all the list
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }


    /// <summary>
    /// function read: checks and return the wanted element from the list
    /// </summary>
   
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(Vol=> Vol.VolunteerId == id ); 
    }


    /// <summary>
    /// function readall: returns the list
    /// </summary>
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }


    /// <summary>
    /// update function : checks if the element exists, delete it, create it with new details
    /// </summary>

    public void Update(Volunteer item)
    {
        Volunteer? volunteerToUpdate = Read(item.VolunteerId);

        if (volunteerToUpdate == null)
            throw new ArgumentException($"A volunteer with ID {item.VolunteerId} does not exist.");

        // שימוש ב-with לעדכון הערכים
        Volunteer updatedVolunteer = volunteerToUpdate with
        {
            Name = item.Name,
            Phone = item.Phone,
            Email = item.Email,
            RoleType = item.RoleType,
            DistanceType = item.DistanceType,
            Password = item.Password,
            Adress = item.Adress,
            Distance = item.Distance,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            IsActive = item.IsActive
        };

        // מחליפים את המתנדב הקודם בחדש
        DataSource.Volunteers.Remove(volunteerToUpdate);
        DataSource.Volunteers.Add(updatedVolunteer);
    }

    /// <summary>
    /// toString : takes volunteer object as input and creates a textual representation of this object
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    string IVolunteer.ToString(Volunteer item)
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
        details.AppendLine($"Address: {item.Adress}");
        details.AppendLine($"Distance: {item.Distance}");
        details.AppendLine($"Latitude: {item.Latitude}");
        details.AppendLine($"Longitude: {item.Longitude}");
        details.AppendLine($"IsActive: {item.IsActive}");

        return details.ToString();

    }
}
