using Dal;
using DalApi;
using DO;
using System.Threading.Channels;

namespace DalTest
{
    public static class Program
    {
        private static IVolunteer s_dalVolunteer = new VolunteerImplementation(); // stage 1
        private static ICall s_dalCall = new CallImplementation(); // stage 1
        private static IAssignment s_dalAssignment = new AssignmentImplementation(); // stage 1
        private static IConfig s_dalConfig = new ConfigImplementation(); // stage 1

        /// <summary>
        /// Main entry point of the application. Displays the main menu and handles user input.
        /// </summary>
        public static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("enter a number");
                    int num = int.Parse(Console.ReadLine()!);
                    if (num == 0)
                        return;
                    ShowMainMenu();
                    if(!MainMenuChoice())
                        return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Displays the main menu options for navigating to different sub-menus or exiting.
        /// </summary>
        private static void ShowMainMenu()
        {
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Volunteer Menu");
            Console.WriteLine("2. Call Menu");
            Console.WriteLine("3. Assignment Menu");
            Console.WriteLine("4. Initialize Database");
            Console.WriteLine("5. Display All Data");
            Console.WriteLine("6. Configuration Menu");
            Console.WriteLine("7. Reset Database and Configuration");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");
        }

        /// <summary>
        /// Method to process the user's choice from the main menu
        /// </summary>
        /// <returns></returns>
        private static bool MainMenuChoice()
        {
            string input = Console.ReadLine()!;
            switch (input)
            {
                case "1":
                    ShowInterMenu("Volunteer", ProcessVolunteerMenu);
                    break;
                case "2":
                    ShowInterMenu("Call", ProcessCallMenu);
                    break;
                case "3":
                    ShowInterMenu("Assignment", ProcessAssignmentMenu);
                    break;
                case "4":
                    InitializeDatabase();
                    break;
                case "5":
                    DisplayAllData();
                    break;
                case "6":
                    ShowInterMenu("Configuration", ProcessConfigMenu);
                    break;
                case "7":
                    ResetDatabaseAndConfig();
                    break;
                case "0":
                    return false;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
            return true;
        }


        /// <summary>
        ///  Helper method to display a CRUD menu for any entity
        ///the function "ShowInterMenu" receives a name and a process fonction to accomplish.
        ///according to what "ShowInterMenu" receives, it will lead to ProcessVolunteerMenu, ProcessAssignmentMenu or ProcesscallMenu
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="processEntityMenu"></param>
        private static void ShowInterMenu(string entityName, Action processEntityMenu)
        {
            Console.WriteLine($"{entityName} Menu:");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read");
            Console.WriteLine("3. Read All");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Delete All");
            Console.WriteLine("0. Back to Main Menu");
            processEntityMenu();
        }


        /// <summary>
        /// // choice 1 :Process menu options for Volunteer entity
        /// </summary>
        private static void ProcessVolunteerMenu()
        {
            try
            {
                string choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "1":
                        AddVolunteer();
                        break;
                    case "2":
                        DisplayVolunteer();
                        break;
                    case "3":
                        DisplayAllVolunteers();
                        break;
                    case "4":
                        UpdateVolunteer();
                        break;
                    case "5":
                        DeleteVolunteer();
                        break;
                    case "6":
                        s_dalVolunteer?.DeleteAll();
                        Console.WriteLine("All volunteers deleted.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// choice 2 : Process menu options for Call entity
        /// </summary>
        private static void ProcessCallMenu()
        {
            try
            {
                string choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "1":
                        AddCall();
                        break;
                    case "2":
                        DisplayCall();
                        break;
                    case "3":
                        DisplayAllCalls();
                        break;
                    case "4":
                        UpdateCall();
                        break;
                    case "5":
                        DeleteCall();
                        break;
                    case "6":
                        s_dalCall?.DeleteAll();
                        Console.WriteLine("All calls deleted.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// choice 3 : Process menu options for assignment entity
        /// </summary>
        private static void ProcessAssignmentMenu()
        {
            try
            {
                string choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "1":
                        AddAssignment();
                        break;
                    case "2":
                        DisplayAssignment();
                        break;
                    case "3":
                        DisplayAllAssignments();
                        break;
                    case "4":
                        UpdateAssignment();
                        break;
                    case "5":
                        DeleteAssignment();
                        break;
                    case "6":
                        s_dalAssignment?.DeleteAll();
                        Console.WriteLine("All assignments deleted.");
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }


        /// <summary>
        /// choice 3 : Process menu options for assignment entity
        /// </summary>
        private static void InitializeDatabase()
        {
            Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
            Console.WriteLine("Database initialized.");
        }


        /// <summary>
        /// choice 5 :Method to display all data from the database
        /// </summary>
        private static void DisplayAllData()
        {
            DisplayAllVolunteers();
            DisplayAllCalls();
            DisplayAllAssignments();
        }



        /// <summary>
        ///  //choice 6 : Configuration Menu
        ///we decided to give the possibility to add to the clock 1 minute 1 hour 1 day 1 week 1 month
        /// </summary>

        private static void ProcessConfigMenu()
        {
            bool exitConfigMenu = false;

            while (!exitConfigMenu)
            {
                Console.Clear();
                Console.WriteLine("---- Config Menu ----");
                Console.WriteLine("1. Advance system clock by 1 minute");
                Console.WriteLine("2. Advance system clock by 1 hour");
                Console.WriteLine("3. Advance system clock by 1 day");
                Console.WriteLine("4. Advance system clock by 7 days");
                Console.WriteLine("5. Advance system clock by 1 month");
                Console.WriteLine("6. Show current system clock");
                Console.WriteLine("7. Set a new value for the clock");
                Console.WriteLine("8. Reset all config values");
                Console.WriteLine("9. exit");
                Console.Write("Please select an option: ");

                string choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "1":
                        s_dalConfig.Clock = s_dalConfig.Clock.Add(TimeSpan.FromMinutes(1));
                        Console.WriteLine("Clock advanced by 1 minute.");
                        break;
                    case "2":
                        s_dalConfig.Clock = s_dalConfig.Clock.Add(TimeSpan.FromHours(1));
                        Console.WriteLine("Clock advanced by 1 hour.");
                        break;
                    case "3":
                        s_dalConfig.Clock = s_dalConfig.Clock.Add(TimeSpan.FromDays(1));
                        Console.WriteLine("Clock advanced by 1 day.");
                        break;
                    case "4":
                        s_dalConfig.Clock = s_dalConfig.Clock.Add(TimeSpan.FromDays(7));
                        Console.WriteLine("Clock advanced by 7 days.");
                        break;
                    case "5":
                        s_dalConfig.Clock = s_dalConfig.Clock.Add(TimeSpan.FromDays(30));  // Approx. 1 month
                        Console.WriteLine("Clock advanced by 1 month.");
                        break;
                    case "6":
                        Console.WriteLine("Current system clock: " + s_dalConfig.Clock);
                        break;
                    case "7":
                        // Allow user to set a specific new value for the clock
                        Console.Write("Enter the new value for the date): ");
                        DateTime newTime;
                        while (!DateTime.TryParse(Console.ReadLine(), out newTime))
                        {
                            Console.WriteLine("Invalid date format. Please enter a valid date (YYYY-MM-DD HH:mm:ss): ");
                        }
                        s_dalConfig.Clock = newTime;
                        Console.WriteLine("Clock updated.");
                        break;
                    case "8":
                        s_dalConfig.Reset();
                        Console.WriteLine("All config values have been reset.");
                        break;
                    case "9":
                        exitConfigMenu = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }

                if (choice != "10")
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// choice 7 : Method to reset the database and configuration to initial state
        /// </summary>
        private static void ResetDatabaseAndConfig()
        {
            s_dalVolunteer.DeleteAll();
            s_dalCall.DeleteAll();
            s_dalAssignment.DeleteAll();
            s_dalConfig.Reset();
            Console.WriteLine("Database and configuration reset.");
        }





        /// NOW ARE GOING TO BE ALL THE FUNCTIONS OF THE CRUD OF VOLUNTEERS, CALL AND ASSIGNMENT
        /// each time we used TryParse, we did the fullchecking, to ask until we get a correct answer


        /// <summary>
        /// Add a new volunteer
        /// </summary>

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
            Distance distance = (Distance)int.Parse(Console.ReadLine()!);
            Console.Write("Enter volunteer password: ");
            string password = Console.ReadLine()!;
            Console.Write("Enter volunteer address: ");
            string address = Console.ReadLine()!;
            
            Console.Write("Enter maximum distance: ");
            double dis = int.Parse(Console.ReadLine()!);

            Volunteer v = new Volunteer (){ Name = name, Phone = phone, Email = email, RoleType = role, DistanceType = distance, Password = password, Adress = address, Distance = dis };
            s_dalVolunteer?.Create(v);
            Console.WriteLine("Volunteer added.");

        }

        /// <summary>
        ///  Display a volunteer by ID
        ///  Prompts the user to enter a volunteer ID, validates the input, 
        /// and displays the corresponding volunteer details if found.
        /// </summary>
        private static void DisplayVolunteer()
        {
            Console.Write("Enter volunteer ID: ");
            int id;
            while (!(int.TryParse(Console.ReadLine(), out id)))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID: ");
            }
            var volunteer = s_dalVolunteer?.Read(id);
            Console.WriteLine(volunteer != null ? volunteer.ToString() : "Volunteer not found.");
        }


        /// <summary>
        ///  Display all volunteer
        /// </summary>
        private static void DisplayAllVolunteers()
        {
            if (s_dalVolunteer != null)
            {
                foreach (var vol in s_dalVolunteer.ReadAll())
                {
                    Console.WriteLine(vol);
                }
            }
        }


        /// <summary>
        ///  update volunteer
        ///  Prompts the user for an ID, verifies it, and updates the volunteer if found. 
        /// Displays a message indicating whether the update was successful or if the volunteer was not found.
        /// </summary>
        /*private static void UpdateVolunteer()
        {
            int id;
            Console.WriteLine("Enter ID:");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }
            
            bool volunteerFound = false;
            foreach (var item in s_dalVolunteer.ReadAll())
            {
                if (item.VolunteerId == id)
                {
                    s_dalVolunteer?.Update(item);
                    Console.WriteLine("Volunteer updated.");
                    volunteerFound = true;
                    break;
                }
            }

            if (!volunteerFound)
            {
                Console.WriteLine("Volunteer not found.");
            }
        }
        */
        /*
        private static void UpdateVolunteer()
        {
            int id;
            Console.WriteLine("Enter ID:");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }

            Volunteer? volunteer = s_dalVolunteer.Read(id);
            if (volunteer == null)
            {
                Console.WriteLine("Volunteer not found.");
                return;
            }

            // עדכון שם המתנדב
            Console.WriteLine($"Current name: {volunteer.Name}. Enter new name (or press Enter to keep current):");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName)) volunteer.Name = newName;

            // עדכון טלפון
            Console.WriteLine($"Current phone: {volunteer.Phone}. Enter new phone (or press Enter to keep current):");
            string newPhone = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPhone)) volunteer.Phone = newPhone;

            // עדכון אימייל
            Console.WriteLine($"Current email: {volunteer.Email}. Enter new email (or press Enter to keep current):");
            string newEmail = Console.ReadLine();
            if (!string.IsNullOrEmpty(newEmail)) volunteer.Email = newEmail;

            // עדכון סיסמה
            Console.WriteLine($"Current password: {volunteer.Password}. Enter new password (or press Enter to keep current):");
            string newPassword = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPassword)) volunteer.Password = newPassword;

            // עדכון כתובת
            Console.WriteLine($"Current address: {volunteer.Adress}. Enter new address (or press Enter to keep current):");
            string newAddress = Console.ReadLine();
            if (!string.IsNullOrEmpty(newAddress)) volunteer.Adress = newAddress;

            // עדכון מרחק
            Console.WriteLine($"Current distance: {volunteer.Distance}. Enter new distance (or press Enter to keep current):");
            string newDistanceInput = Console.ReadLine();
            if (double.TryParse(newDistanceInput, out double newDistance)) volunteer.Distance = newDistance;

            // קריאה לעדכון במערכת
            s_dalVolunteer.Update(volunteer);
            Console.WriteLine("Volunteer details updated.");
        }

        */
        private static void UpdateVolunteer()
        {
            int id;
            Console.WriteLine("Enter ID:");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }

            Volunteer? volunteer = s_dalVolunteer.Read(id);
            if (volunteer == null)
            {
                Console.WriteLine("Volunteer not found.");
                return;
            }

            // עדכון שם המתנדב
            Console.WriteLine($"Current name: {volunteer.Name}. Enter new name (or press Enter to keep current):");
            string newName = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(newName)) volunteer = volunteer with { Name = newName };

            // עדכון טלפון
            Console.WriteLine($"Current phone: {volunteer.Phone}. Enter new phone (or press Enter to keep current):");
            string newPhone = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(newPhone)) volunteer = volunteer with { Phone = newPhone };

            // עדכון אימייל
            Console.WriteLine($"Current email: {volunteer.Email}. Enter new email (or press Enter to keep current):");
            string newEmail = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(newEmail)) volunteer = volunteer with { Email = newEmail };

            // עדכון סיסמה
            Console.WriteLine($"Current password: {volunteer.Password}. Enter new password (or press Enter to keep current):");
            string newPassword = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(newPassword)) volunteer = volunteer with { Password = newPassword };

            // עדכון כתובת
            Console.WriteLine($"Current address: {volunteer.Adress}. Enter new address (or press Enter to keep current):");
            string newAddress = Console.ReadLine()!;
            if (!string.IsNullOrEmpty(newAddress)) volunteer = volunteer with { Adress = newAddress };

            // עדכון מרחק
            Console.WriteLine($"Current distance: {volunteer.Distance}. Enter new distance (or press Enter to keep current):");
            string newDistanceInput = Console.ReadLine()!;
            if (double.TryParse(newDistanceInput, out double newDistance)) volunteer = volunteer with { Distance = newDistance };

            // קריאה לעדכון במערכת
            s_dalVolunteer.Update(volunteer);
            Console.WriteLine("Volunteer details updated.");
        }
        /// <summary>
        ///  Delete a volunteer by ID
        ///   Prompts the user for an ID, validates it, and deletes the volunteer if the ID is valid. 
        /// Displays a confirmation message upon deletion.
        /// </summary>
        private static void DeleteVolunteer()
        {
            int id;
            Console.Write("Enter volunteer ID to delete: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }
            s_dalVolunteer?.Delete(id);
            Console.WriteLine("Volunteer deleted.");
        }


        /// <summary>
        ///  Add a new call 
        /// </summary>
        private static void AddCall()
        {
            //id
            int callId = s_dalConfig.nextCallId;
            //lesson
            Console.Write("What type of lesson do you want, and what level ?");
            Console.WriteLine("1.Math_Primary");
            Console.WriteLine("2.Math_Middle");
            Console.WriteLine("3.Math_High");
            Console.WriteLine("3.English_Primary");
            Console.WriteLine("3.English_Middle");
            Console.WriteLine("3.English_High");
            Console.WriteLine("3.Grammary_Primary");
            Console.WriteLine("3.Grammary_Middle");
            Console.WriteLine("3.Grammary_High");
            CallType lesson = (CallType)int.Parse(Console.ReadLine()!);
            //adress
            Console.WriteLine("Enter an address");
            string address = Console.ReadLine()!;
            //latitude
            Console.WriteLine("Enter the latitude");
            double l1 = int.Parse(Console.ReadLine()!);
            //longitude
            Console.WriteLine("Enter the longitude");
            double l2 = int.Parse(Console.ReadLine()!);
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

            Call c = new Call { CallId = callId, CallType = lesson, Adress = address!, Latitude = l1, Longitude = l2, OpenTime = s_dalConfig.Clock, Details = details, MaxTime = specificDate2 };
            s_dalCall?.Create(c);
            Console.WriteLine("Call added.");
        }

        /// <summary>
        ///  Display a call by ID 
        /// Displays the details of a call by its ID. Prompts the user to enter a valid call ID, then retrieves and displays the call if found, or shows a "Call not found" message.
        /// </summary>
        private static void DisplayCall()
        {
            int id;
            Console.Write("Enter call ID: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }
            var call = s_dalCall?.Read(id);
            Console.WriteLine(call != null ? call.ToString() : "Call not found.");
        }


        /// <summary>
        ///  Display all calls
        /// </summary>
        private static void DisplayAllCalls()
        {
            if (s_dalCall != null)
            {
                foreach (var call in s_dalCall.ReadAll())
                {
                    Console.WriteLine(call);
                }
            }
        }

        /// <summary>
        /// Update a student's details 
        /// Updates the details of a call by its ID. Prompts the user for an ID, validates it, and updates the call if found. 
        /// Displays a message indicating whether the update was successful or if the call was not found.
        /// </summary>
        private static void UpdateCall()
        {
            int id;
            Console.WriteLine("Enter ID:");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }

            bool callFound = false;
            foreach (var item in s_dalCall.ReadAll())
            {
                if (item.CallId == id)
                {
                    s_dalCall?.Update(item);
                    Console.WriteLine("Call updated.");
                    callFound = true;
                    break;
                }
            }
            if (!callFound)
            {
                Console.WriteLine("Call not found.");
            }
        }


        /// <summary>
        /// Delete a call by ID 
        /// </summary>
        private static void DeleteCall()
        {
            int id;
            Console.Write("Enter call ID to delete: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }
            s_dalCall?.Delete(id);
            Console.WriteLine("Call deleted.");
        }

        

        /// <summary>
        /// Add a new assignment
        /// </summary>
        private static void AddAssignment()
        {
            //id
            Console.Write("Enter Id: ");
            int id = int.Parse(Console.ReadLine()!);
            //callId
            Console.Write("Enter CallId: ");
            int callid = int.Parse(Console.ReadLine()!);
            //VolunteerId
            Console.Write("Enter VolunteerId: ");
            int volunteerid = int.Parse(Console.ReadLine()!);

            //opentime
            Console.WriteLine("Enter the time you want to start :");
            int year = int.Parse(Console.ReadLine()!);
            int month = int.Parse(Console.ReadLine()!);
            int day = int.Parse(Console.ReadLine()!);
            DateTime specificDate = new DateTime(year, month, day);
            //type of end
            Console.WriteLine("Enter the type of end");
            TypeOfEnd end = (TypeOfEnd)int.Parse(Console.ReadLine()!);
            //maxtime
            Console.WriteLine("Enter the latest time you want to start");
            int year2 = int.Parse(Console.ReadLine()!);
            int month2 = int.Parse(Console.ReadLine()!);
            int day2 = int.Parse(Console.ReadLine()!);
            DateTime specificDate2 = new DateTime(year2, month2, day2);

            Assignment a = new Assignment { Id = id, CallId = callid, VolunteerId = volunteerid!, StartTime = specificDate, TypeOfEnd = end, FinishTime = specificDate2 };
            Console.WriteLine("Student added.");
        }

        /// <summary>
        /// Display an assignment by ID 
        /// Prompts the user to enter a valid assignment ID, then retrieves and displays the assignment if found, or shows an "Assignment not found" message.
        /// </summary>
        private static void DisplayAssignment()
        {
            int id;
            Console.Write("Enter assignment ID: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }
            var assignment = s_dalAssignment?.Read(id);
            Console.WriteLine(assignment != null ? assignment.ToString() : "Assignment not found.");
        }

        /// <summary>
        /// Display all assignments
        /// </summary>
        private static void DisplayAllAssignments()
        {
            if (s_dalAssignment != null)
            {
                foreach (var asi in s_dalAssignment.ReadAll())
                {
                    Console.WriteLine(asi);
                }
            }
        }

        /// <summary>
        /// Update a assignment's details 
        /// Prompts the user for an ID, validates it, and updates the assignment if found.
        /// Displays a message indicating whether the update was successful or if the assignment was not found.
        /// </summary>
        private static void UpdateAssignment()
        {
            int id;
            Console.WriteLine("Enter ID:");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }

            bool assignmentFound = false;
            foreach (var item in s_dalAssignment.ReadAll())
            {
                if (item.Id == id)
                {
                    s_dalAssignment?.Update(item);
                    Console.WriteLine("Assignment updated.");
                    assignmentFound = true;
                    break;
                }
            }
            if (!assignmentFound)
            {
                Console.WriteLine("Assignment not found.");
            }
        }


        /// <summary>
        /// Delete a call by ID 
        /// Prompts the user to enter a valid assignment ID, validates it, and deletes the assignment if the ID is valid. 
        /// Displays a confirmation message upon deletion.
        /// </summary>
        private static void DeleteAssignment()
        {
            int id;
            Console.Write("Enter assignment ID to delete: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid ID format. Please enter a valid integer ID:");
            }
            s_dalAssignment?.Delete(id);
            Console.WriteLine("Assignment deleted.");
        }

    }
}



