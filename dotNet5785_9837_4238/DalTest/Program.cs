namespace DalTest
{
    using Dal;
    using DalApi;
    

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


        // Add a new student (example of Create operation)
        private static void AddStudent()
        {
            Console.Write("Enter student name: ");
            string name = Console.ReadLine();

            Student student = new Student { Name = name };
            s_dalStudent?.Create(student);
            Console.WriteLine("Student added.");
        }

        // Display a student by ID (example of Read operation)
        private static void DisplayStudent()
        {
            Console.Write("Enter student ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var student = s_dalStudent?.Read(id);
                Console.WriteLine(student != null ? student.ToString() : "Student not found.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Display all students (example of ReadAll operation)
        private static void DisplayAllStudents()
        {
            foreach (var student in s_dalStudent?.ReadAll() ?? Array.Empty<Student>())
            {
                Console.WriteLine(student);
            }
        }

        // Update a student's details (example of Update operation)
        private static void UpdateStudent()
        {
            Console.Write("Enter student ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var student = s_dalStudent?.Read(id);
                if (student == null)
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

        // Delete a student by ID (example of Delete operation)
        private static void DeleteStudent()
        {
            Console.Write("Enter student ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dalStudent?.Delete(id);
                Console.WriteLine("Student deleted.");
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }


    }
}


