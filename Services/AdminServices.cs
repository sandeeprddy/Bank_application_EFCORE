using DataAcessLayer;
using Models;

namespace AllServices
{
    public class AdminServices
    {
        public static void ChangeDefaultCurrency(string bankId,string currencyCode)
        {
            using BankDBContext context = new();

            List<Bank> banks = context.Banks.Where(bank => bank.Id == bankId).ToList();

            banks[0].Currency = currencyCode;

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

            context.BankCurrencies.Add(new BankCurrency(newBank.Id,"INR"));
            context.BankCurrencies.Add(new BankCurrency(newBank.Id, "EUR"));
            context.BankCurrencies.Add(new BankCurrency(newBank.Id,"USD"));

            context.SaveChanges();
        }

        public static void InitializeDefaultValuesForCurrencies()
        {
            using BankDBContext context = new();

            context.Currencies.Add(new Currency("INR", 0));
            context.Currencies.Add(new Currency("EUR", 90));
            context.Currencies.Add(new Currency("USD", 80));

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

            
            using BankDBContext context = new();

            /*List<Bank> banks = context.Banks.Where(bank => bank.Id == bankId).ToList();
*/
             return  (context.BankCurrencies.Where(a => a.BankId == bankId).Select(b => b.CurrencyCode)).ToList();
        }
       
        public static List<User> GetAllStaff(string bankName)
        {
            Bank bank = GetBank(bankName);

            string bankId = bank.Id;

            BankDBContext context = new BankDBContext();

            List<User> allStaff = context.Users.Where(user => (user.BankId == bankId && user.UserType.Equals(Models.Enum_Types.EnumTypes.UserTypes.Staff))).ToList() ;

            return allStaff;

        }

        public static Bank GetBank(string bankName)
        {
            using BankDBContext context = new();
            List<Bank> banks = (context.Banks.Where(bank => bank.Name == bankName.ToUpper())).ToList();
            return banks[0];
        }

        public static User GetOneStaff(string staffId)
        {
            using BankDBContext context = new();
            return  ((context.Users.Where(user => user.Id == staffId)).ToList())[0];
        }

    }
}
