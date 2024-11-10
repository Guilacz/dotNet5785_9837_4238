namespace DalTest;

using DalApi;
using DO;
using System;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static ICall? s_dalCall; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1

    private static readonly Random s_rand = new();

    private static void createVolunteers()
    {
        string[] volunteersNames = {
    "Efrati Amar",
    "Chani Nadler",
    "Shulamit Reches",
    "Ivy Kidron",
    "Esteria Eisenberg",
    "Yafit Maayan",
    "Dan Zilberstein",
    "Moshe Cohen",
    "Yael Levy",
    "Noa Shapira",
    "Avi Goldman",
    "Talia Friedman",
    "Ronen Mizrachi",
    "Maya Berkovich",
    "Doron Avraham",
    "Sarah Rosenberg"
};

        string[] volunteersMails = {
    "Efrati@gmail.com",
    "Chani@gmail.com",
    "Shulamit@gmail.com",
    "Ivy@gmail.com",
    "Esteria@gmail.com",
    "Yafit@gmail.com",
    "Dan@gmail.com",
    "Moshe@gmail.com",
    "Yael@gmail.com",
    "Noa@gmail.com",
    "Avi@gmail.com",
    "Talia@gmail.com",
    "Ronen@gmail.com",
    "Maya@gmail.com",
    "Doron@gmail.com",
    "Sarah@gmail.com"
};
        string[] volunteersAdress = {
    "רחוב יפו 123, ירושלים",
    "רחוב המלך דוד 45, ירושלים",
    "רחוב הנביאים 67, ירושלים",
    "שדרות הרצל 89, ירושלים",
    "רחוב עזה 101, ירושלים",
    "רחוב עמק רפאים 12, ירושלים",
    "רחוב דרך חברון 34, ירושלים",
    "רחוב אגריפס 56, ירושלים",
    "רחוב בן יהודה 78, ירושלים",
    "רחוב קרן היסוד 23, ירושלים",
    "רחוב שמואל הנגיד 15, ירושלים",
    "רחוב בצלאל 42, ירושלים",
    "רחוב הפלמ״ח 90, ירושלים",
    "רחוב המלך ג׳ורג׳ 33, ירושלים",
    "רחוב הלל 67, ירושלים",
    "רחוב עין כרם 28, ירושלים"
};

        Random random = new Random();

        int id = 100000000;
        string phone;
        string prefix;
        int middlePart;
        double distance;
        string password = "";

        for (int k = 0; k < 15; k++)
        {
            prefix = "05" + random.Next(2, 9);
            middlePart = random.Next(1000000, 10000000);
            phone = $"{prefix}-{middlePart}";

            distance = random.Next(0, 40000);

            for (int j = 0; j < 7; j++)
            {
                int digit = random.Next(0, 10);
                password += digit.ToString();
            }

            Volunteer vol = new Volunteer(id, volunteersNames[k], phone, volunteersMails[k], Role.Volunteer, 0, password, volunteersAdress[k]);

            s_dalVolunteer!.Create(vol);
            id++;
        }
        prefix = "05" + random.Next(2, 9);
        middlePart = random.Next(1000000, 10000000);
        phone = $"{prefix}-{middlePart}";

        distance = random.Next(0, 40000);

        for (int j = 0; j < 7; j++)
        {
            int digit = random.Next(0, 10);
            password += digit.ToString();
        }
        Volunteer v = new Volunteer(id, volunteersNames[15], phone, volunteersMails[15], 0, 0, password, volunteersAdress[15]);


    }


    private static void createCalls()
    {
        int j;
        int id = s_dalConfig.nextCallId;
        string[] CallsAddress = new string[]
{
    "רחוב יפו 10, ירושלים",
    "רחוב עמק רפאים 20, ירושלים",
    "רחוב אגריפס 30, ירושלים",
    "רחוב שמואל הנביא 40, ירושלים",
    "רחוב הנביאים 50, ירושלים",
    "רחוב עזה 60, ירושלים",
    "שדרות הרצל 70, ירושלים",
    "רחוב קרן היסוד 80, ירושלים",
    "רחוב בן יהודה 90, ירושלים",
    "רחוב דרך חברון 100, ירושלים",
    "רחוב הלל 5, ירושלים",
    "רחוב כורזין 14, ירושלים",
    "רחוב המלך ג'ורג' 35, ירושלים",
    "רחוב בן צבי 78, ירושלים",
    "רחוב דבורה הנביאה 4, ירושלים",
    "רחוב יהודה הנשיא 15, ירושלים",
    "רחוב עליאש 3, ירושלים",
    "רחוב ארלוזורוב 17, ירושלים",
    "רחוב שמאי 13, ירושלים",
    "רחוב התבור 10, ירושלים",
    "רחוב אוסישקין 55, ירושלים",
    "רחוב הלני המלכה 7, ירושלים",
    "רחוב לינקולן 14, ירושלים",
    "רחוב כיכר צרפת 1, ירושלים",
    "רחוב מאה שערים 22, ירושלים",
    "רחוב קינג דייויד 23, ירושלים",
    "רחוב רות 30, ירושלים",
    "רחוב יצחק בן צבי 21, ירושלים",
    "רחוב דוד ילין 5, ירושלים",
    "רחוב שמואל הנגיד 8, ירושלים",
    "רחוב הצנחנים 15, ירושלים",
    "רחוב גבעת התחמושת 2, ירושלים",
    "רחוב רבי עקיבא 9, ירושלים",
    "רחוב קרן קימת לישראל 100, ירושלים",
    "רחוב הנרייטה סולד 6, ירושלים",
    "רחוב שמואל הנגיד 5, ירושלים",
    "רחוב דובנוב 18, ירושלים",
    "רחוב נווה שאנן 12, ירושלים",
    "רחוב רחל אמנו 3, ירושלים",
    "רחוב הלל 15, ירושלים",
    "רחוב בצלאל 8, ירושלים",
    "רחוב קרית משה 9, ירושלים",
    "רחוב פייר קניג 13, ירושלים",
    "רחוב הרב הרצוג 24, ירושלים",
    "רחוב עין גדי 5, ירושלים",
    "רחוב האומן 8, ירושלים",
    "רחוב חיים הזז 3, ירושלים",
    "רחוב בר אילן 25, ירושלים",
    "רחוב יצחק קריב 6, ירושלים",
    "רחוב ויצמן 17, ירושלים",
    "רחוב שמואל הנגיד 9, ירושלים",
    "רחוב ריבלין 10, ירושלים"
};


        for (j = 0; j < 35; j++)
        {
            Call c = new Call(id, 0, CallsAddress[j], 0, 0, s_dalConfig!.Clock); // we didnt put the 2 last variables because they can be null
            s_dalCall!.Create(c);
        }
        for (j = 35; j < 50; j++)
        {
            Call c = new Call(id, 0, CallsAddress[j], 0, 0, s_dalConfig!.Clock); // we didnt put the 2 last variables because they can be null
            s_dalCall!.Create(c);
        }
    }


    private static void createAssignments()
    {
        //we need to write only VolunteerId because the others, we create them with numbers 
        int VolunteerId = 100000000;
        int callId = s_dalConfig.nextCallId;
        int assignmentId = s_dalConfig.nextAsignmentId;

        for (int i = 0; i < 7; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, VolunteerId + i, s_dalConfig!.Clock);
            s_dalAssignment!.Create(a);
        }

    }


    public static void Do(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig) //stage 1
    {
        //check if they are null
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1


        //reset everything
        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();


        //create news
        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteers();

        Console.WriteLine("Initializing calls list ...");
        createCalls();

        Console.WriteLine("Initializing Assignments list ...");
        createAssignments();

    }

}


