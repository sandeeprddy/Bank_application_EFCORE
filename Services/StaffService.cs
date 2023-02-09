
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

            Account account = new(customer.Id);

            context.Accounts.Add(account);

            context.SaveChanges();
        }

        public static void CreateAccountForExistingCustomer(string customerId)
        {

            Account newAccount = new(customerId);

            newAccount.UserId = customerId;

            using BankDBContext context = new();

            context.Accounts.Add(newAccount);

            context.SaveChanges();

        }

        public static void UpdateFirstName(string customerId, string name)
        { 
            using BankDBContext context = new();

            List<User> users = context.Users.Where(user => user.Id == customerId).ToList();

            users[0].FirstName = name;

        }

        public static void UpdateLastName(string customerId, string name)
        {
            using BankDBContext context = new();

            List<User> users = context.Users.Where(user => user.Id == customerId).ToList();

            users[0].LastName = name;

        }

        public static void UpdateEmail(string customerId, string email)
        {
            using BankDBContext context = new();

            List<User> users = context.Users.Where(user => user.Id == customerId).ToList();

            users[0].Email = email;

        }

        public static void UpdatePassword(string customerId, string password)
        {
            using BankDBContext context = new();

            List<User> users = context.Users.Where(user => user.Id == customerId).ToList();

            users[0].Password = password;

        }

        public static void DeleteCustomer(string customerId)
        {
            using BankDBContext context = new();

            List<User> users = context.Users.Where(user => user.Id == customerId).ToList();

            List<Account> accounts = context.Accounts.Where(account => account.UserId == customerId).ToList();

            foreach (Account account in accounts)
            {
                context.Accounts.Remove(account);
            }

            context.Users.Remove(users[0]);

            context.SaveChanges();
        }

        public static void DeleteAccount(string accountIDToDelete)
        {
            using BankDBContext context = new();
            List<Account> accounts = context.Accounts.Where(account => account.Id == accountIDToDelete).ToList();
            context.Accounts.Remove(accounts[0]);  
        }

        public static void AddServiceChargeForTransferringBank(string bankName, float updatedRTGSChargeToOtherBank, float updatedIMPSChargeToOtherBank)
        {
            using BankDBContext context = new BankDBContext();
            List<Bank> banks = context.Banks.Where(bank => bank.Name == bankName).ToList();
            banks[0].TransferRTGSCharge = updatedRTGSChargeToOtherBank;
            banks[0].TransferRTGSCharge = updatedIMPSChargeToOtherBank;
        }

        public static float GetMoneyTransferredInTransaction(string transactionId)
        {
            using BankDBContext context = new();

            List<Transaction> transactions = context.Transactions.Where(transaction => transaction.Id == transactionId).ToList();

            return transactions[0].MoneyTransferred;

        }

        public static void AddServiceChargeForSameBank(string bankName, float updatedRTGSCharge, float updatedIMPSCharge)
        {
            using BankDBContext context = new BankDBContext();
            List<Bank> banks = context.Banks.Where(bank => bank.Name == bankName).ToList();
            banks[0].RTGSCharge = updatedRTGSCharge;
            banks[0].IMPSCharge = updatedIMPSCharge;
        }

        public static List<Transaction> GetTransaction(string accountId)
        {
            using BankDBContext context = new();

           List<Transaction> transactions = context.Transactions.Where(transaction => transaction.AccountId == accountId).ToList();

            return transactions;
        }

        public static string GetReceiverTransactionID(string transactionId)
        {
            using (var context = new BankDBContext())
            {
                List<Transaction> transactions = context.Transactions.Where(transaction => transaction.Id == transactionId).ToList();
                return (transactions[0].ReceiverTransactionId);
            }
        }

        public static string GetReceiverAccountIdFromTransaction(string transactionId)
        {
            using (var context = new BankDBContext())
            {
               List<Transaction> transactions = context.Transactions.Where(transaction => transaction.Id == transactionId).ToList();
                return transactions[0].ReceiverId;
            }
        }

        public static List<User> GetAllCustomers(string bankName)
        {
            using (var context = new BankDBContext())
            {
                string bankId = (AdminServices.GetBank(bankName)).Id;
                List<User> allCustomers = context.Users.Where(customer => (customer.BankId == bankId && customer.UserType.Equals(Models.Enum_Types.EnumTypes.UserTypes.Customer))).ToList();
                return allCustomers;   
            }
        }

        public static List<Account> GetAllAccounts(string customerId)
        {
            using (var context = new BankDBContext())
            {
                List<Account> allAccounts = context.Accounts.Where(account => (account.UserId == customerId)).ToList();
                return allAccounts;
            }
        }

        public static string GetBankNameFromAccount(string accountId)
        {
            using (var context = new BankDBContext())
            {
              List<Account> accounts = context.Accounts.Where(account => account.Id == accountId).ToList();


              List<User> customers = context.Users.Where(user => user.Id == accounts[0].UserId).ToList();

                List<Bank> banks = context.Banks.Where(bank => bank.Id == (customers[0].BankId)).ToList();

                return banks[0].Name;

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
                List<Account> accounts = context.Accounts.Where(account => account.Id == accountId).ToList();
                List<User> customers = context.Users.Where(user => user.Id == accounts[0].UserId).ToList();
                return customers[0];
            }

        }
    }
}
