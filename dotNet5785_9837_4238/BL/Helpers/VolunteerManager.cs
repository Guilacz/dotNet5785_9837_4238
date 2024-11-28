using DalApi;

namespace Helpers;

internal class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
                                             //internal static

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


}
