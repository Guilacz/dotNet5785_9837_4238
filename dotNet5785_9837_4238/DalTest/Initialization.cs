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
        string[] volunteersNames =
            { "Efrati Amar", "Chani Nadler" , "Shulamit Reches", "Ivy Kidron" , "Esteria Eisenberg" , "Yafit Maayan", "Dan Zilberstein" };

        string[] volunteersMails =
            { "efrati@gmail.com", "chani@gmail.com" , "shulamit@gmail.com", "ivy@gmail.com" , "esteria@gmail.com" , "yafit@gmail.com", "dan@gmail.com" };

        string[] volunteersAdress =
            {
               "רחוב יפו 123, ירושלים",
               "רחוב המלך דוד 45, ירושלים",
               "רחוב הנביאים 67, ירושלים",
               "שדרות הרצל 89, ירושלים",
               "רחוב עזה 101, ירושלים",
               "רחוב עמק רפאים 12, ירושלים",
               "רחוב דרך חברון 34, ירושלים"
            };

        Random random = new Random();

        int id = 100000000;
        string phone;
        string prefix;
        int middlePart;
        double distance;
        string password = "";

        for (int k = 0; k < 7; k++)
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

            Volunteer v = new Volunteer(id, volunteersNames[k], phone, volunteersMails[k], Role.Volunteer, 0, password, volunteersAdress[k]);

            s_dalVolunteer!.Create(v);
            id++;

        }

    }


    private static void createCalls()
    {

        string[] CallsAddress =
            {
                "רחוב הנביאים 43, ירושלים",
                "רחוב שלומציון המלכה 18, ירושלים",
                "רחוב קרן היסוד 34, ירושלים",
                "רחוב דרך בית לחם 74, ירושלים",
                "רחוב הנשיא 5, ירושלים",
                "רחוב בצלאל 7, ירושלים",
                "רחוב הלני המלכה 16, ירושלים"
            };


        for (int j = 0; j < 7; j++)
        {
            Call c = new Call(j + 1, 0, CallsAddress[j], 0, 0, s_dalConfig!.Clock); // we didnt put the 2 last variables because they can be null
            s_dalCall!.Create(c);
        }
    }


    private static void createAssignments()
    {
        //we need to write only VolunteerId because the others, we create them with numbers 
        int VolunteerId = 100000000;
        
        for (int i = 0; i < 7; i++)
        {
            Assignment a = new Assignment(i+1, i+1, VolunteerId +i , s_dalConfig!.Clock);
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


