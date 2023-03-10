using System.Reflection;
using DataAcessLayer;
using Microsoft.Identity.Client;
using Models;

namespace AllServices
{
    public  class CustomerService
    {
        public static void Deposit(string accountId, float money)
        {
            using (var context = new BankDBContext())
            {
                List<Account> account = context.Accounts.Where(account => account.Id == accountId).ToList();
                account[0].Balance += money;
                context.SaveChanges();
            }
        }

        public static void Withdraw(string accountId, float money)
        {
            using (var context = new BankDBContext())
            {
                List<Account> account = context.Accounts.Where(account => account.Id == accountId).ToList();
                account[0].Balance -= money;
                context.SaveChanges();
            }
        }

        public static float GetBalance(string accountId)
        {
            using (var context = new BankDBContext())
            {
                List<Account> account = context.Accounts.Where(account => account.Id == accountId).ToList();
                return account[0].Balance;
            }
        }

        public static float GetCurrencyExchangeValue(string currencyCode)
        {
            using (var context = new BankDBContext())
            {
               return (context.Currencies.Where(currency => currency.Code == currencyCode).ToList())[0].ExchangeValue;
            }
        }

        public static void Transfer(string senderAccountId,string receiverAccountId,string senderBankName,string receiverBankName,float moneyToTransfer, float transactionCharge, float senderAccountAmountDebited, ref float receiverAccountAmountCredited)
        {
            Withdraw(senderAccountId, moneyToTransfer);

            Bank senderBank = AdminServices.GetBank(senderBankName);
            string senderCurrencyCode = senderBank.Currency;

            Bank receiverBank = AdminServices.GetBank(receiverBankName);
            string receiverCurrencyCode = receiverBank.Currency;


            if (senderCurrencyCode != receiverCurrencyCode)
            {
                float sendersCurrencyValue = GetCurrencyExchangeValue(senderCurrencyCode);
                float receiversCurrencyValue = GetCurrencyExchangeValue(receiverCurrencyCode); ;

                if (senderCurrencyCode != "INR" && receiverCurrencyCode != "INR")
                {
                    float senderCurrencyToINR = (moneyToTransfer - transactionCharge) * (sendersCurrencyValue);

                    float INRToReceiversCurrency = (senderCurrencyToINR / receiversCurrencyValue);

                    CustomerService.Deposit(receiverAccountId, INRToReceiversCurrency);

                    receiverAccountAmountCredited = INRToReceiversCurrency;

                }
                else
                {
                    if (sendersCurrencyValue > receiversCurrencyValue)
                    {
                        moneyToTransfer = (moneyToTransfer - transactionCharge) * (sendersCurrencyValue);

                       
                    }
                    else
                    {
                        moneyToTransfer = (moneyToTransfer - transactionCharge) / (sendersCurrencyValue);
                    }

                    CustomerService.Deposit(receiverAccountId, moneyToTransfer);

                    receiverAccountAmountCredited = moneyToTransfer;
                }
            }
            else
            {
                CustomerService.Deposit(receiverAccountId, moneyToTransfer - transactionCharge);
                
            }
            CustomerService.GenerateTransactionInfo(senderAccountId, receiverAccountId, senderBankName, receiverBankName, (senderAccountAmountDebited - transactionCharge), receiverAccountAmountCredited);
        

        }

        public static void AddTransaction(Transaction Transaction)
        {
            using (var context = new BankDBContext())
            {
                context.Transactions.Add(Transaction);
                context.SaveChanges();
            }
        }

        public static void RemoveTransaction(string transactionId)
        {
            using (var context = new BankDBContext())
            {
                List<Transaction> transactions = context.Transactions.Where(transaction => transaction.Id == transactionId).ToList();
                context.Transactions.Remove(transactions[0]);
                context.SaveChanges();
            }
        }

        public static void GenerateTransactionInfo(string senderAccountId, string receiverAccountId, string senderBankName, string receiverBankName, float senderAccountAmountDebited,float receiverAccountAmountCredited)
        {
            Bank senderBank = AdminServices.GetBank(senderBankName);
            Bank receiverBank = AdminServices.GetBank(receiverBankName);

            User sender = StaffService.GetCustomerFromAccountId(senderAccountId);
            User receiver = StaffService.GetCustomerFromAccountId(receiverAccountId);

            string senderTransactionID = "TXN" + senderBank.Id + senderAccountId + DateTime.Now.ToString("");

            string receiverTransactionID = "TXN" + (receiverBank).Id + receiverAccountId + DateTime.Now.ToString("");

            string senderTransactionInfo = sender.FirstName + " sent " + senderAccountAmountDebited + senderBank.Currency + " to " + receiver.FirstName;

            string receiverTransactionInfo = receiver.FirstName + " received " + receiverAccountAmountCredited + receiverBank.Currency + " from " + sender.FirstName;


            Transaction senderTransaction = new(senderTransactionID, senderAccountId, receiverAccountId, senderAccountAmountDebited,senderTransactionInfo, receiverTransactionID, senderAccountId);

            Transaction receiverTransaction = new(receiverTransactionID, senderAccountId, receiverAccountId, receiverAccountAmountCredited, receiverTransactionInfo,senderTransactionID, receiverAccountId);

            CustomerService.AddTransaction(senderTransaction);
            CustomerService.AddTransaction(receiverTransaction);

        }
    }
         

}

