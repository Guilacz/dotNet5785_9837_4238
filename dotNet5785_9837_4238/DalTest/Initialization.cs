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


        foreach (var name in volunteersNames)
        {
            int i = 0;
            int id;
            string phone;
            string prefix;
            int middlePart;
            double distance;
            string password = "";
            do
                id = s_rand.Next(000000001, 329999999);
            while (s_dalVolunteer!.Read(id) != null);


            prefix = "05" + random.Next(2, 9);
            middlePart = random.Next(1000000, 10000000);
            phone = $"{prefix}-{middlePart}";

            distance = random.Next(0, 40000);
            

            for (int j = 0; j < 7; j++)
            {
                int digit = random.Next(0, 10);
                password += digit.ToString();
            }

            Volunteer v = new Volunteer(id, volunteersNames[i], phone, volunteersMails[i], Role.Volunteer, 0, password, volunteersAdress[i]);
            //DateTime start = new DateTime(1995, 1, 1);
            //DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            s_dalVolunteer.Create(v);
            i++;
        }
    }


}
