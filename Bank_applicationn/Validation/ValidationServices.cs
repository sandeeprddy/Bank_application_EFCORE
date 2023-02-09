using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AllServices;
using DataAcessLayer;
using Models;
using Models.Enum_Types;

namespace Bank_Application
{
    public class ValidationServices
    {

        public static bool ValidateAdmin(string adminId, string adminPassword)
        {
            return (adminId == "admin" && adminPassword == "admin");
        }

        public static bool ValidateCustomer(string bankName, string customerID, string password)
        {
            using BankDBContext context = new();

            if (context.Users.Any(user => user.Id == customerID))
            {
                List<User> users = (context.Users.Where(user => user.Id == customerID)).ToList();
                return (users[0].Password == password && users[0].UserType.Equals(EnumTypes.UserTypes.Customer));
            }
            return false;
        }

        public static bool ValidateAccount(string customerId, string accountId)
        {
            using BankDBContext context = new();

            if (context.Accounts.Any(account => account.Id == accountId))
            {
               List<Account> accounts = (context.Accounts.Where(user => user.Id == accountId)).ToList();
                return (accounts[0].UserId == customerId);
            }
            return false;
        }


        public static Boolean ValidateStaff(string Id, string password, string staffBankName)
        {
            using BankDBContext context = new();

            string staffBankId = AdminServices.GetBank(staffBankName).Id;

            if (context.Users.Any(user => user.Id == Id))
            {
                List<User> users = (context.Users.Where(user => user.Id == Id)).ToList();
                return (users[0].BankId == staffBankId && users[0].Password == password && users[0].UserType.Equals(EnumTypes.UserTypes.Staff));
            }
            return false;
        }


        public static Boolean EmailValidator(string email)
        {

            Regex patternForEmailValidation = new("^[a-z0-9]+@([-a-z0-9]+.)+[a-z]{2,5}$");
            return (patternForEmailValidation.IsMatch(email));
        }


        public static Boolean PasswordValidator(string password)
        {
            Regex PatternForPasswordValidation = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$");
            return PatternForPasswordValidation.IsMatch(password);
        }


        public static bool ValidateBankCurrency(string bankId, string currencyCode)
        {
            using BankDBContext context = new();
            return (context.BankCurrencies.Any(rec => (rec.BankId == bankId && rec.CurrencyCode == currencyCode)));

        }

        public static bool ValidateTransaction(string accountId, string transactionId)
        {
            using BankDBContext context = new();
            return context.Transactions.Any(transaction => (transaction.Id == transactionId && transaction.AccountId == accountId));
        }

    }
}
