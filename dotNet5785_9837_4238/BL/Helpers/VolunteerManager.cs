namespace Helpers;

using BO;
using DalApi;
using System;
using System.Text.RegularExpressions;

internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
                                             
    internal static bool CheckVolunteer(BO.Volunteer vol)
    {
        if(!CheckMail(vol.Email))
            return false;
        if(!IsValidID(vol.VolunteerId))
            return false;
        if(!CheckPhone(vol.Phone)) 
            return false;
        if (!Tools.CheckAddressVolunteer(vol))
        {
            return false;
        }
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

        int atCount = email.Count(c => c == '@');
        if (atCount != 1)
            return false;

        string[] validDomains = { ".com", ".il", ".net", ".org" };
        bool hasValidDomain = validDomains.Any(domain => email.EndsWith(domain));
        if (!hasValidDomain)
            return false;

        if (email.StartsWith("@") || email.StartsWith(".") || email.EndsWith("."))
            return false;

        int atIndex = email.IndexOf('@');
        int dotIndex = email.IndexOf('.', atIndex); 
        if (dotIndex == -1 || dotIndex <= atIndex + 1) 
            return false;

        // If all conditions pass, the email is valid
        return true;
    }




    public static BO.Volunteer ConvertVolToBO(DO.Volunteer volunteer)
    {
        return new BO.Volunteer
        {
            VolunteerId = volunteer.VolunteerId,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            RoleType = (BO.Role)volunteer.RoleType,
            DistanceType = (BO.DistanceType)volunteer.DistanceType,
            Password = volunteer.Password,
            Adress = volunteer.Adress,
            Distance = volunteer.Distance
        };
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
    public static DO.Volunteer DOManeger(BO.Volunteer vol)
    {
        int Id = vol.VolunteerId;
        string FullName = vol.Name;
        string Phone = vol.Phone;
        string Email = vol.Email;
        DO.Role Role = (DO.Role)vol.RoleType;
        DO.Distance DistanceType = (DO.Distance)vol.DistanceType;
        bool Active = vol.IsActive;
        string? Password = vol.Password;
        string? Address = vol.Adress;
        double? Distance = vol.Distance;
        double? Latitude = vol.Latitude;
        double? Longitude = vol.Longitude;
        return new DO.Volunteer(Id, FullName, Phone, Email, Role, DistanceType, Password, Address,Distance, Latitude, Longitude, Active);
    }
    public static DO.Volunteer DOVolunteer(DO.Volunteer vol1, BO.Volunteer vol)
    {
        int Id = vol.VolunteerId;
        string FullName = vol.Name;
        string Phone = vol.Phone;
        string Email = vol.Email;
        DO.Role Role = (DO.Role)vol1.RoleType;
        DO.Distance DistanceType = (DO.Distance)vol.DistanceType;
        bool Active = vol1.IsActive;
        string? Password = vol.Password;
        string? Adress = vol.Adress;
        double? MaxDistance = vol.Distance;
        double? Latitude = vol.Latitude;
        double? Longitude = vol.Longitude;
        return new DO.Volunteer(Id, FullName, Phone, Email, Role, DistanceType, Password, Adress, MaxDistance, Latitude, Longitude, Active);

    }
    public static bool CheckPassword(string password)
    {
        if (password == null) 
            return false;
        if (password.Length <8)
            return false;
        if (!Regex.IsMatch(password, @"^[a-z@.]+$"))
            return false;
        if (!password.Contains('@') || password.Contains('.'))
            return false;
        return true;
    }
    /// <summary>
    /// encryption function : applies the ATBASH encryption + shift 2
    /// </summary>

    public static string EncryptPassword(string password)
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

    public static string DecryptPassword(string password)
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

    public static void UpdateExpiredCalls()
    {
        try
        {

        }
        catch (Exception ex)
        {
            // טיפול בשגיאות (לוגיקה או בעיות חיבור לבסיס נתונים)
            throw new Exception($"Error while updating expired calls: {ex.Message}");
        }
    }
}
