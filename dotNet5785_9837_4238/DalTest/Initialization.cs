namespace DalTest;

using DalApi;
using DO;
using System;

/// <summary>
/// Initialization of the elements
/// </summary>
public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    /// <summary>
    /// createVolunteers function : 14 volunteers and 1 manager
    /// </summary>
    private static void createVolunteers()
    {
        // arrays with  names , mails,  adresses of the volunteers
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
    "Yaffo Street 123, Jerusalem",
    "King David Street 45, Jerusalem",
    "Prophets Street 67, Jerusalem",
    "Herzl Boulevard 89, Jerusalem",
    "Gaza Street 101, Jerusalem",
    "Emek Refaim Street 12, Jerusalem",
    "Hebron Road 34, Jerusalem",
    "Agripas Street 56, Jerusalem",
    "Ben Yehuda Street 78, Jerusalem",
    "Keren Hayesod Street 23, Jerusalem",
    "Shmuel Hanagid Street 15, Jerusalem",
    "Betzalel Street 42, Jerusalem",
    "Palmach Street 90, Jerusalem",
    "King George Street 33, Jerusalem",
    "Hillel Street 67, Jerusalem",
    "Ein Kerem Street 28, Jerusalem"
};


        Random random = new Random();

        int[] id = new int[]
{
979662525,
524933538,
364447862,
327598595,
565353679,
206870933,
994168219,
295677710,
245657465,
829237411,
957760242,
749113379,
117511204,
388066235,
566248829,
147142541
};

        double distance;
        string[] password = new string[]
{
    "abcd@xyz",
    "passw@rd",
    "valid@key",
    "strong@pw",
    "email@tag",
    "name@pass",
    "data@save",
    "secure@ok",
    "simple@me",
    "user@info",
    "login@now",
    "token@set",
    "value@put",
    "access@key",
    "safe@code",
    "check@yes"
};



        //creation of a random phone number, password... for a volunteer
        string[] phoneNumbers = {
    "0501234567", // קידומת 05
    "0523456789", // קידומת 05
    "0549876543", // קידומת 05
    "0535678901", // קידומת 05
    "0771234567", // קידומת 07
    "0723456789", // קידומת 07
    "031234567",  // קידומת 03 (תל אביב)
    "022345678",  // קידומת 02 (ירושלים)
    "045678901",  // קידומת 04 (חיפה)
    "099876543",  // קידומת 09 (נתניה)
    "0508765432", // קידומת 05
    "0531234567", // קידומת 05
    "0545678901", // קידומת 05
    "039876543",  // קידומת 03
    "043456789",  // קידומת 04
    "061234567"   // קידומת 06 (השרון)
};



        for (int i = 0; i < 5; i++)
        {

            distance = random.Next(0, 40000);
            s_dal.Volunteer.Create(new Volunteer(id[i], volunteersNames[i], phoneNumbers[i], volunteersMails[i],
                Role.Volunteer, Distance.AirDistance, password[i], volunteersAdress[i], distance));
        }
        for (int i = 5; i < 10; i++)
        {

            distance = random.Next(0, 40000);
            s_dal.Volunteer.Create(new Volunteer(id[i], volunteersNames[i], phoneNumbers[i],
                volunteersMails[i], Role.Volunteer, Distance.CarDistance, password[i], volunteersAdress[i], distance));
        }
        for (int i = 10; i < 15; i++)
        {

            distance = random.Next(0, 40000);
            s_dal.Volunteer.Create(new Volunteer(id[i], volunteersNames[i], phoneNumbers[i],
                volunteersMails[i], Role.Volunteer, Distance.WalkDistance, password[i], volunteersAdress[i], distance));
        }
        distance = random.Next(0, 40000);

        s_dal.Volunteer.Create(new Volunteer(id[15], volunteersNames[15], phoneNumbers[15], volunteersMails[15],
            0, 0, password[15], volunteersAdress[15], distance));


    }

    /// <summary>
    /// createCalls function : create a call with all his elements
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static void createCalls()
    {
        Random random = new Random();

        int j;
        int id = s_dal!.Config.nextCallId;

        string[] CallAddress =
        {
    "10 Jaffa Street, Jerusalem",
    "20 Emek Refaim Street, Jerusalem",
    "30 Agripas Street, Jerusalem",
    "40 Shmuel HaNavi Street, Jerusalem",
    "50 HaNevi'im Street, Jerusalem",
    "60 Gaza Street, Jerusalem",
    "4 Herzl Boulevard, Jerusalem",
    "40 Masliansky Street, Jerusalem",
    "10 Ben Yehuda Street, Jerusalem",
    "150 Hebron Road, Jerusalem",
    "5 Hillel Street, Jerusalem",
    "6 Korazin Street, Jerusalem",
    "35 King George Street, Jerusalem",
    "78 Ben Zvi Street, Jerusalem",
    "4 Devorah HaNeviah Street, Jerusalem",
    "15 Yehuda HaNasi Street, Jerusalem",
    "5 Aliash Street, Jerusalem",
    "15 Arlozorov Street, Jerusalem",
    "13 Shamai Street, Jerusalem",
    "10 Tavor Street, Jerusalem",
    "55 Ussishkin Street, Jerusalem",
    "7 Helene HaMalka Street, Jerusalem",
    "14 Lincoln Street, Jerusalem",
    "40 Zerach Street, Jerusalem",
    "22 Mea Shearim Street, Jerusalem",
    "10 King David Street, Jerusalem",
    "30 Ruth Street, Jerusalem",
    "9 Beit HaDfus Street, Jerusalem",
    "5 David Yellin Street, Jerusalem",
    "8 Shmuel HaNagid Street, Jerusalem",
    "15 HaTzanhanim Street, Jerusalem",
    "9 Rabbi Akiva Street, Jerusalem",
    "7 Greenberg Street, Jerusalem",
    "6 Henrietta Szold Street, Jerusalem",
    "5 Shmuel HaNagid Street, Jerusalem",
    "18 Beit Vagan Street, Jerusalem",
    "12 Neve Sha'anan Street, Jerusalem",
    "3 Rachel Imenu Street, Jerusalem",
    "15 Hillel Street, Jerusalem",
    "8 Bezalel Street, Jerusalem",
    "9 Kiryat Moshe Street, Jerusalem",
    "13 Pierre Koenig Street, Jerusalem",
    "24 Rabbi Herzog Street, Jerusalem",
    "5 Ein Gedi Street, Jerusalem",
    "8 HaOman Street, Jerusalem",
    "5 Ibn Shaprut Street, Jerusalem",
    "25 Bar Ilan Street, Jerusalem",
    "6 Yitzhak Kariv Street, Jerusalem",
    "17 Weizmann Street, Jerusalem",
    "9 Shmuel HaNagid Street, Jerusalem",
    "7 Ben Ze'ev Street, Jerusalem",
    "10 Rivlin Street, Jerusalem"
};
        double[] longi = new double[]
        {
         31.78130811991647, 31.76543961779154, 31.783470280009738,31.791048846509597,31.78391107912798, 31.771053788166768, 31.785777797665002, 31.817876781912197, 31.78186936361579, 31.74578724034863, 31.78068910546481, 31.78255160178496, 31.779769, 31.7798566, 31.789588, 31.755770, 31.755974, 31.781584, 31.772610, 31.781239, 31.782361, 31.780486, 31.7820255, 31.775421, 31.816719, 31.787706, 31.776943, 31.7765005, 31.785952, 31.786426, 31.780594, 31.779808, 31.816370, 31.779226, 31.817092, 31.757365, 31.780274, 31.771251, 31.771493, 31.763331, 31.780627, 31.780728, 31.786061, 31.756843, 31.769767, 31.753902, 31.748810, 31.776448, 31.794707, 31.777475, 31.790191, 31.779852, 31.780946

        };
        double[] lati = new double[]
        {
         35.22119546257982, 35.22123577032626,  35.215657362641764,  35.224665335668945, 35.221665622205705, 35.21205714918638, 35.19764122028971, 35.18968763754008, 35.21774235102746, 35.21658163568471, 35.21638617800225, 35.209312878017535, 35.2159379, 35.2092867, 35.224894, 35.202146, 35.201317, 35.215236, 35.216455, 35.218053, 35.213336, 35.212081, 35.221056, 35.219964, 35.190149, 35.222337, 35.222155, 35.217356, 35.189180, 35.216914, 35.214816, 35.226296, 35.192176, 35.218002, 35.191500, 35.160694, 35.214955, 35.183917, 35.200909, 35.218213, 35.217068, 35.214544, 35.195686, 35.215021, 35.208627, 35.221349, 35.211373, 35.211675, 35.218078, 35.224944, 35.198722, 35.214792, 35.220409

        };


        //creation of 45 calls
        for (j = 0; j < 45; j++)
        {
            CallType callType = s_rand.Next(1, 9) switch
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

                _ => throw new DalInvalidValueException("Invalid value in call")
            };
            Call c = new Call(id, callType, CallAddress[j], lati[j], longi[j], s_dal.Config.Clock);
            s_dal!.Call.Create(c);
        }

        //creation of 5 calls already expired
        for (j = 45; j < 50; j++)
        {
            CallType callType = s_rand.Next(1, 9) switch
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

                _ => throw new DalInvalidValueException("Invalid value in call")
            };
            Call c = new Call(id, callType, CallAddress[j], lati[j], longi[j], s_dal.Config!.Clock.AddSeconds(-5));
            s_dal!.Call.Create(c);
        }


    }

    /// <summary>
    /// create assignment : we had 50 calls , for each type of end, so we create 200 assignments
    /// </summary>

    private static void createAssignments()
    {

        int[] volunteerId = new int[]
{
979662525,
524933538,
364447862,
327598595,
565353679,
206870933,
994168219,
295677710,
245657465,
829237411,
957760242,
749113379,
117511204,
388066235,
566248829,
147142541
};
        int i;
        int callId = s_dal!.Config.nextCallId;
        int assignmentId = s_dal.Config.nextAsignmentId;
        //short explanation : for each type of end we separated in several for , like that each volunteer receives a different num of calls
        //creation of 50 assignments who are fulfilled
        for (i = 0; i < 15; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i], TypeOfEnd.Fulfilled);
            s_dal!.Assignment.Create(a);
        }
        for (i = 15; i < 29; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 15], TypeOfEnd.Fulfilled);
            s_dal!.Assignment.Create(a);
        }
        for (i = 29; i < 40; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 29], TypeOfEnd.Fulfilled);
            s_dal!.Assignment.Create(a);
        }
        for (i = 40; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 40], TypeOfEnd.Fulfilled);
            s_dal!.Assignment.Create(a);
        }

        //creation of 50 assignments who are CancelledAfterTime

        for (i = 0; i < 15; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i], TypeOfEnd.CancelledAfterTime);
            s_dal!.Assignment.Create(a);
        }
        for (i = 15; i < 29; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 15], TypeOfEnd.CancelledAfterTime);
            s_dal!.Assignment.Create(a);
        }
        for (i = 29; i < 40; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 29], TypeOfEnd.CancelledAfterTime);
            s_dal!.Assignment.Create(a);
        }
        for (i = 40; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 40], TypeOfEnd.CancelledAfterTime);
            s_dal!.Assignment.Create(a);
        }

        //creation of 50 assignments who are CancelledByManager

        for (i = 0; i < 15; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i], TypeOfEnd.CancelledByManager);
            s_dal!.Assignment.Create(a);
        }
        for (i = 15; i < 29; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 15], TypeOfEnd.CancelledByManager);
            s_dal!.Assignment.Create(a);
        }
        for (i = 29; i < 40; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 29], TypeOfEnd.CancelledByManager);
            s_dal!.Assignment.Create(a);
        }
        for (i = 40; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 40], TypeOfEnd.CancelledByManager);
            s_dal!.Assignment.Create(a);
        }


        //creation of 50 assignments who are CancelledByVolunteer

        for (i = 0; i < 15; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i], TypeOfEnd.CancelledByVolunteer);
            s_dal!.Assignment.Create(a);
        }
        for (i = 15; i < 29; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 15], TypeOfEnd.CancelledByVolunteer);
            s_dal!.Assignment.Create(a);
        }
        for (i = 29; i < 40; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 29], TypeOfEnd.CancelledByVolunteer);
            s_dal!.Assignment.Create(a);
        }
        for (i = 40; i < 50; i++)
        {
            Assignment a = new Assignment(assignmentId, callId, s_dal!.Config.Clock, volunteerId[i - 40], TypeOfEnd.CancelledByVolunteer);
            s_dal!.Assignment.Create(a);
        }
    }

    /// <summary>
    /// /// <summary>
    /// Initializes the data source by verifying non-null DAL instances, 
    /// resetting configuration values, clearing all data lists, 
    /// and populating them with initial data.
    /// <param name="dalVolunteer"></param>
    /// <param name="dalCall"></param>
    /// <param name="dalAssignment"></param>
    /// <param name="dalConfig"></param>
    /// <exception cref="NullReferenceException"></exception>
    ///  </summary>
    public static void Do()
    {

        //s_dal = dal ?? throw new DalArgumentNullException("DAL object can not be null!");//stage 2
        s_dal = DalApi.Factory.Get; //stage 4

        //reset everything
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.resetDB();



        //create news
        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteers();

        Console.WriteLine("Initializing calls list ...");
        createCalls();

        Console.WriteLine("Initializing Assignments list ...");
        createAssignments();

    }

}


