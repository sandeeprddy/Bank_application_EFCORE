using DataAcessLayer;
using Models;

namespace AllServices
{
    public class AdminServices
    {
        public static void ChangeDefaultCurrency(string bankId,string currencyCode)
        {
            using BankDBContext context = new();

            Bank bank = (Bank)context.Banks.Where(bank => bank.Id == bankId); 

            bank.Currency = currencyCode;

            context.SaveChanges();

        }

        public static void AddCurrencyToAcceptedCurrencies(string targetBankId, string currencyCode, float currencyExchangeValue)
        {
            using BankDBContext context = new();

            Currency currency = new(currencyCode, currencyExchangeValue);

            context.Currencies.Add(currency);

            BankCurrency mapping = new(targetBankId, currencyCode);

            context.BankCurrencies.Add(mapping);

            context.SaveChanges();

        }

        public static void CreateBank(string name, string location)
        {
            using BankDBContext context = new();
            Bank newBank = new(name.ToUpper(), location);
            context.Banks.Add(newBank);
            context.SaveChanges();
        }

        public static bool ValidateBankInDB(string name)
        {

            using BankDBContext context = new();
            return (context.Banks.Any(bank => bank.Name == name));

        }

        public static void CreateNewStaffAccount(string firstName, string lastName, string email, string password,string bankName)
        {

            User staff = new(firstName, lastName, email, password, AdminServices.GetBank(bankName).Id);

            staff.UserType = Models.Enum_Types.EnumTypes.UserTypes.Staff;

            using BankDBContext context = new();

            context.Users.Add(staff);

            context.SaveChanges();

        }

        public static List<string> GetAcceptedCurrencies(string bankId)
        {

            Bank bank = GetBank(bankId);

            using BankDBContext context = new();

             return  (List<string>)(context.BankCurrencies.Where(a => a.BankId == bankId).Select(b => b.CurrencyCode));
        }
       
        public static List<User> GetAllStaff(string bankName)
        {
            Bank bank = GetBank(bankName);

            string bankId = bank.Id;

            BankDBContext context = new BankDBContext();

            List<User> allStaff = (List<User>)context.Users.Where(user => (user.Id == bankId && user.UserType.Equals(Models.Enum_Types.EnumTypes.UserTypes.Staff)));

            return allStaff;

        }

        public static Bank GetBank(string bankName)
        {
            using BankDBContext context = new();
            return (Bank)(context.Banks.Where(bank => bank.Name == bankName));
        }

        public static User GetOneStaff(string staffId)
        {
            using BankDBContext context = new();
            return (User)(context.Users.Where(user => user.Id == staffId));
        }

    }
}
