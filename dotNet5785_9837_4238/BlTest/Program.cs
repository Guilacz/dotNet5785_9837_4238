namespace BL;
using Helpers;

using BO;
using System;
using System.Net;
using System.Xml.Linq;

class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("choose an option:");
            Console.WriteLine("1. Volunteers Management");
            Console.WriteLine("2. calls Management");
            Console.WriteLine("3. system Management");
            Console.WriteLine("0. exit");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    VolunteerMenu();
                    break;
                case "2":
                    CallMenu();
                    break;
                case "3":
                    AdminMenu();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("invalid choice");
                    break;
            }
        }
    }

    static void VolunteerMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Display the list of volunteers");
            Console.WriteLine("2. Display volunteer details");
            Console.WriteLine("3. Update volunteer details");
            Console.WriteLine("4. Delete a volunteer");
            Console.WriteLine("5. Create a new volunteer");
            Console.WriteLine("6. Read volunteer details");
            Console.WriteLine("7. Enter to the system of the volunteers");
            Console.WriteLine("0. Return to the main menu");


            string input = Console.ReadLine();

            switch (input)
            {
                case "1":

                    Console.WriteLine("Enter a number to sort by (0 to VolunteerId, 1 to Name, 2 to IsActive, 3 to SumOfCaredCall, 4 to SumOfCancelledCall, 5 to SumOfCallExpired, 6 to CallId, 7 to CallType, or leave empty for no sorting):");
                    string? sortInput = Console.ReadLine();
                    VolunteerSortField? sort = string.IsNullOrWhiteSpace(sortInput) ? null : (VolunteerSortField?)int.Parse(sortInput);

                    Console.WriteLine("Enter 0 for inActive, 1 for active, or leave empty for no filtering:");
                    string? isActiveInput = Console.ReadLine();
                    bool? isActive = string.IsNullOrWhiteSpace(isActiveInput) ? null : (isActiveInput == "1");

                    // קריאה לפונקציה עם פרמטרים אופציונליים
                    var volunteers = s_bl.Volunteer.GetVolunteerInLists(isActive, sort);
                    if (volunteers == null || !volunteers.Any())
                    {
                        Console.WriteLine("No volunteers available.");
                    }
                    else
                    {
                        // שימוש בלולאת foreach להדפסת כל הערכים
                        foreach (var volunteerr in volunteers)
                        {
                            Console.WriteLine($"Volunteer ID: {volunteerr.VolunteerId}");
                            Console.WriteLine($"Name: {volunteerr.Name}");
                            Console.WriteLine($"Is Active: {(volunteerr.IsActive ? "Yes" : "No")}");
                            Console.WriteLine($"Sum of Cared Calls: {volunteerr.SumOfCaredCall}");
                            Console.WriteLine($"Sum of Cancelled Calls: {volunteerr.SumOfCancelledCall}");
                            Console.WriteLine($"Sum of Expired Calls: {volunteerr.SumOfCallExpired}");
                            Console.WriteLine($"Current Call ID: {volunteerr.CallId?.ToString() ?? "N/A"}"); // בודק אם CallId הוא null
                            Console.WriteLine($"Current Call Type: {volunteerr.CallType}");
                            Console.WriteLine("--------------------------------------");
                        }
                    }

                    break;
                case "2":
                    // בקשת זיהוי המתנדב
                    Console.WriteLine("Enter Volunteer ID:");
                    int volunteerId;
                    while (!int.TryParse(Console.ReadLine(), out volunteerId))
                    {
                        Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
                    }

                    // קריאה לפונקציה
                    var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

                    if (volunteer == null)
                    {
                        Console.WriteLine("Volunteer not found.");
                    }
                    else
                    {
                        Console.WriteLine($"Volunteer ID: {volunteer.VolunteerId}");
                        Console.WriteLine($"Name: {volunteer.Name}");
                        Console.WriteLine($"Phone: {volunteer.Phone}");
                        Console.WriteLine($"Email: {volunteer.Email}");
                        Console.WriteLine($"Role: {volunteer.RoleType}");
                        Console.WriteLine($"Distance Type: {volunteer.DistanceType}");
                        Console.WriteLine($"Password: {volunteer.Password ?? "N/A"}");
                        Console.WriteLine($"Address: {volunteer.Adress ?? "N/A"}");
                        Console.WriteLine($"Distance: {volunteer.Distance?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Latitude: {volunteer.Latitude?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Longitude: {volunteer.Longitude?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Is Active: {(volunteer.IsActive ? "Yes" : "No")}");
                        Console.WriteLine($"Sum of Cared Calls: {volunteer.SumOfCaredCall}");
                        Console.WriteLine($"Sum of Cancelled Calls: {volunteer.SumOfCancelledCall}");
                        Console.WriteLine($"Sum of Expired Calls: {volunteer.SumOfCallExpired}");
                    }

                    break;
                case "3":
                    UpdateVolunteer();
                    Console.WriteLine("volunteer update");
                    break;
                case "4":
                    Console.WriteLine("enter id of the volunteer to delete:");
                    int volId = int.Parse(Console.ReadLine());
                    s_bl.Volunteer.Delete(volId);
                    Console.WriteLine("the volunteer was deleted");
                    break;
                case "5":
                    AddVolunteer();
                    Console.WriteLine("the volunteer was added");

                    break;
                case "6":
                    // בקשת זיהוי המתנדב
                    Console.WriteLine("Enter Volunteer ID:");
                    while (!int.TryParse(Console.ReadLine(), out volunteerId))
                    {
                        Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
                    }

                    // קריאה לפונקציה
                    var volunteer1 = s_bl.Volunteer.Read(volunteerId);

                    if (volunteer1 == null)
                    {
                        Console.WriteLine("Volunteer not found.");
                    }
                    else
                    {
                        Console.WriteLine($"Volunteer ID: {volunteer1.VolunteerId}");
                        Console.WriteLine($"Name: {volunteer1.Name}");
                        Console.WriteLine($"Phone: {volunteer1.Phone}");
                        Console.WriteLine($"Email: {volunteer1.Email}");
                        Console.WriteLine($"Role: {volunteer1.RoleType}");
                        Console.WriteLine($"Distance Type: {volunteer1.DistanceType}");
                        Console.WriteLine($"Password: {volunteer1.Password ?? "N/A"}");
                        Console.WriteLine($"Address: {volunteer1.Adress ?? "N/A"}");
                        Console.WriteLine($"Distance: {volunteer1.Distance?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Latitude: {volunteer1.Latitude?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Longitude: {volunteer1.Longitude?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Is Active: {(volunteer1.IsActive ? "Yes" : "No")}");
                        Console.WriteLine($"Sum of Cared Calls: {volunteer1.SumOfCaredCall}");
                        Console.WriteLine($"Sum of Cancelled Calls: {volunteer1.SumOfCancelledCall}");
                        Console.WriteLine($"Sum of Expired Calls: {volunteer1.SumOfCallExpired}");
                    }
                    break;
                case "7":
                    Console.WriteLine("Enter your name:");
                    string name = Console.ReadLine();

                    Console.WriteLine("Enter your password:");
                    string password = Console.ReadLine();
                    if (password != null)
                    {
                        BO.Role type = s_bl.Volunteer.EnterSystem(name, password);
                        Console.WriteLine($"Role: {type}");
                    }
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("invalid choice");
                    break;
            }
        }
    }

    static void CallMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Get list of calls");
            Console.WriteLine("2. Get call details");
            Console.WriteLine("3. Update call");
            Console.WriteLine("4. Delete call");
            Console.WriteLine("5. Create call");
            Console.WriteLine("6. Get list of closed calls");
            Console.WriteLine("7. Get list of open calls");
            Console.WriteLine("8. Update call as finished treatment");
            Console.WriteLine("9. Update call as cancelled");
            Console.WriteLine("10. Choose call for treatment");
            Console.WriteLine("0. Return to the main menu");


            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("Enter a number to the thing that you want to filter by it (1 to Id, 2 to CallId, 3 to CallType, 4 to OpenTime, 5 to TimeToEnd, 6 to LastName, 7 to TimeToCare, 8 to CallInListStatus, or leave empty for no filter):");
                    string? filterInput1 = Console.ReadLine();
                    CallInListSort? filter = string.IsNullOrWhiteSpace(filterInput1) ? null : (CallInListSort?)int.Parse(filterInput1);

                    Console.WriteLine("Enter a number to the thing that you want to sort by it (1 to CallId, 2 to CallType, 3 to Address, 4 to OpenTime, 5 to StartTime, 6 to TypeOfEnd, 7 to FinishTime, or leave empty for no sorting):");
                    string? sortInput1 = Console.ReadLine();
                    CallInListSort? sort = string.IsNullOrWhiteSpace(sortInput1) ? null : (CallInListSort?)int.Parse(sortInput1);

                    Console.WriteLine("Enter any value that eil the object that you want to sort by it (or leave empty for no value):");
                    string? objInput = Console.ReadLine();
                    object? obj = string.IsNullOrWhiteSpace(objInput) ? null : objInput;

                    // קריאה לפונקציה עם פרמטרים אופציונליים
                    var calls = s_bl.Call.GetListOfCalls(filter, obj, sort);
                    if (calls == null || !calls.Any())
                    {
                        Console.WriteLine("No calls available.");
                        return;
                    }

                    foreach (var callss in calls)
                    {
                        //Console.WriteLine($"ID: {callss.Id}");
                        Console.WriteLine($"Call ID: {callss.CallId}");
                        Console.WriteLine($"Call Type: {callss.CallType}");
                        Console.WriteLine($"Open Time: {callss.OpenTime}");
                        Console.WriteLine($"Time to End: {callss.TimeToEnd?.ToString() ?? "N/A"}"); // בודק אם TimeToEnd הוא null
                        Console.WriteLine($"Last Name: {callss.LastName ?? "N/A"}"); // בודק אם LastName הוא null
                        Console.WriteLine($"Time to Care: {callss.TimeToCare?.ToString() ?? "N/A"}"); // בודק אם TimeToCare הוא null
                        Console.WriteLine($"Call Status: {callss.CallInListStatus}");
                        Console.WriteLine($"Number of Assignments: {callss.NumberOfAssignment}");
                        Console.WriteLine("--------------------------------------");
                    }

                    break;
                case "2":
                    Console.WriteLine("enter id of call:");
                    int callId = int.Parse(Console.ReadLine());
                    var call = s_bl.Call.GetCallDetails(callId);

                    if (call != null)
                    {
                        Console.WriteLine($"Call ID: {call.CallId}");
                        Console.WriteLine($"Call Type: {call.CallType}");
                        Console.WriteLine($"Address: {call.Address ?? "N/A"}");
                        Console.WriteLine($"Latitude: {call.Latitude}");
                        Console.WriteLine($"Longitude: {call.Longitude}");
                        Console.WriteLine($"Open Time: {call.OpenTime}");
                        Console.WriteLine($"Max Time: {call.MaxTime?.ToString() ?? "N/A"}");
                        Console.WriteLine($"Call Status: {call.CallStatus}");
                        Console.WriteLine($"Details: {call.Details ?? "N/A"}");
                    }
                    else
                    {
                        Console.WriteLine("Call not found.");
                    }
                    break;
                case "3":
                    UpdateCall();
                    break;
                case "4":
                    Console.WriteLine("enter id of call to delete:");
                    callId = int.Parse(Console.ReadLine());
                    s_bl.Call.Delete(callId);
                    Console.WriteLine("the call was deleted:");

                    break;
                case "5":
                    AddCall();
                    break;
                case "6":
                    Console.Write("Enter a number to the thing that you want to filter by CallType (1 to Math_Primary, 2 to Math_Middle, 3 to Math_High, 4 to English_Primary, 5 to English_Middle, 6 to English_High, 7 to Grammary_Primary, 8 to Grammary_Middle, 9 to Grammary_High, or leave empty for no filter): ");
                    string? filterInput2 = Console.ReadLine();
                    CallType? filter1 = string.IsNullOrWhiteSpace(filterInput2) ? null : (CallType?)int.Parse(filterInput2);

                    Console.Write("Enter a number to the thing that you want to sort by it (1 to CallId, 2 to CallType, 3 to Address, 4 to OpenTime, 5 to StartTime, 6 to TypeOfEnd, 7 to FinishTime, or leave empty for no sorting): ");
                    string? sortInput2 = Console.ReadLine();
                    CloseCallInListSort? sort1 = string.IsNullOrWhiteSpace(sortInput2) ? null : (CloseCallInListSort?)int.Parse(sortInput2);

                    Console.WriteLine("Enter the ID of the volunteer:");
                    int volId;
                    while (!int.TryParse(Console.ReadLine(), out volId) || volId <= 0)
                    {
                        Console.WriteLine("Invalid ID. Please enter a valid positive integer:");
                    }

                    // Call the function with the required and optional parameters
                    var closedCalls = s_bl.Call.GetListOfClosedCall(volId, filter1, sort1);

                    // בדיקה אם האוסף ריק או null
                    if (closedCalls == null || !closedCalls.Any())
                    {
                        Console.WriteLine("No closed calls available.");
                    }
                    else
                    {
                        // שימוש בלולאת foreach להדפסת כל הערכים
                        foreach (var closedCall in closedCalls)
                        {
                            Console.WriteLine($"Call ID: {closedCall.CallId}");
                            Console.WriteLine($"Call Type: {closedCall.CallType}");
                            Console.WriteLine($"Address: {closedCall.Adress}");
                            Console.WriteLine($"Open Time: {closedCall.OpenTime}");
                            Console.WriteLine($"Start Time: {closedCall.StartTime}");
                            Console.WriteLine($"Type of End: {closedCall.TypeOfEnd?.ToString() ?? "N/A"}"); // בודק אם TypeOfEnd הוא null
                            Console.WriteLine($"Finish Time: {closedCall.FinishTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}"); // בודק אם FinishTime הוא null
                            Console.WriteLine("--------------------------------------");
                        }
                    }

                    break;
                case "7":


                    /*Console.Write("Enter a number to the thing that you want to sort by it:");
                    Console.Write("1 to Math_Primary, 2 to Math_Middle, 3 to Math_High, 4 to English_Primary, 5 to English_Middle, 6 to English_High, 7 to Grammary_Primary, 8 to Grammary_Middle, 9 to Grammary_High");
                    CallType filter2 = (CallType)int.Parse(Console.ReadLine()!);
                    Console.Write("Enter a number to the thing that you want to sort by it:");
                    Console.Write("1 to CallId, 2 to CallType, 3 to Address, 4 to OpenTime, 5 to MaxTime, 6 to Details, 7 to Distance");
                    OpenCallInListSort sort2 = (OpenCallInListSort)int.Parse(Console.ReadLine()!);
                    Console.WriteLine("enter id of volunteer:");
                    int voluId = int.Parse(Console.ReadLine());
                    var openCalls = s_bl.Call.GetListOfOpenCall(voluId, filter2, sort2); // זימון הפונקציה
                    */
                    Console.Write("Enter a number to filter by CallType (1 to Math_Primary, 2 to Math_Middle, 3 to Math_High, 4 to English_Primary, 5 to English_Middle, 6 to English_High, 7 to Grammary_Primary, 8 to Grammary_Middle, 9 to Grammary_High, or leave empty for no filter): ");
                    string? filterInput = Console.ReadLine();
                    CallType? filter2 = string.IsNullOrWhiteSpace(filterInput) ? null : (CallType?)int.Parse(filterInput);

                    Console.Write("Enter a number to sort by (1 to CallId, 2 to CallType, 3 to Address, 4 to OpenTime, 5 to MaxTime, 6 to Details, 7 to Distance, or leave empty for no sorting): ");
                    string? sortInput = Console.ReadLine();
                    OpenCallInListSort? sort2 = string.IsNullOrWhiteSpace(sortInput) ? null : (OpenCallInListSort?)int.Parse(sortInput);

                    Console.WriteLine("enter id of volunteer:");
                    int voluId = int.Parse(Console.ReadLine());
                    var openCalls = s_bl.Call.GetListOfOpenCall(voluId, filter2, sort2);

                    if (openCalls == null || !openCalls.Any())
                    {
                        Console.WriteLine("No open calls available.");
                    }
                    else
                    {
                        // שימוש בלולאת foreach להדפסת כל הערכים
                        foreach (var openCall in openCalls)
                        {
                            Console.WriteLine($"Call ID: {openCall.CallId}");
                            Console.WriteLine($"Call Type: {openCall.CallType}");
                            Console.WriteLine($"Address: {openCall.Address}");
                            Console.WriteLine($"Open Time: {openCall.OpenTime:yyyy-MM-dd HH:mm:ss}");
                            Console.WriteLine($"Max Time: {openCall.MaxTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}"); // בודק אם MaxTime הוא null
                            Console.WriteLine($"Details: {openCall.Details ?? "N/A"}"); // בודק אם Details הוא null
                            Console.WriteLine($"Distance: {openCall.Distance:F2} km"); // מדפיס את המרחק בפורמט עם 2 ספרות אחרי הנקודה
                            Console.WriteLine("--------------------------------------");
                        }
                    }

                    break;
                case "8":
                    Console.WriteLine("enter id of volunteer:");
                    volId = int.Parse(Console.ReadLine());
                    Console.WriteLine("enter id of call:");
                    callId = int.Parse(Console.ReadLine());
                    s_bl.Call.UpdateCallFinished(volId, callId);
                    Console.WriteLine("updated");
                    break;
                case "9":
                    Console.WriteLine("enter id of call:");
                    callId = int.Parse(Console.ReadLine());
                    Console.WriteLine("enter id of assignment:");
                    int assiId = int.Parse(Console.ReadLine());
                    s_bl.Call.UpdateCallCancelled(callId, assiId);
                    Console.WriteLine("updated");
                    break;
                case "10":
                    Console.WriteLine("enter id of volunteer:");
                    volId = int.Parse(Console.ReadLine());
                    Console.WriteLine("enter id of call:");
                    callId = int.Parse(Console.ReadLine());
                    s_bl.Call.ChoiceOfCallToCare(volId, callId);
                    Console.WriteLine("the call was care");
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("incorrect choice");
                    break;
            }
        }
    }

    static void AdminMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Initialize database");
            Console.WriteLine("2. Reset database");
            Console.WriteLine("3. Get maximum time range");
            Console.WriteLine("4. Update maximum time range");
            Console.WriteLine("5. Get current clock");
            Console.WriteLine("6. Forward the clock");
            Console.WriteLine("0. Return to the main menu");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    s_bl.Admin.InitializeDB();
                    Console.WriteLine("the initialize succeede");
                    break;
                case "2":
                    s_bl.Admin.ResetDB();
                    Console.WriteLine("the reset succeede");

                    break;
                case "3":
                    TimeSpan maxRange = s_bl.Admin.GetMaxRange();
                    Console.WriteLine($"Max Range: {maxRange}");
                    break;
                case "4":
                    Console.WriteLine("enter the max range:");
                    TimeSpan maxrange = TimeSpan.Parse(Console.ReadLine());
                    s_bl.Admin.SetMaxRange(maxrange);
                    Console.WriteLine("the update of the max range succeede");
                    break;
                case "5":
                    DateTime cl = s_bl.Admin.GetClock();
                    Console.WriteLine($"get clock: {cl}");
                    break;
                case "6":
                    Console.WriteLine("enter the unit to update the clock:");
                    Console.WriteLine("0 to add 1 minute, 1 to add 1 hour, 2 to add 1 day, 3 to add 1 month, 4 to add 1 year");

                    string unit = Console.ReadLine();
                    BO.TimeUnit timeUnit = (BO.TimeUnit)Enum.Parse(typeof(BO.TimeUnit), unit);
                    s_bl.Admin.ForwardClock(timeUnit);
                    Console.WriteLine("the update of the time succeede");

                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("incorrect choice");
                    break;
            }
        }
    }

    private static void UpdateVolunteer()
    {
        int id2;
        Console.WriteLine("Enter ID of the person who ask to change:");
        while (!int.TryParse(Console.ReadLine(), out id2))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
        }
        int id;
        Console.WriteLine("Enter ID:");
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
        }

        Volunteer? volunteer = s_bl.Volunteer.Read(id);
        if (volunteer == null)
        {
            Console.WriteLine("Volunteer not found.");
            return;
        }

        // עדכון שם המתנדב
        Console.WriteLine($"Current name: {volunteer.Name}. Enter new name (or press Enter to keep current):");
        string newName = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newName))
        {
            volunteer.Name = newName; // שינוי שם בלבד
        }
        // עדכון טלפון
        Console.WriteLine($"Current phone: {volunteer.Phone}. Enter new phone (or press Enter to keep current):");
        string newPhone = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newName))
        {
            volunteer.Phone = newPhone; // שינוי שם בלבד
        }
        // עדכון אימייל
        Console.WriteLine($"Current email: {volunteer.Email}. Enter new email (or press Enter to keep current):");
        string newEmail = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newName))
        {
            volunteer.Email = newEmail; // שינוי שם בלבד
        }
        // עדכון סיסמה
        Console.WriteLine($"Current password: {volunteer.Password}. Enter new password (or press Enter to keep current):");
        string newPassword = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newName))
        {
            volunteer.Password = newPassword; // שינוי שם בלבד
        }
        // עדכון כתובת
        Console.WriteLine($"Current address: {volunteer.Adress}. Enter new address (or press Enter to keep current):");
        string newAddress = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newName))
        {
            volunteer.Adress = newAddress; // שינוי שם בלבד
        }
        var coordinates = Helpers.Tools.GetAddressCoordinates(volunteer.Adress);
        volunteer.Latitude = coordinates.Latitude;
        volunteer.Longitude = coordinates.Longitude;
        // עדכון מרחק
        Console.WriteLine($"Current distance: {volunteer.Distance}. Enter new distance (or press Enter to keep current):");
        string newDistanceInput = Console.ReadLine()!;
        if (double.TryParse(newDistanceInput, out double newDistance))
        {
            volunteer.Distance = newDistance; // עדכון ישיר של הערך
        }

        // קריאה לעדכון במערכת
        s_bl.Volunteer.Update(id2, volunteer);
        Console.WriteLine("Volunteer details updated.");
    }
    private static void AddVolunteer()
    {
        Console.Write("Enter volunteer id: ");
        int id = int.Parse(Console.ReadLine()!);
        Console.Write("Enter volunteer name: ");
        string name = Console.ReadLine()!;
        Console.Write("Enter volunteer phone: ");
        string phone = Console.ReadLine()!;
        Console.Write("Enter volunteer email: ");
        string email = Console.ReadLine()!;
        Console.Write("Enter 1 for manager and 2 for volunteer: ");
        Role role = (Role)int.Parse(Console.ReadLine()!);
        Console.Write("Enter 1 forAirDistance, 2 for WalkDistance,and 3 for CarDistance: ");
        DistanceType distance = (DistanceType)int.Parse(Console.ReadLine()!);
        Console.Write("Enter volunteer password: ");
        string password = Console.ReadLine()!;
        Console.Write("Enter volunteer address: ");
        string address = Console.ReadLine()!;

        Console.Write("Enter maximum distance: ");
        double dis = int.Parse(Console.ReadLine()!);

        var coordinates = Helpers.Tools.GetAddressCoordinates(address);
        double latitude = coordinates.Latitude;
        double longitude = coordinates.Longitude;
        Volunteer v = new Volunteer() { VolunteerId = id, Name = name, Phone = phone, Email = email, RoleType = role, DistanceType = distance, Password = password, Adress = address, Distance = dis };
        s_bl.Volunteer?.Create(v);
        Console.WriteLine("Volunteer added.");

    }

    private static void AddCall()
    {
        //id
        //var config = DalApi.IConfig(); // אם יש פונקציה כזו

       // int callId; 
       // BO.Call = s_bl.Call.Read(callId++);
        
        //lesson
        Console.Write("What type of lesson do you want, and what level ?");
        Console.WriteLine("1.Math_Primary");
        Console.WriteLine("2.Math_Middle");
        Console.WriteLine("3.Math_High");
        Console.WriteLine("4.English_Primary");
        Console.WriteLine("5.English_Middle");
        Console.WriteLine("6.English_High");
        Console.WriteLine("7.Grammary_Primary");
        Console.WriteLine("8.Grammary_Middle");
        Console.WriteLine("9.Grammary_High");
        CallType lesson = (CallType)int.Parse(Console.ReadLine()!);
        //adress
        Console.WriteLine("Enter an address");
        string address = Console.ReadLine()!;
        var coordinates = Helpers.Tools.GetAddressCoordinates(address);
        double latitude = coordinates.Latitude;
        double longitude = coordinates.Longitude;
        //opentime
        Console.WriteLine("Enter the time you want to start :");
        //details
        Console.WriteLine("Enter details if needed");
        string details = (Console.ReadLine()!);
        //maxtime
        Console.WriteLine("Enter the latest time you want to start");
        int year = int.Parse(Console.ReadLine()!);
        int month = int.Parse(Console.ReadLine()!);
        int day = int.Parse(Console.ReadLine()!);
        DateTime specificDate2 = new DateTime(year, month, day);

        //Call c = new Call { CallId = callId, CallType = lesson, Address = address!, Latitude = latitude, Longitude = longitude, OpenTime = DateTime.Now, Details = details, MaxTime = specificDate2 };
        Call c = new Call {  CallType = lesson, Address = address!, Latitude = latitude, Longitude = longitude, OpenTime = DateTime.Now, Details = details, MaxTime = specificDate2 };

        s_bl.Call?.Create(c);
        Console.WriteLine("Call added.");
    }


    private static void UpdateCall()
    {
        int id;
        Console.WriteLine("Enter ID:");
        while (!int.TryParse(Console.ReadLine(), out id))
        {
            Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
        }

        Call? call = s_bl.Call.Read(id);
        if (call == null)
        {
            Console.WriteLine("Call not found.");
            return;
        }

        // עדכון סוג הקריאה
        Console.WriteLine($"Current call type: {call.CallType}. Enter new call type (or press Enter to keep current):");
        string newCallTypeInput = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newCallTypeInput))
        {
            if (Enum.TryParse<CallType>(newCallTypeInput, out CallType newCallType))
            {
                call.CallType = newCallType; // עדכון סוג הקריאה
            }
        }

        // עדכון כתובת
        Console.WriteLine($"Current address: {call.Address}. Enter new address (or press Enter to keep current):");
        string newAddress = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newAddress))
        {
            call.Address = newAddress; // שינוי כתובת
        }
        var coordinates = Helpers.Tools.GetAddressCoordinates(call.Address);
        call.Latitude = coordinates.Latitude;
        call.Longitude = coordinates.Longitude;
        call.OpenTime = DateTime.Now;
        // עדכון זמן סיום מקסימלי
        Console.WriteLine($"Current max time: {call.MaxTime}. Enter new max time (format: yyyy-MM-dd HH:mm:ss or press Enter to keep current):");
        string newMaxTimeInput = Console.ReadLine()!;
        if (DateTime.TryParse(newMaxTimeInput, out DateTime newMaxTime))
        {
            call.MaxTime = newMaxTime; // עדכון זמן סיום מקסימלי
        }

        // עדכון פרטים נוספים
        Console.WriteLine($"Current details: {call.Details}. Enter new details (or press Enter to keep current):");
        string newDetails = Console.ReadLine()!;
        if (!string.IsNullOrEmpty(newDetails))
        {
            call.Details = newDetails; // עדכון פרטים נוספים
        }

        // קריאה לעדכון במערכת
        s_bl.Call.Update(call);
        Console.WriteLine("Call details updated.");
    }

}
