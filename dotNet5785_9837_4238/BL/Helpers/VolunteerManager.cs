namespace Helpers;

using BlImplementation;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;



/// <summary>
/// Class of all the help function that we will use in the Volunteer Implementation
/// </summary>
internal static class VolunteerManager
{
    private static readonly IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();


    /// <summary>
    /// function to check the validity of a volunteer : 
    /// check his mail, id, phone, address
    /// </summary>
    /// <param name="vol"></param>
    /// <returns></returns>
    internal static bool CheckVolunteer(BO.Volunteer vol)
    {
        if (!CheckMail(vol.Email))
            return false;
        if (!IsValidID(vol.VolunteerId))
            return false;
        if (!CheckPhone(vol.Phone))
            return false;
        //if (!Tools.CheckAddressVolunteer(vol))
        //    return false;
        //    if(!CheckValidityOfPassword(vol.Password))
        //       return false;

        return true;
    }





    /// <summary>
    /// check validity of the mail
    /// if : it is null, size, strudel, domain, start, dot after strudel
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    internal static bool CheckMail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        if (email.Length < 5 || email.Length > 254)
            return false;


        string[] validDomains = { ".com", ".il", ".net", ".org", ".blablabla" };
        bool hasValidDomain = validDomains.Any(domain => email.EndsWith(domain));
        if (!hasValidDomain)
            return false;

        if (email.StartsWith("@") || email.StartsWith(".") || email.EndsWith("."))
            return false;

        int atIndex = email.IndexOf('@');
        int dotIndex = email.IndexOf('.', atIndex);
        if (dotIndex == -1 || dotIndex <= atIndex + 1)
            return false;

