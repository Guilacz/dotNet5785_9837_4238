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

            Volunteer vol = new Volunteer(id, volunteersNames[k], phone, volunteersMails[k], Role.Volunteer, 0, password, volunteersAdress[k], distance);

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
        Volunteer v = new Volunteer(id, volunteersNames[15], phone, volunteersMails[15], 0, 0, password, volunteersAdress[15], distance);


    }


    private static void createCalls()
    {
        Random random = new Random();

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
    "שדרות הרצל 4, ירושלים",
    "רחוב מסליאנסקי 40, ירושלים",
    "רחוב בן יהודה 10, ירושלים",
    "רחוב דרך חברון 150, ירושלים",
    "רחוב הלל 5, ירושלים",
    "רחוב כורזין 6, ירושלים",
    "רחוב המלך ג'ורג' 35, ירושלים",
    "רחוב בן צבי 78, ירושלים",
    "רחוב דבורה הנביאה 4, ירושלים",
    "רחוב יהודה הנשיא 15, ירושלים",
    "רחוב עליאש 5, ירושלים",
    "רחוב ארלוזורוב 15, ירושלים",
    "רחוב שמאי 13, ירושלים",
    "רחוב תבור 10, ירושלים",
    "רחוב אוסישקין 55, ירושלים",
    "רחוב הלני המלכה 7, ירושלים",
    "רחוב לינקולן 14, ירושלים",
    "רחוב זרחי 40, ירושלים",
    "רחוב מאה שערים 22, ירושלים",
    "רחוב קינג דייויד 10, ירושלים",
    "רחוב רות 30, ירושלים",
    "רחוב בית הדפוס 9, ירושלים",
    "רחוב דוד ילין 5, ירושלים",
    "רחוב שמואל הנגיד 8, ירושלים",
    "רחוב הצנחנים 15, ירושלים",
    "רחוב בן זאב 7, ירושלים",
    "רחוב רבי עקיבא 9, ירושלים",
    "רחוב גרינברג 7, ירושלים",
    "רחוב הנרייטה סולד 6, ירושלים",
    "רחוב שמואל הנגיד 5, ירושלים",
    "רחוב בית וגן 18, ירושלים",
    "רחוב נווה שאנן 12, ירושלים",
    "רחוב רחל אמנו 3, ירושלים",
    "רחוב הלל 15, ירושלים",
    "רחוב בצלאל 8, ירושלים",
    "רחוב קרית משה 9, ירושלים",
    "רחוב פייר קניג 13, ירושלים",
    "רחוב הרב הרצוג 24, ירושלים",
    "רחוב עין גדי 5, ירושלים",
    "רחוב האומן 8, ירושלים",
    "רחוב אבן שפרוט 5, ירושלים",
    "רחוב בר אילן 25, ירושלים",
    "רחוב יצחק קריב 6, ירושלים",
    "רחוב ויצמן 17, ירושלים",
    "רחוב שמואל הנגיד 9, ירושלים",
    "רחוב ריבלין 10, ירושלים"
};
        double[] longi = new double[]
        {
         31.78130811991647, 31.76543961779154, 31.783470280009738,31.791048846509597,31.78391107912798, 31.771053788166768, 31.785777797665002, 31.817876781912197, 31.78186936361579, 31.74578724034863, 31.78068910546481, 31.78255160178496, 31.779769, 31.7798566, 31.789588, 31.755770, 31.755974, 31.781584, 31.772610, 31.781239, 31.782361, 31.780486, 31.7820255, 31.775421, 31.816719, 31.787706, 31.776943, 31.7765005, 31.785952, 31.786426, 31.780594, 31.779808, 31.816370, 31.779226, 31.817092, 31.757365, 31.780274, 31.771251, 31.771493, 31.763331, 31.780627, 31.780728, 31.786061, 31.756843, 31.769767, 31.753902, 31.748810, 31.776448, 31.794707, 31.777475, 31.790191, 31.779852, 31.780946

        };
        double[] lati = new double[]
        {
         35.22119546257982, 35.22123577032626,  35.215657362641764,  35.224665335668945, 35.221665622205705, 35.21205714918638, 35.19764122028971, 35.18968763754008, 35.21774235102746, 35.21658163568471, 35.21638617800225, 35.209312878017535, 35.2159379, 35.2092867, 35.224894, 35.202146, 35.201317, 35.215236, 35.216455, 35.218053, 35.213336, 35.212081, 35.221056, 35.219964, 35.190149, 35.222337, 35.222155, 35.217356, 35.189180, 35.216914, 35.214816, 35.226296, 35.192176, 35.218002, 35.191500, 35.160694, 35.214955, 35.183917, 35.200909, 35.218213, 35.217068, 35.214544, 35.195686, 35.215021, 35.208627, 35.221349, 35.211373, 35.211675, 35.218078, 35.224944, 35.198722, 35.214792, 35.220409

        };

        for (j = 0; j < 45; j++)
        {
            CallType callType = s_rand.Next(1, 11) switch
            {
                1 => CallType.Math_Primary,
                2 => CallType.Math_Middle,
                3 => CallType.Math_High,
                4 => CallType.English_Primary,
                5 => CallType.English_Middle,
                6 => CallType.English_High,
                7 => CallType.Grammary_Primary,
                8 => CallType.Grammary_Middle,
                9 => CallType.Grammary_High,

                _ => throw new ArgumentOutOfRangeException()
            };
            Call c = new Call(id, callType, CallsAddress[j], lati[j], longi[j], s_dalConfig!.Clock); // we didnt put the 2 last variables because they can be null
            s_dalCall!.Create(c);
        }
        for (j = 45; j < 50; j++)
        {
            CallType callType = s_rand.Next(1, 11) switch
            {
                1 => CallType.Math_Primary,
                2 => CallType.Math_Middle,
                3 => CallType.Math_High,
                4 => CallType.English_Primary,
                5 => CallType.English_Middle,
                6 => CallType.English_High,
                7 => CallType.Grammary_Primary,
                8 => CallType.Grammary_Middle,
                9 => CallType.Grammary_High,

                _ => throw new ArgumentOutOfRangeException()
            };
            Call c = new Call(id, callType, CallsAddress[j], lati[j], longi[j], s_dalConfig!.Clock.AddSeconds(-5)); // we didnt put the 2 last variables because they can be null
            s_dalCall!.Create(c);
        }
    }


    private static void createAssignments()
    {
        //we need to write only VolunteerId because the others, we create them with numbers 
        int VolunteerId = 100000000;
        int callId = s_dalConfig.nextCallId;
        int assignmentId = s_dalConfig.nextAsignmentId;

        for (int i = 0; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, VolunteerId + i, s_dalConfig!.Clock, TypeOfEnd.Fulfilled);
            s_dalAssignment!.Create(a);
        }
        for (int i = 0; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, VolunteerId + i, s_dalConfig!.Clock, TypeOfEnd.CancelledAfterTime);
            s_dalAssignment!.Create(a);
        }
        for (int i = 0; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, VolunteerId + i, s_dalConfig!.Clock, TypeOfEnd.CancelledByManager);
            s_dalAssignment!.Create(a);
        }
        for (int i = 0; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, VolunteerId + i, s_dalConfig!.Clock, TypeOfEnd.CancelledByVolunteer);
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


