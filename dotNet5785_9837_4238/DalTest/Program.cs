namespace DalTest
{
    using Dal;
    using DalApi;
    using DO;

    public static class Program
    {
        private static IVolunteer s_dalVolunteer = new VolunteerImplementation(); // stage 1
        private static ICall s_dalCall = new CallImplementation(); // stage 1
        private static IAssignment s_dalAssignment = new AssignmentImplementation(); // stage 1
        private static IConfig s_dalConfig = new ConfigImplementation(); // stage 1

        public static void Main(string[] args)
        {
            try
            {
                while (true)
                    ShowMainMenu(); 


            }
            catch (Exception)
            {

                throw;
            }
        }

        // Method to display the main menu options
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



        
        // Method to process the user's choice from the main menu
        private static bool MainMenuChoice()
        {
            string input = Console.ReadLine();
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


        // Helper method to display a CRUD menu for any entity
        /* the function "ShowInterMenu" receives a name and a process fonction to accomplish.
        * according to what "ShowInterMenu" receives, it will lead to ProcessVolunteerMenu, ProcessAssignmentMenu or ProcesscallMenu
        */
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


        // Process menu options for Volunteer entity
        private static void ProcessVolunteerMenu()
        {
            try
            {
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddVolunteer();
                        break;
                    case "2":
                        DisplayVolunteer();
                        break;
                    case "3":
                        DisplayAllVolunteer();
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


        // Process menu options for Call entity
        private static void ProcessCallMenu()
        {
            try
            {
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddCall();
                        break;
                    case "2":
                        DisplayCall();
                        break;
                    case "3":
                        DisplayAllCall();
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

        // Process menu options for assignment entity
        private static void ProcessAssignmentMenu()
        {
            try
            {
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddAssignment();
                        break;
                    case "2":
                        DisplayAssignment();
                        break;
                    case "3":
                        DisplayAllAssignment();
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


        //choice 4 : initialization
        private static void InitializeDatabase()
        {
            Initialization.Do(s_dalVolunteer, s_dalCall, s_dalAssignment, s_dalConfig);
            Console.WriteLine("Database initialized.");
        }


        //choice 5 :Method to display all data from the database
        private static void DisplayAllData()
        {
            DisplayAllVolunteers();
            DisplayAllCalls();
            DisplayAllAssignments();
            DisplayConfig();
        }


        //choice 7 : Method to reset the database and configuration to initial state
        private static void ResetDatabaseAndConfig()
        {
            s_dalVolunteer.DeleteAll();
            s_dalCall.DeleteAll();
            s_dalAssignment.DeleteAll();
            s_dalConfig.Reset();
            Console.WriteLine("Database and configuration reset.");
        }


        // Add a new call 
        private static void AddCall()
        { 
            //id
            Console.Write("Enter CallId: ");
            int id = int.Parse(Console.ReadLine());
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
            CallType lesson = (CallType)int.Parse(Console.ReadLine());
            //adress
            Console.WriteLine("Enter an address"); 
            string address = Console.ReadLine();
            //latitude
            Console.WriteLine("Enter the latitude");
            double l1 = int.Parse(Console.ReadLine());
            //longitude
            Console.WriteLine("Enter the longitude");
            double l2 = int.Parse(Console.ReadLine());
            //opentime
            Console.WriteLine("Enter the time you want to start :");
            int hour = int.Parse(Console.ReadLine());
            int minute = int.Parse(Console.ReadLine());
            int seconde = int.Parse(Console.ReadLine());
            DateTime specificDate = new DateTime(hour, minute, seconde);
            //details
            Console.WriteLine("Enter details if needed");
            string details = (Console.ReadLine());
            //maxtime
            Console.WriteLine("Enter the latest time you want to start");
            int hour2 = int.Parse(Console.ReadLine());
            int minute2 = int.Parse(Console.ReadLine());
            int seconde2 = int.Parse(Console.ReadLine());
            DateTime specificDate2 = new DateTime(hour, minute, seconde);

            Call c = new Call {CallId = id, CallType = lesson, Adress = address!,Latitude = l1, Longitude = l2, OpenTime = specificDate, Details = details, MaxTime = specificDate2 };
            s_dalCall?.Create(c);
            Console.WriteLine("Student added.");
        }

        // Display a call by ID 
        private static void DisplayCall()
        {
            Console.Write("Enter call ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var Call = s_dalCall?.Read(id);
                Console.WriteLine(Call != null ? Call.ToString() : "call not found.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Display all calls
        private static void DisplayAllCalls()
        {
            foreach (var student in s_dalCall?.ReadAll() ?? Array.Empty<Student>())
            {
                Console.WriteLine(student);
            }
        }

        // Update a student's details (example of Update operation)
        private static void UpdateStudent()
        {
            Console.Write("Enter call ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var call = s_dalCall?.Read(id);
                if (call == null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }

                Console.WriteLine("Current details: " + student);
                Console.Write("Enter new name (leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    student.Name = newName;
                }

                s_dalStudent?.Update(student);
                Console.WriteLine("Student updated.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Delete a call by ID 
        private static void DeleteCall()
        {
            Console.Write("Enter call ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dalCall?.Delete(id);
                Console.WriteLine("Call deleted.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Add a new assignment
        private static void AddAssignment()
        {
            //id
            Console.Write("Enter Id: ");
            int id = int.Parse(Console.ReadLine());
            //callId
            Console.Write("Enter CallId: ");
            int callid = int.Parse(Console.ReadLine());
            //VolunteerId
            Console.Write("Enter VolunteerId: ");
            int volunteerid = int.Parse(Console.ReadLine());
          
            //opentime
            Console.WriteLine("Enter the time you want to start :");
            int hour = int.Parse(Console.ReadLine());
            int minute = int.Parse(Console.ReadLine());
            int seconde = int.Parse(Console.ReadLine());
            DateTime specificDate = new DateTime(hour, minute, seconde);
            //type of end
            Console.WriteLine("Enter the type of end");
            TypeOfEnd end = (TypeOfEnd)int.Parse(Console.ReadLine());
            //maxtime
            Console.WriteLine("Enter the latest time you want to start");
            int hour2 = int.Parse(Console.ReadLine());
            int minute2 = int.Parse(Console.ReadLine());
            int seconde2 = int.Parse(Console.ReadLine());
            DateTime specificDate2 = new DateTime(hour, minute, seconde);

            Assignment a = new Assignment { Id = id, CallId = callid, VolunteerId = volunteerid!, StartTime = specificDate, TypeOfEnd = end, FinishTime = specificDate2 };
            Console.WriteLine("Student added.");
        }

        // Display an assignment by ID 
        private static void DisplayAssignment()
        {
            Console.Write("Enter assignment ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var Assign = s_dalAssignment?.Read(id);
                Console.WriteLine(Assign != null ? Assign.ToString() : "Assignment not found.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Display all assignments
        private static void DisplayAllAssignments()
        {
            foreach (var assign in s_dalAssignments?.ReadAll() ?? Array.Empty<Student>())
            {
                Console.WriteLine(student);
            }
        }

        // Update a student's details 
        private static void UpdateStudent()
        {
            Console.Write("Enter call ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var call = s_dalCall?.Read(id);
                if (call == null)
                {
                    Console.WriteLine("Student not found.");
                    return;
                }

                Console.WriteLine("Current details: " + student);
                Console.Write("Enter new name (leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    student.Name = newName;
                }

                s_dalStudent?.Update(student);
                Console.WriteLine("Student updated.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Delete a call by ID 
        private static void DeleteAssignment()
        {
            Console.Write("Enter assignment ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dalAssignment?.Delete(id);
                Console.WriteLine("Assignment deleted.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }


    }
}