        return true;
    }



    /// <summary>
    /// function to convert a DO volunteer to Bo
    /// </summary>
    /// <param name="volunteer"></param>
    /// <returns></returns>
    internal static BO.Volunteer ConvertVolToBO(DO.Volunteer volunteer)
    {
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex)  //stage 7
            volunteers = s_dal.Volunteer.ReadAll();

        IEnumerable<DO.Assignment> assignment;
        lock (AdminManager.BlMutex)  //stage 7
            assignment = s_dal.Assignment.ReadAll();

        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)  //stage 7
            calls = s_dal.Call.ReadAll();

        DO.Assignment assi;
        lock (AdminManager.BlMutex)  //stage 7
            assi = assignment.LastOrDefault(a => a.VolunteerId == volunteer.VolunteerId && a.FinishTime == null);
        return new BO.Volunteer
        {
            VolunteerId = volunteer.VolunteerId,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            RoleType = (BO.Role)volunteer.RoleType,
            DistanceType = (BO.DistanceType)volunteer.DistanceType,
            Password = DecryptPassword(volunteer.Password),
            Address = volunteer.Address,
            Distance = volunteer.Distance,
            SumOfCaredCall = assignment.Count(call => call.VolunteerId == volunteer.VolunteerId && call.TypeOfEnd == DO.TypeOfEnd.Fulfilled),
            SumOfCancelledCall = assignment.Count(call => call.VolunteerId == volunteer.VolunteerId && call.TypeOfEnd == DO.TypeOfEnd.CancelledByVolunteer),
            SumOfCallExpired = assignment.Count(call => call.VolunteerId == volunteer.VolunteerId && call.TypeOfEnd == DO.TypeOfEnd.CancelledAfterTime),
            callInCaring = GetCallInProgress(assi, volunteer)
        };
    }


    internal static BO.CallInProgress? GetCallInProgress(DO.Assignment assignment, DO.Volunteer DOvoluTemp)
    {
        //DO.Call call = s_dal.Call.Read(assignment.CallId);
        //IEnumerable<DO.Assignment> assignmentss = s_dal.Assignment.ReadAll();

        //if (call == null)
        //    return null;
        //if (assignment == null)
        //    return null;

        if (assignment == null || DOvoluTemp == null)
            return null;

        // Try to read the call; return null if it doesn't exist
        DO.Call? call;
        lock (AdminManager.BlMutex)  //stage 7
            call = s_dal.Call.Read(assignment.CallId);
        if (call == null)
            return null;

        IEnumerable<DO.Assignment> assignmentss;
        lock (AdminManager.BlMutex)  //stage 7
            assignmentss = s_dal.Assignment.ReadAll();

        //BO.CallStatus status = Helpers.CallManager.GetCallStatus(call, assignmentss);
        BO.CallInProgressStatus? callInProgressStatus = GetCallInProgressStatus(call, assignmentss);

        if (callInProgressStatus == null)
            return null;
        return new BO.CallInProgress
        {
            Id = assignment.Id,
            CallId = assignment.CallId,
            CallType = (BO.CallType)call.CallType,
            Details = call.Details ?? string.Empty,
            Adress = call.Address ?? string.Empty,
            distance = (double)DOvoluTemp.Distance,
            StartTime = assignment.StartTime,
            MaxTime = call.MaxTime,
            OpenTime = call.OpenTime,
            CallInProgressStatus = (BO.CallInProgressStatus)callInProgressStatus
        };
    }
    internal static CallInProgressStatus? GetCallInProgressStatus(DO.Call call, IEnumerable<DO.Assignment> assignments)
    {
        BO.CallInListStatus currentStatus = Helpers.CallManager.GetCallStatus(call, assignments);

        switch (currentStatus)
        {
            case BO.CallInListStatus.InCare:
                return CallInProgressStatus.InCare;

            case BO.CallInListStatus.OpenAtRisk:
                var isInCare = assignments.Any(a => a.CallId == call.CallId && a.FinishTime == null);
                if (isInCare)
                {
                    return CallInProgressStatus.InCareAtRisk;
                }
                return null;

            default:
                return null;
        }
    }

    /// <summary>
    /// function wich checks the id according to the formula israelite
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    internal static bool IsValidID(int id)
    {
        if (id < 100000000 || id > 999999999)
            return false;

        int sum = 0;
        bool doubleDigit = false;

        while (id > 0)
        {
            int digit = id % 10;
            id /= 10;

            if (doubleDigit)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            doubleDigit = !doubleDigit;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    /// check phone function : check if it is not white space, check the size, 
    /// check that everything number is a number, checks the beggining of the number
    /// </summary>
    internal static bool CheckPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || (phone.Length != 9 && phone.Length != 10))
            return false;

        if (!phone.All(char.IsDigit))
            return false;

        if (phone.StartsWith("05") || phone.StartsWith("02") || phone.StartsWith("03") || phone.StartsWith("04") ||
            phone.StartsWith("09") || phone.StartsWith("06") || phone.StartsWith("07"))
            return true;


        return false;
    }

    /// <summary>
    /// function to make a bo volunteer to DO manager
    /// </summary>
    /// <param name="vol"></param>
    /// <returns></returns>
    internal static DO.Volunteer DOManeger(BO.Volunteer vol)
    {
        int Id = vol.VolunteerId;
        string FullName = vol.Name;
        string Phone = vol.Phone;
        string Email = vol.Email;

        DO.Role Role;
        lock (AdminManager.BlMutex)  //stage 7
            Role = (DO.Role)vol.RoleType;

        DO.Distance DistanceType;
        lock (AdminManager.BlMutex)  //stage 7
            DistanceType = (DO.Distance)vol.DistanceType;
        bool Active = vol.IsActive;
        string? Password = vol.Password;
        string? Address = vol.Address;
        double? Distance = vol.Distance;
        double? Latitude = vol.Latitude;
        double? Longitude = vol.Longitude;

        return new DO.Volunteer(Id, FullName, Phone, Email, Role, DistanceType, Password, Address, Distance, Latitude, Longitude, Active);
    }


    /// <summary>
    /// function to make a 
    /// </summary>
    /// <param name="vol1"></param>
    /// <param name="vol"></param>
    /// <returns></returns>
    internal static DO.Volunteer DOVolunteer(DO.Volunteer vol1, BO.Volunteer vol)
    {
        int Id = vol.VolunteerId;
        string FullName = vol.Name;
        string Phone = vol.Phone;
        string Email = vol.Email;
        DO.Role Role;
        lock (AdminManager.BlMutex)  //stage 7
            Role = (DO.Role)vol1.RoleType;
        DO.Distance DistanceType;
        lock (AdminManager.BlMutex)  //stage 7
            DistanceType = (DO.Distance)vol.DistanceType;
        bool Active = vol1.IsActive;
        string? Password = EncryptPassword(vol.Password);
        string? Adress = vol.Address;
        double? MaxDistance = vol.Distance;
        double? Latitude = vol.Latitude;
        double? Longitude = vol.Longitude;
        lock (AdminManager.BlMutex)  //stage 7
            return new DO.Volunteer(Id, FullName, Phone, Email, Role, DistanceType, Password, Adress, MaxDistance, Latitude, Longitude, Active);

    }


    /// <summary>
    /// function to check the validity and the security level of a password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    internal static bool CheckValidityOfPassword(string password)
    {
        if (password == null)
            return false;
        if (password.Length <= 8)
            return false;
        //  if (!Regex.IsMatch(password, @"^[a-z@]+$"))
        //      return false;
        //   if (!(password.Contains('@') || password.Contains('.')))
        //      return false;
        return true;
    }



    /// <summary>
    /// encryption function : applies the ATBASH encryption + shift 2
    /// </summary>
    internal static string EncryptPassword(string password)
    {
        char[] encryptedChars = new char[password.Length];
        for (int i = 0; i < password.Length; i++)
        {
            char letter = password[i];
            if (letter >= 'a' && letter <= 'z')
            {
                // atbash
                char atbashChar = (char)('a' + ('z' - letter));
                // +2 
                char finalChar = (char)(atbashChar + 2);
                if (finalChar > 'z')
                {
                    finalChar = (char)(finalChar - 26);
                }
                encryptedChars[i] = finalChar;
            }
            else
            {
                encryptedChars[i] = letter;
            }
        }
        return new string(encryptedChars);
    }


    /// <summary>
    /// decryption function : return the origin password
    /// </summary>
    internal static string DecryptPassword(string password)
    {

        char[] decryptedChars = new char[password.Length];

        for (int i = 0; i < password.Length; i++)
        {
            char letter = password[i];
            if (letter >= 'a' && letter <= 'z')
            {
                char minusChar = (char)(letter - 2);
                char finalChar = (char)('a' + ('z' - minusChar));

                if (finalChar < 'a')
                {
                    finalChar = (char)(finalChar + 26);
                }
                decryptedChars[i] = finalChar;
            }
            else
            {
                decryptedChars[i] = letter;
            }
        }
        return new string(decryptedChars);
    }



    /// <summary>
    /// simulation function
    /// 
    /// </summary>
    private static Random s_random = new Random();

    internal static void SimulateVolunteerRegistrationToCall()
    {
        List<DO.Assignment> assignmentList;
        List<DO.Volunteer> doVolunteerList;
        List<BO.Volunteer> boVolunteerList;
        List<DO.Call> doCallList;
        List<BO.Call> boCallList;

        lock (AdminManager.BlMutex)  //stage 7
            doVolunteerList = s_dal.Volunteer.ReadAll(vol => vol.IsActive == true).ToList();
        boVolunteerList = doVolunteerList.Select(vol => ConvertVolToBO(vol)).ToList();

        lock (AdminManager.BlMutex)  //stage 7
            doCallList = s_dal.Call.ReadAll().ToList();

        lock (AdminManager.BlMutex)  //stage 7
            assignmentList = s_dal.Assignment.ReadAll().ToList();



        foreach (var vol in boVolunteerList)
        {
            lock (AdminManager.BlMutex)

            {
                //if he dosent have a call in caring
                if (vol.callInCaring == null)
                {
                    IEnumerable<BO.OpenCallInList> openCallInList;
                    openCallInList = CallManager.GetListOfOpenCall(vol.VolunteerId);

                    if (openCallInList.Count() != 0)
                    {
                        //if (s_random.Next(0, 100) < 20)
                        //{
                        int randomIndex = s_random.Next(0, openCallInList.Count());
                        var selectedCall = openCallInList.ElementAt(randomIndex);
                        CallManager.ChoiceOfCallToCare(vol.VolunteerId, selectedCall.CallId);
                        
                        //}   
                    }

                }


                //if (vol.callInCaring != null)
                else
                {
                    var Docall = doCallList.FirstOrDefault(call => call.CallId == vol.callInCaring.CallId);
                    var Dovol = doVolunteerList.FirstOrDefault(vol2 => vol2.VolunteerId == vol.VolunteerId);
                    DO.Assignment callAssignment = assignmentList.FirstOrDefault(assign => assign.CallId == vol.callInCaring.CallId && assign.VolunteerId == Dovol.VolunteerId);



                    if (callAssignment != null && vol.callInCaring.MaxTime.HasValue)
                    {
                        double distance = Task.Run(() => Tools.CalculateDistanceBetweenAddresses(Dovol, Docall)).Result;
                        double estimatedTime = distance / 50.0;
                        estimatedTime += new Random().Next(0, 60);

                        if ((DateTime.Now - vol.callInCaring.MaxTime.Value).TotalHours > estimatedTime)
                        {
                            CallManager.UpdateCallFinished(vol.VolunteerId, vol.callInCaring.CallId, callAssignment.Id);
                            //vol.callInCaring.CallStatus = CallInListStatus.Closed;
                        }

                        else
                        {

                            //if (s_random.Next(0, 100) < 10) // 10% 

                            callAssignment = callAssignment with
                            {
                                // FinishTime = DateTime.Now,
                                FinishTime = null,
                            };
                            s_dal.Assignment.Update(callAssignment);
                            CallManager.UpdateCallCancelled(vol.VolunteerId, callAssignment.Id);
                            //callIncaring.CallStatus = CallInListStatus.Open;


                        }


                    }
                }
                }
                
            



        }



        //private static void functa()
        //{

        //    List<DO.Volunteer> doVolunteerList;
        //    List<BO.Volunteer> boVolunteerList;
        //    List<DO.Assignment> assignmentList;
        //    List<DO.Call> doCallList;
        //    List<BO.Call> boCallList;


        //    //take all the volunteers if they are active
        //    lock (AdminManager.BlMutex)  //stage 7
        //        doVolunteerList = s_dal.Volunteer.ReadAll(vol => vol.IsActive == true).ToList();

        //    boVolunteerList = doVolunteerList.Select(vol => ConvertVolToBO(vol)).ToList();


        //    //take all the calls if they have coordinates
        //    lock (AdminManager.BlMutex)  //stage 7
        //        doCallList = s_dal.Call.ReadAll(call => call.Latitude != null && call.Longitude != null).ToList();

        //    boCallList = doCallList.Select(call => Helpers.CallManager.ConvertCallToBO(call, s_dal)).ToList();

        //    //take all the assignments
        //    lock (AdminManager.BlMutex)  //stage 7
        //        assignmentList = s_dal.Assignment.ReadAll().ToList();




        //    List<BO.Call> callOfVol = new List<BO.Call>();
        //    //CallImplementation callImplementation = new CallImplementation();


        //    foreach (var vol in boVolunteerList)
        //    {
        //        //lock (AdminManager.BlMutex)
        //        //{
        //        //    //List<BO.Call> boCallListOpen = boCallList.Where(call => call.CallStatus == CallInListStatus.Open || call.CallStatus == CallInListStatus.OpenAtRisk).ToList();




        //        //else if he has a current call check for how long he is taking gare of it
        //        else
        //        {
        //            //getcalldetails of the (vol.callInCaring.CallId);

        //            if (vol.callInCaring != null)
        //            {
        //                BO.Call callIncaring = GetCallDetails(vol.callInCaring.CallId);
        //                if (callIncaring != null)
        //                {
        //                    var Docall = doCallList.FirstOrDefault(call => call.CallId == callIncaring.CallId);
        //                    var Dovol = doVolunteerList.FirstOrDefault(vol2 => vol2.VolunteerId == vol.VolunteerId);

        //                    //        BO.Call callIncaring = callImplementation.GetCallDetails(vol.callInCaring.CallId);
        //                    //var Docall = doCallList.FirstOrDefault(call => call.CallId == callIncaring.CallId);
        //                    //var Dovol = doVolunteerList.FirstOrDefault(vol => vol.VolunteerId == vol.VolunteerId);
        //                    //find the assignement of the call
        //                    DO.Assignment callAssignment = assignmentList.FirstOrDefault(assign => assign.CallId == callIncaring.CallId && assign.VolunteerId == Dovol.VolunteerId);


        //                    if (callAssignment != null && callIncaring.MaxTime.HasValue)
        //                    {
        //                        double distance = Task.Run(() => Tools.CalculateDistanceBetweenAddresses(Dovol, Docall)).Result;
        //                        double estimatedTime = distance / 50.0;
        //                        estimatedTime += new Random().Next(0, 60);

        //                        if ((DateTime.Now - callIncaring.MaxTime.Value).TotalHours > estimatedTime)
        //                        {
        //                            callImplementation.UpdateCallFinished(vol.VolunteerId, callIncaring.CallId, callAssignment.Id);
        //                            callIncaring.CallStatus = CallInListStatus.Closed;
        //                        }

        //                        else
        //                        {
        //                            Random random = new Random();
        //                            //if (random.Next(0, 100) < 10) // 10% 

        //                            callAssignment = callAssignment with
        //                            {
        //                                // FinishTime = DateTime.Now,
        //                                FinishTime = null,
        //                            };
        //                            callImplementation.UpdateCallCancelled(vol.VolunteerId, callAssignment.Id);
        //                            callIncaring.CallStatus = CallInListStatus.Open;


        //                        }
        //                    }
        //                }
        //            }
        //            //    }
        //            //}
        //            VolunteerManager.Observers.NotifyItemUpdated(vol.VolunteerId);
        //        }
        //        VolunteerManager.Observers.NotifyListUpdated();
        //    }

        //    //List<DO.Volunteer> doVolunteerList;
        //    //List<BO.Volunteer> boVolunteerList;
        //    //List<DO.Assignment> assignmentList;
        //    //List<DO.Call> doCallList;
        //    //List<BO.Call> boCallList;


        //    ////take all the volunteers if they are active
        //    //lock (AdminManager.BlMutex)  //stage 7
        //    //    doVolunteerList = s_dal.Volunteer.ReadAll(vol => vol.IsActive == true).ToList();

        //    //boVolunteerList = doVolunteerList.Select(vol => ConvertVolToBO(vol)).ToList();


        //    ////take all the calls if they have coordinates
        //    //lock (AdminManager.BlMutex)  //stage 7
        //    //    doCallList = s_dal.Call.ReadAll(call => call.Latitude != null && call.Longitude != null).ToList();

        //    //boCallList = doCallList.Select(call => Helpers.CallManager.ConvertCallToBO(call, s_dal)).ToList();

        //    ////take all the assignments
        //    //lock (AdminManager.BlMutex)  //stage 7
        //    //    assignmentList = s_dal.Assignment.ReadAll().ToList();




        //    //List<BO.Call> callOfVol = new List<BO.Call>();
        //    ////CallImplementation callImplementation = new CallImplementation();


        //    //foreach (var vol in boVolunteerList)
        //    //{
        //    //    //lock (AdminManager.BlMutex)
        //    //    //{
        //    //    //    //List<BO.Call> boCallListOpen = boCallList.Where(call => call.CallStatus == CallInListStatus.Open || call.CallStatus == CallInListStatus.OpenAtRisk).ToList();


        //    //    ////if the volunteer doesnt have a current call, then 
        //    //    if (vol.callInCaring == null)
        //    //    {
        //    //        //    foreach (var call in boCallListOpen)
        //    //        //    {
        //    //        //        var DOcall = doCallList.FirstOrDefault(call2 => call2.CallId == call.CallId);
        //    //        //        var DOvol = DOManeger(vol);
        //    //        //        // check if there is a call in his caring distance
        //    //        //        double distance = Task.Run(() => Tools.CalculateDistanceBetweenAddresses(DOvol, DOcall)).Result;
        //    //        //        //var BoCall = Helpers.CallManager.ConvertCallToBO(call, s_dal);
        //    //        //        //var status = Helpers.CallManager.GetCallStatus(call, assignmentList);

        //    //        //        if ((distance < vol.Distance)) //&& status!= CallInListStatus.Closed && status != CallInListStatus.Expired && status != CallInListStatus.InCare && status != CallInListStatus.InCareAtRisk)
        //    //        //        {
        //    //        //            var status = Helpers.CallManager.GetCallStatus(DOcall, assignmentList);
        //    //        //            if (status == BO.CallInListStatus.Open || status == BO.CallInListStatus.OpenAtRisk)
        //    //        //            {
        //    //        //                callOfVol.Add(call);
        //    //        //            }
        //    //        //        }
        //    //        //    }

        //    //        //callOfVol = GetListOfOpenCall(vol.VolunteerId);
        //    //        //if there is a call, add it to the volunteer
        //    //        if (callOfVol.Count != 0)
        //    //        {

        //    //            Random random = new Random();
        //    //            //if (random.Next(0, 100) < 20)
        //    //            //{

        //    //            int randomIndex = random.Next(0, callOfVol.Count);

        //    //            var selectedCall = callOfVol[randomIndex];
        //    //            callImplementation.ChoiceOfCallToCare(vol.VolunteerId, selectedCall.CallId);
        //    //            selectedCall.CallStatus = CallInListStatus.InCare;

        //    //            //boCallListOpen = boCallList.Where(call => call.CallStatus == CallInListStatus.Open || call.CallStatus == CallInListStatus.OpenAtRisk).ToList();

        //    //            //}

        //    //        }
        //    //    }

        //    //    //else if he has a current call check for how long he is taking gare of it
        //    //    else
        //    //    {
        //    //        //getcalldetails of the (vol.callInCaring.CallId);

        //    //        if (vol.callInCaring != null)
        //    //        {
        //    //            BO.Call callIncaring = callImplementation.GetCallDetails(vol.callInCaring.CallId);
        //    //            if (callIncaring != null)
        //    //            {
        //    //                var Docall = doCallList.FirstOrDefault(call => call.CallId == callIncaring.CallId);
        //    //                var Dovol = doVolunteerList.FirstOrDefault(vol2 => vol2.VolunteerId == vol.VolunteerId);

        //    //                //        BO.Call callIncaring = callImplementation.GetCallDetails(vol.callInCaring.CallId);
        //    //                //var Docall = doCallList.FirstOrDefault(call => call.CallId == callIncaring.CallId);
        //    //                //var Dovol = doVolunteerList.FirstOrDefault(vol => vol.VolunteerId == vol.VolunteerId);
        //    //                //find the assignement of the call
        //    //                DO.Assignment callAssignment = assignmentList.FirstOrDefault(assign => assign.CallId == callIncaring.CallId && assign.VolunteerId == Dovol.VolunteerId);


        //    //                if (callAssignment != null && callIncaring.MaxTime.HasValue)
        //    //                {
        //    //                    double distance = Task.Run(() => Tools.CalculateDistanceBetweenAddresses(Dovol, Docall)).Result;
        //    //                    double estimatedTime = distance / 50.0;
        //    //                    estimatedTime += new Random().Next(0, 60);

        //    //                    if ((DateTime.Now - callIncaring.MaxTime.Value).TotalHours > estimatedTime)
        //    //                    {
        //    //                        callImplementation.UpdateCallFinished(vol.VolunteerId, callIncaring.CallId, callAssignment.Id);
        //    //                        callIncaring.CallStatus = CallInListStatus.Closed;
        //    //                    }

        //    //                    else
        //    //                    {
        //    //                        Random random = new Random();
        //    //                        //if (random.Next(0, 100) < 10) // 10% 

        //    //                        callAssignment = callAssignment with
        //    //                        {
        //    //                            // FinishTime = DateTime.Now,
        //    //                            FinishTime = null,
        //    //                        };
        //    //                        callImplementation.UpdateCallCancelled(vol.VolunteerId, callAssignment.Id);
        //    //                        callIncaring.CallStatus = CallInListStatus.Open;


        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //        //    }
        //    //        //}
        //    //        VolunteerManager.Observers.NotifyItemUpdated(vol.VolunteerId);
        //    //    }
        //    //    VolunteerManager.Observers.NotifyListUpdated();
        //    //}
        //}

    }
}



