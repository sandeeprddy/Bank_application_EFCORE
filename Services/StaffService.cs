
using DataAcessLayer;
using Models;

namespace AllServices
{
    public class StaffService
    {
        public static void CreateAccountForNewCustomer(string firstName, string lastName, string email, string password, string bankName)
        {

            User customer = new(firstName, lastName, email, password, AdminServices.GetBank(bankName).Id);

            customer.UserType = Models.Enum_Types.EnumTypes.UserTypes.Customer;

            using BankDBContext context = new();

            context.Users.Add(customer);

            Account account = new();

            context.Accounts.Add(account);

            context.SaveChanges();
        }

        public static void CreateAccountForExistingCustomer(string customerId)
        {

            Account newAccount = new();

            newAccount.UserId = customerId;

            using BankDBContext context = new();

            context.Accounts.Add(newAccount);

            context.SaveChanges();

        }

        public static void UpdateFirstName(string customerId, string name)
        { 
            using BankDBContext context = new();

            User user = (User)context.Users.Where(user => user.Id == customerId);
         
            user.FirstName = name;

        }

        public static void UpdateLastName(string customerId, string name)
        {
            using BankDBContext context = new();

            User user = (User)context.Users.Where(user => user.Id == customerId);

            user.LastName = name;

        }

        public static void UpdateEmail(string customerId, string email)
        {
            using BankDBContext context = new();

            User user = (User)context.Users.Where(user => user.Id == customerId);

            user.Email = email;

        }

        public static void UpdatePassword(string customerId, string password)
        {
            using BankDBContext context = new();

            User user = (User)context.Users.Where(user => user.Id == customerId);

            user.Password = password;

        }

        public static void DeleteCustomer(string customerId)
        {
            using BankDBContext context = new();

            User user = (User)context.Users.Where(user => user.Id == customerId);

            List<Account> accounts = (List<Account>)context.Accounts.Where(account => account.UserId == customerId);

            foreach (Account account in accounts)
            {
                context.Accounts.Remove(account);
            }

            context.Users.Remove(user);

            context.SaveChanges();
        }

        public static void DeleteAccount(string accountIDToDelete)
        {
            using BankDBContext context = new();
            Account account = (Account)context.Accounts.Where(account => account.Id == accountIDToDelete);
            context.Accounts.Remove(account);  
        }

        public static void AddServiceChargeForTransferringBank(string bankName, float updatedRTGSChargeToOtherBank, float updatedIMPSChargeToOtherBank)
        {
            using BankDBContext context = new BankDBContext();
            Bank bank = (Bank)context.Banks.Where(bank => bank.Name == bankName);
            bank.TransferRTGSCharge = updatedRTGSChargeToOtherBank;
            bank.TransferRTGSCharge = updatedIMPSChargeToOtherBank;
        }

        public static float GetMoneyTransferredInTransaction(string transactionId)
        {
            using BankDBContext context = new();

            Transaction transaction = (Transaction)context.Transactions.Where(transaction => transaction.Id == transactionId);

            return transaction.MoneyTransferred;

        }

        public static void AddServiceChargeForSameBank(string bankName, float updatedRTGSCharge, float updatedIMPSCharge)
        {
            using BankDBContext context = new BankDBContext();
            Bank bank = (Bank)context.Banks.Where(bank => bank.Name == bankName);
            bank.RTGSCharge = updatedRTGSCharge;
            bank.IMPSCharge = updatedIMPSCharge;
        }

        public static List<Transaction> GetTransaction(string accountId)
        {
            using BankDBContext context = new();

           List<Transaction> transactions = (List<Transaction>)context.Transactions.Where(transaction => transaction.AccountId == accountId);

            return transactions;
        }

        public static string GetReceiverTransactionID(string transactionId)
        {
            using (var context = new BankDBContext())
            {
                Transaction transaction = (Transaction)context.Transactions.Where(transaction => transaction.Id == transactionId);
                return (string)transaction.ReceiverTransactionID;
            }
        }

        public static string GetSenderAccountIdFromTransaction(string transactionId)
        {
            using (var context = new BankDBContext())
            {
               Transaction transaction = (Transaction)context.Transactions.Where(transaction => transaction.Id == transactionId);
                return transaction.SenderId;
            }
        }

        public static List<User> GetAllCustomers(string bankName)
        {
            using (var context = new BankDBContext())
            {
                string bankId = (AdminServices.GetBank(bankName)).Id;
                List<User> allCustomers = (List<User>)context.Users.Where(customer => (customer.BankId == bankId && customer.UserType.Equals(Models.Enum_Types.EnumTypes.UserTypes.Customer)));
                return allCustomers;   
            }
        }

        public static List<Account> GetAllAccounts(string customerId)
        {
            using (var context = new BankDBContext())
            {
                List<Account> allAccounts = (List<Account>)context.Accounts.Where(account => (account.UserId == customerId));
                return allAccounts;
            }
        }

        public static string GetBankNameFromAccount(string accountId)
        {
            using (var context = new BankDBContext())
            {
              Account account = (Account)context.Accounts.Where(account => account.Id == accountId);

              User customer = (User)context.Users.Where(user => user.Id == account.UserId);

                Bank bank = AdminServices.GetBank(customer.BankId);

                return bank.Name;

            }
             
        }

        public static bool ValidateAmountForDebit(string accountId, float amountToWithdraw)
        {

            float balance = CustomerService.GetBalance(accountId);
            return (balance >= amountToWithdraw);
        }

        public static bool ValidateTransaction(string senderBankName, string receiverBankName)
        {
            Bank senderBank = AdminServices.GetBank(senderBankName);
            string senderCurrencyCode = senderBank.Currency;
            Bank receiverBank = AdminServices.GetBank(receiverBankName);
            string receiverBankId = receiverBank.Id;
            List<string> acceptedCurrencies = AdminServices.GetAcceptedCurrencies(receiverBankId);
            return acceptedCurrencies.Contains(senderCurrencyCode);

        }

        public static void GenerateTransactionCharge(ref float transactionCharge, string receiversBankName, string transactionType, float moneyToTransfer, string senderBankName)
        {
            Bank senderBank = AdminServices.GetBank(senderBankName);
           
            if (senderBankName == receiversBankName)
            {
                if (transactionType == "RTGS")
                {
                    transactionCharge = (senderBank.RTGSCharge) * moneyToTransfer;
                }
                else if (transactionType == "IMPS")
                {
                    transactionCharge = (senderBank.IMPSCharge) * moneyToTransfer;
                }
            }
            else
            {
                if (transactionType == "RTGS")
                {
                    transactionCharge = (senderBank.TransferRTGSCharge) * moneyToTransfer;
                }
                else if (transactionType == "IMPS")
                {
                    transactionCharge = (senderBank.TransferIMPSCharge) * moneyToTransfer;
                }

            }
        }

        public static User GetCustomerFromAccountId(string accountId) {

            using (var context = new BankDBContext())
            {
                Account account = (Account)context.Accounts.Where(account => account.Id == accountId);
                User customer = (User)context.Users.Where(user => user.Id == account.UserId);
                return customer;
            }

        }
    }
}
