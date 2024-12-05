namespace BL;

using BO;
using System;

class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("בחר אפשרות:");
            Console.WriteLine("1. ניהול מתנדבים");
            Console.WriteLine("2. ניהול קריאות");
            Console.WriteLine("3. ניהול מערכת");
            Console.WriteLine("0. יציאה");

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
                    Console.WriteLine("בחירה לא חוקית");
                    break;
            }
        }
    }

    static void VolunteerMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("בחר אפשרות:");
            Console.WriteLine("1. הצג את רשימת המתנדבים");
            Console.WriteLine("2. הצג פרטי מתנדב");
            Console.WriteLine("3. עדכן פרטי מתנדב");
            Console.WriteLine("4. מחק מתנדב");
            Console.WriteLine("5. צור מתנדב חדש");
            Console.WriteLine("6. קרא פרטי מתנדב");
            Console.WriteLine("0. חזור לתפריט הראשי");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    s_bl.Volunteer.GetVolunteerInLists();
                    break;
                case "2":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    int volId = int.Parse(Console.ReadLine());
                    s_bl.Volunteer.GetVolunteerDetails(volId);
                    break;
                case "3":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    volId = int.Parse(Console.ReadLine());
                    Console.WriteLine("הזן פרטי מתנדב לעדכון:");
                    BO.Volunteer vol = new BO.Volunteer(); 
                    s_bl.Volunteer.Update(volId, vol);
                    break;
                case "4":
                    Console.WriteLine("הזן את מזהה המתנדב למחיקה:");
                    volId = int.Parse(Console.ReadLine());
                    s_bl.Volunteer.Delete(volId);
                    break;
                case "5":
                    Console.WriteLine("הזן פרטי מתנדב חדש:");
                    vol = new BO.Volunteer(); 
                    s_bl.Volunteer.Create(vol);
                    break;
                case "6":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    volId = int.Parse(Console.ReadLine());
                    s_bl.Volunteer.Read(volId);
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("בחירה לא חוקית");
                    break;
            }
        }
    }

    static void CallMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("בחר אפשרות:");
            Console.WriteLine("1. קבל רשימת קריאות");
            Console.WriteLine("2. קבל פרטי קריאה");
            Console.WriteLine("3. עדכן קריאה");
            Console.WriteLine("4. מחק קריאה");
            Console.WriteLine("5. צור קריאה");
            Console.WriteLine("6. קבל רשימת קריאות סגורות");
            Console.WriteLine("7. קבל רשימת קריאות פתוחות");
            Console.WriteLine("8. עדכון קריאה כטיפול הסתיים");
            Console.WriteLine("9. עדכון קריאה כמבוטלת");
            Console.WriteLine("10. בחר קריאה לטיפול");
            Console.WriteLine("0. חזור לתפריט הראשי");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    s_bl.Call.GetListOfCalls();
                    break;
                case "2":
                    Console.WriteLine("הזן את מזהה הקריאה:");
                    int callId = int.Parse(Console.ReadLine());
                    s_bl.Call.GetCallDetails(callId);
                    break;
                case "3":
                    Console.WriteLine("הזן את מזהה הקריאה:");
                    callId = int.Parse(Console.ReadLine());
                    Call c = new Call(); 
                    s_bl.Call.Update(c);
                    break;
                case "4":
                    Console.WriteLine("הזן את מזהה הקריאה למחיקה:");
                    callId = int.Parse(Console.ReadLine());
                    s_bl.Call.Delete(callId);
                    break;
                case "5":
                    c = new Call();  
                    s_bl.Call.Create(c);
                    break;
                case "6":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    int volId = int.Parse(Console.ReadLine());
                    s_bl.Call.GetListOfClosedCall(volId);
                    break;
                case "7":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    volId = int.Parse(Console.ReadLine());
                    s_bl.Call.GetListOfOpenCall(volId);
                    break;
                case "8":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    volId = int.Parse(Console.ReadLine());
                    Console.WriteLine("הזן את מזהה הקריאה:");
                    callId = int.Parse(Console.ReadLine());
                    s_bl.Call.UpdateCallFinished(volId, callId);
                    break;
                case "9":
                    Console.WriteLine("הזן את מזהה הקריאה:");
                    callId = int.Parse(Console.ReadLine());
                    Console.WriteLine("הזן את מזהה ההקצאה:");
                    int assiId = int.Parse(Console.ReadLine());
                    s_bl.Call.UpdateCallCancelled(callId, assiId);
                    break;
                case "10":
                    Console.WriteLine("הזן את מזהה המתנדב:");
                    volId = int.Parse(Console.ReadLine());
                    Console.WriteLine("הזן את מזהה הקריאה:");
                    callId = int.Parse(Console.ReadLine());
                    s_bl.Call.ChoiceOfCallToCare(volId, callId);
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("בחירה לא חוקית");
                    break;
            }
        }
    }

    static void AdminMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("בחר אפשרות:");
            Console.WriteLine("1. אתחול בסיס נתונים");
            Console.WriteLine("2. איפוס בסיס נתונים");
            Console.WriteLine("3. קבל טווח זמן מקסימלי");
            Console.WriteLine("4. עדכון טווח זמן מקסימלי");
            Console.WriteLine("5. קבל את השעון הנוכחי");
            Console.WriteLine("6. קדום את השעון");
            Console.WriteLine("0. חזור לתפריט הראשי");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    s_bl.Admin.InitializeDB();
                    break;
                case "2":
                    s_bl.Admin.ResetDB();
                    break;
                case "3":
                    s_bl.Admin.GetMaxRange();
                    break;
                case "4":
                    Console.WriteLine("הזן את הטווח המקסימלי:");
                    TimeSpan maxRange = TimeSpan.Parse(Console.ReadLine());
                    s_bl.Admin.SetMaxRange(maxRange);
                    break;
                case "5":
                    s_bl.Admin.GetClock();
                    break;
                case "6":
                    Console.WriteLine("הזן את היחידה לעדכון השעון:");
                    string unit = Console.ReadLine();
                    BO.TimeUnit timeUnit = (BO.TimeUnit)Enum.Parse(typeof(BO.TimeUnit), unit);
                    s_bl.Admin.ForwardClock(timeUnit);
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("בחירה לא חוקית");
                    break;
            }
        }
    }
}
