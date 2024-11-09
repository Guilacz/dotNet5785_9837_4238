namespace DalTest
{
    using Dal;
    using DalApi;
    using DO;
    using System.Threading.Channels;

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


        // Add a new volunteer
        private static void AddVolunteer()
        {
            Console.Write("Enter volunteer id: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Enter volunteer name: ");
            string name = Console.ReadLine();
            Console.Write("Enter volunteer phone: ");
            string phone = Console.ReadLine();
            Console.Write("Enter volunteer email: ");
            string email = Console.ReadLine();
            Console.Write("Enter 1 for manager and 2 for volunteer: ");
            Role role = (Role)int.Parse(Console.ReadLine());
            Console.Write("Enter 1 forAirDistance, 2 for WalkDistance,and 3 for CarDistance: ");
            Distance distance = (Distance)int.Parse(Console.ReadLine());
            Console.Write("Enter volunteer password: ");
            string password = Console.ReadLine();
            Console.Write("Enter volunteer address: ");
            string address = Console.ReadLine();

            Volunteer v = new Volunteer { Name = name, Phone = phone, Email = email, RoleType = role, DistanceType = distance, Password = password, Adress = address, };
            s_dalVolunteer?.Create(v);
            Console.WriteLine("Volunteer added.");


        }

        // Display a volunteer by ID
        private static void DisplayVolunteer()
        {
            Console.Write("Enter volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var volunteer = s_dalVolunteer?.Read(id);
                Console.WriteLine(volunteer != null ? volunteer.ToString() : "Volunteer not found.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Display all volunteer
        private static void DisplayAllVolunteer()
        {
            foreach (var volunteer in s_dalVolunteer?.ReadAll() ?? Array.Empty<Volunteer>())
            {
                Console.WriteLine(volunteer);
            }
        }

        // Update a volunteer's details
        private static void UpdateVolunteer()
        {
            Console.Write("Enter volunteer ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var volunteer = s_dalVolunteer?.Read(id);
                //s_dalVolunteer?.Delete(id);
                if (volunteer == null)
                {
                    Console.WriteLine("Volunteer not found.");
                    return;
                }

                Console.WriteLine("Current details: " + volunteer);

                Console.Write("Enter new name (leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrEmpty(newName))
                {
                    volunteer.Name = newName;
                }

                Console.Write("Enter new phone (leave blank to keep current): ");
                string newPhone = Console.ReadLine();
                if (!string.IsNullOrEmpty(newPhone))
                {
                    volunteer.Phone = newPhone;
                }

                Console.Write("Enter new email (leave blank to keep current): ");
                string newEmail = Console.ReadLine();
                if (!string.IsNullOrEmpty(newEmail))
                {
                    volunteer.Email = newEmail;
                }

                Console.Write("Enter new role (1 for Manager, 2 for Volunteer, leave blank to keep current): ");
                if (int.TryParse(Console.ReadLine(), out int roleInput) && Enum.IsDefined(typeof(Role), roleInput))
                {
                    volunteer.RoleType = (Role)roleInput;
                }

                Console.Write("Enter new distance type (1 for AirDistance, 2 for WalkDistance, 3 for CarDistance, leave blank to keep current): ");
                if (int.TryParse(Console.ReadLine(), out int distanceInput) && Enum.IsDefined(typeof(Distance), distanceInput))
                {
                    volunteer.DistanceType = (Distance)distanceInput;
                }

                Console.Write("Enter new password (leave blank to keep current): ");
                string newPassword = Console.ReadLine();
                if (!string.IsNullOrEmpty(newPassword))
                {
                    volunteer.Password = newPassword;
                }

                Console.Write("Enter new address (leave blank to keep current): ");
                string newAddress = Console.ReadLine();
                if (!string.IsNullOrEmpty(newAddress))
                {
                    volunteer.Address = newAddress;
                }

                Console.Write("Enter new distance (leave blank to keep current): ");
                if (double.TryParse(Console.ReadLine(), out double newDistance))
                {
                    volunteer.Distance = newDistance;
                }

                try
                {
                    s_dalVolunteer?.Update(volunteer);
                    Console.WriteLine("Volunteer updated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating volunteer: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }



        // Delete a volunteer by ID (example of Delete operation)
        private static void DeleteVolunteer()
        {
            Console.Write("Enter volunteer ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dalVolunteer?.Delete(id);
                Console.WriteLine("Volunteer deleted.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
    }
}


