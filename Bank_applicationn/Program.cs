using Models;
using AllServices;
using Bank_Application;
using DataAcessLayer;
using Models.Enum_Types;

Console.WriteLine("Welcome..");

Boolean isSessionComplete = false;

while (!isSessionComplete)
{

    Boolean isCustomerSelectedCorrectOptions = false;

    while (!isCustomerSelectedCorrectOptions)
    {

        Console.WriteLine("1.Customer\n2.Staff\n3.Exit");

        string? userSelectionOption = Console.ReadLine();

        switch (userSelectionOption)
        {
            case "1":
                LoginAsAccountHolder();
                isCustomerSelectedCorrectOptions = true;
                break;
            case "2":
                Console.WriteLine("1.Admin\n2.Staff");
                string userRole = Console.ReadLine();
                switch (userRole)
                {
                    case "1":
                        Boolean validAdminCredentials = false;
                        while (!validAdminCredentials)
                        {
                            Console.WriteLine("Enter admin ID and Password");
                            string adminId = Console.ReadLine();
                            string adminPassword = Console.ReadLine();

                            if (ValidationServices.ValidateAdmin(adminId, adminPassword))
                            {
                                validAdminCredentials = true;
                                Console.WriteLine("Welcome Admin");
                            }
                            else
                            {
                                Console.WriteLine("Incorrect Credentials");
                            }
                        }
                        LoginAsAdmin();
                        break;
                    case "2":
                        LoginAsStaff();
                        break;
                    default:
                        Console.WriteLine("Please select a valid option");
                        break;
                }
                isCustomerSelectedCorrectOptions = true;
                break;
            default:
                Console.WriteLine("Invalid option\nPlease select valid option");
                break;
        }
    }
}

void LoginAsAccountHolder()
{
    Console.WriteLine("Please Enter your Bank Name.");

    string bankName = "";

    InputAndValidateBank(ref bankName);

    string customerID = "";

    InputAndValidateCustomer(bankName, ref customerID);

    string currentAccountID = "";

    InputAndValidateAccountId(customerID, ref currentAccountID);

    Boolean isCustomerSessionComplete = false;

    while (!isCustomerSessionComplete)
    {
        Console.WriteLine("1.Deposit amount\n2.Withdraw amount\n3.Transfer amount\n4.View Transactions\n5.Get Balance\n6.Exit");

        string customerOptions = Console.ReadLine();

        switch (customerOptions)
        {
            case "1":
                Console.WriteLine("Enter amount to deposit");
                float moneyToDeposit = float.Parse(Console.ReadLine());
                CustomerService.Deposit(currentAccountID, moneyToDeposit);
                Console.WriteLine("Amount deposited successfully");
                Console.WriteLine("Current Balance : " + CustomerService.GetBalance(currentAccountID));
                break;
            case "2":
                WithdrawAmount(currentAccountID);
                break;
            case "3":
                TransferAmount(currentAccountID);
                Console.WriteLine("Amount transferred successfully");
                Console.WriteLine("Current Balance : " + CustomerService.GetBalance(currentAccountID));
                break;
            case "4":

                List<Transaction> transactions = StaffService.GetTransaction(currentAccountID);

                foreach (Transaction transaction in transactions)
                {
                    Console.WriteLine(transaction.Id);
                }
                break;
            case "5":
                Console.WriteLine("Current Balance : " + CustomerService.GetBalance(currentAccountID));
                break;
            case "6":
                isCustomerSessionComplete = true;
                break;
            default:
                Console.WriteLine("Invalid Option.\nPlease Select valid option");
                break;
        }
    }
}

void LoginAsStaff()
{

    string staffBankName = null;

    InputAndValidateStaff(ref staffBankName);

    Boolean isStaffSessionComplete = false;

    while (!isStaffSessionComplete)
    {
        Console.WriteLine("1.Create new account\n2.Update account\n3.Delete account\n4.Add new Accepted currency with exchange rate");
        Console.WriteLine("5.Add service charge for same bank account\n6.Add service charge for other bank account");
        Console.WriteLine("7.Can view account transaction history\n8.Revert a transaction\n9.View All Customers\n10.Exit");


        string staffOptions = Console.ReadLine();

        switch (staffOptions)
        {
            case "1":
                CreateNewAccount(staffBankName);
                break;

            case "2":
                UpdateCustomer(staffBankName);
                break;

            case "3":
                DeleteAccount(staffBankName);
                break;

            case "4":
                AddCurrencyToAcceptedCurrencies(staffBankName);
                break;

            case "5":
                AddServiceChargeForSameBank(staffBankName);
                break;

            case "6":
                AddServiceChargeForTransferringBank(staffBankName);
                break;

            case "7":
                Console.WriteLine("Please Enter Customer ID");
                string customerId = "";
                InputAndValidateCustomer(staffBankName,ref customerId);
                string customerAccountIDToViewTransactions = "";
                InputAndValidateAccountId(customerId, ref customerAccountIDToViewTransactions);
                List<Transaction> transactions =  StaffService.GetTransaction(customerAccountIDToViewTransactions);

                foreach(Transaction transaction in transactions)
                {   
                    Console.WriteLine(transaction.Id);
                }

                break;

            case "8":

                Console.WriteLine("Enter customers Ids participated in the transaction");

                string senderCustomerId = "";

                InputAndValidateCustomer(staffBankName, ref senderCustomerId);

                string senderAccountId = "";

                InputAndValidateAccountId(senderCustomerId, ref senderAccountId);

                RevertTransaction(senderAccountId);

                break;

            case "9":
                ViewAllCustomers(staffBankName);
                break;

            case "10":
                isStaffSessionComplete = true;
                break;

            default:
                Console.WriteLine("Invalid option\nPlease select a valid option");
                break;
        }
    }
}

void LoginAsAdmin()
{

    Boolean isAdminSessionCompleted = false;

    while (!isAdminSessionCompleted)
    {
        Console.WriteLine("1.Create a Bank\n2.Add Staff to an existing Bank\n3.View All staff and their details\n4.Change Bank's Default operating currency\n5.Exit");

        string adminWorkOptions = Console.ReadLine();

        switch (adminWorkOptions)
        {
            case "1":
                CreateBank();
                break;
            case "2":
                AddStaffToExistingBank();
                break;
            case "3":
                ViewAllStaff();
                break;
            case "4":
                ChangeBankDefaultCurrency();
                break;
            case "5":
                isAdminSessionCompleted = true;
                break;
            default:
                Console.WriteLine("Invalid option\n Please select valid option");
                break;
        }
    }


}

void CreateBank()
{

    Boolean isNewBankValid = false;

    string newBankName = "";

    string newBankLocation = "";

    while (!isNewBankValid)
    {
        Console.WriteLine("Enter New Bank Name");
        newBankName = Console.ReadLine();
        Console.WriteLine("Enter Bank Location");
        newBankLocation = Console.ReadLine();
        if (AdminServices.ValidateBankInDB(newBankName.ToUpper()))
        {
            Console.WriteLine("Bank already exists with the same name\nPlease enter a new name for the bank");
        }
        else if(newBankName != "")
        {
            isNewBankValid = true;
        }
    }
    AdminServices.CreateBank(newBankName, newBankLocation);
    Console.WriteLine("New Bank " + newBankName.ToUpper() + " Added successfully");
}

void InputAndValidateBank(ref string BankName)
{
    bool isCorrectBank = false;


    while (!isCorrectBank)
    {
        BankName = Console.ReadLine();

        if (AdminServices.ValidateBankInDB(BankName.ToUpper()))
        {
            BankName = BankName.ToUpper();
            isCorrectBank = true;
        }
        else
        {
            Console.WriteLine("Please Enter valid Bank");
        }
    }
}

void InputAndValidateStaff(ref string staffBankName)
{

    bool validStaffCredentials = false;

    Console.WriteLine("Enter Bank Name");

    InputAndValidateBank(ref staffBankName);

    while (!validStaffCredentials)
    {
        Console.WriteLine("Enter staff ID and password");
        string staffID = Console.ReadLine();
        string staffPassword = Console.ReadLine();

        if (ValidationServices.ValidateStaff(staffID, staffPassword, staffBankName))
        {
            Console.WriteLine("Welcome ");
            validStaffCredentials = true;
        }
        else
        {
            Console.WriteLine("Incorrect Credentials");
        }
    }

}

void ChangeBankDefaultCurrency()
{
    Console.WriteLine("Enter Bank name for which the default currency to be updated");

    string targetBankName = "";

    InputAndValidateBank(ref targetBankName);

    Console.WriteLine("Default operating currency of " + AdminServices.GetBank(targetBankName).Name + " is " + AdminServices.GetBank(targetBankName).Currency);

    Console.WriteLine("1.Change default currency from Existing currencies\n2.Add new default currency\n3.Exit");

    string changeCurrencyOptions = "";

    Boolean isCorrectCurrencyChangeOptionSelected = false;

    string targetBankId = AdminServices.GetBank(targetBankName).Id;

    while (!isCorrectCurrencyChangeOptionSelected)
    {
        changeCurrencyOptions = Console.ReadLine();

        switch (changeCurrencyOptions)
        {
            case "1":
                ChangeDefaultCurrencyFromExistingCurrencies(targetBankId);
                isCorrectCurrencyChangeOptionSelected = true;
                break;
            case "2":
                AddNewDefaultCurrency(targetBankId);
                isCorrectCurrencyChangeOptionSelected = true;
                break;
            case "3":
                isCorrectCurrencyChangeOptionSelected = true;
                break;
            default:
                Console.WriteLine("Invalid Option\nPlease choose a valid option");
                break;
        }
    }
}

void AddNewDefaultCurrency(string targetBankId)
{
    Console.WriteLine("Enter the Code of new Currency(Three letter code)");

    string currencyCode = Console.ReadLine();

    Console.WriteLine("Enter the Exchange value of new Currency to INR");

    float currencyExchangeValue = float.Parse(Console.ReadLine());

    AdminServices.ChangeDefaultCurrency(targetBankId,currencyCode);

    AdminServices.AddCurrencyToAcceptedCurrencies(targetBankId,currencyCode, currencyExchangeValue);

    Console.WriteLine("New default Currency added successfully");

}

void ChangeDefaultCurrencyFromExistingCurrencies(string bankId)
{

    Boolean isCorrectCurrencySelected = false;

    Console.WriteLine("Change default currency to");

    List<string> acceptedCurrencies =  AdminServices.GetAcceptedCurrencies(bankId);

    foreach (String currency in acceptedCurrencies)
    {
        Console.WriteLine(currency);
    }

    while (!isCorrectCurrencySelected)
    {
        Console.WriteLine("Enter the currency code to change to");

        String updatedBankCurrency = (Console.ReadLine()).ToUpper();

        if (ValidationServices.ValidateBankCurrency(bankId,updatedBankCurrency))
        {
            AdminServices.ChangeDefaultCurrency(bankId,updatedBankCurrency);
            Console.WriteLine("Default currency changed to " + updatedBankCurrency.ToUpper() + " successfully");
            isCorrectCurrencySelected = true;
        }
        else
        {
            Console.WriteLine("Please enter currency code from existing accepted currencies");
        }
    }
}

void AddStaffToExistingBank()
{
    Console.WriteLine("Enter Bank Name");
    string BankName = "";
    InputAndValidateBank(ref BankName);

    //creating staff account
    Console.WriteLine("Creating Staff account");
    Console.WriteLine("Enter First Name");
    string firstName = Console.ReadLine();
    Console.WriteLine("Enter Last Name");
    string lastName = Console.ReadLine();

    string email = "";

    ValidateEmail(ref email);

    string password = "";

    ValidatePassword(ref password);


    AdminServices.CreateNewStaffAccount(firstName, lastName, email, password, BankName);

    Console.WriteLine("Account created successfully");  
}

void ValidateEmail(ref string email)
{
    bool isEmailStrong = false;


    while (!isEmailStrong)
    {
        Console.WriteLine("Enter Email");

        email = Console.ReadLine();

        if (ValidationServices.EmailValidator(email))
        {
            isEmailStrong = true;
        }
        else
        {
            Console.WriteLine("Please enter a valid email");
        }
    }
}

void ValidatePassword(ref string password)
{
    bool ispasswordStrong = false;


    while (!ispasswordStrong)
    {
        Console.WriteLine("Enter password");

        password = Console.ReadLine();

        if (ValidationServices.PasswordValidator(password))
        {
            ispasswordStrong = true;
        }
        else
        {
            Console.WriteLine("Please include atleast 1 capital letter, 1 small letter and password should be atleast 8 characters long");
        }
    }
}

void ViewAllStaff()
{
    Console.WriteLine("Enter the bank Name");
    string bankName = "";
    InputAndValidateBank(ref bankName);

    List<User> allStaff = AdminServices.GetAllStaff(bankName);

    int i = 1;

    foreach (User user in allStaff)
    {
        
            Console.WriteLine("staff No." + i);
            Console.WriteLine("\n\n");
            Console.WriteLine("                     Account ID : " + user.Id);
            Console.WriteLine("                     First Name : " + user.FirstName);
            Console.WriteLine("                     Last Name : " + user.LastName);
            Console.WriteLine("                     Email : " + user.Email);

            Console.WriteLine("\n\n");
            i++;
    }
}

void ViewAllCustomers(string bankName)
{

    List<User> allCustomers = StaffService.GetAllCustomers(bankName);

    int i = 1;

    foreach (User customer in allCustomers)
    {
       
            Console.WriteLine("Customer No." + i);
            Console.WriteLine("\n\n");
            Console.WriteLine("                     Account ID : " + customer.Id);
            Console.WriteLine("                     First Name : " + customer.FirstName);
            Console.WriteLine("                     Last Name : " + customer.LastName);
            Console.WriteLine("                     Email : " + customer.Email);
            Console.WriteLine("Accounts of " + customer.FirstName + "are");

            int j = 1;

            List<Account> allAccounts  = (List<Account>)StaffService.GetAllAccounts(customer.Id);


        foreach (Account account in allAccounts)
            {
                Console.WriteLine("Account No." + j);
                Console.WriteLine("\n\n");
                Console.WriteLine("                     Account ID : " + account.Id);
                Console.WriteLine("                     Balance : " + account.Balance);
                j++;
                Console.WriteLine("\n\n");

            }
            Console.WriteLine("\n\n");
            i++;
        }
}

void InputAndValidateCustomer(string bankName,ref string customerId)
{
    Console.WriteLine("Please Enter Customer ID");

    bool isCorrectCustomer = false;

    while (!isCorrectCustomer)
    {
        customerId = Console.ReadLine();

        Console.WriteLine("Enter password");

        string password = Console.ReadLine();

        if (ValidationServices.ValidateCustomer(bankName, customerId, password))
        {
            isCorrectCustomer = true;
        }
        else
        {
            Console.WriteLine("Please Enter valid Customer ID");
        }

    }
}

void InputAndValidateAccountId(string customerId, ref string accountId)
{
    Console.WriteLine("Enter account Id");

    bool isCorrectAccount = false;

    while (!isCorrectAccount)
    {
        accountId = Console.ReadLine();

        if (ValidationServices.ValidateAccount(customerId, accountId))
        {
            isCorrectAccount = true;
        }
        else
        {
            Console.WriteLine("Please Enter valid Account ID");
        }

    }
}

void WithdrawAmount(string accountId)
{
    Console.WriteLine("Enter amount to Debit");
    float moneyToWithdraw = float.Parse(Console.ReadLine());
    StaffService.ValidateAmountForDebit(accountId, moneyToWithdraw);
    CustomerService.Withdraw(accountId, moneyToWithdraw);
    Console.WriteLine("Amount withdrawn successfully");
    float balance = CustomerService.GetBalance(accountId);
    Console.WriteLine("Current Balance :" + balance);

}

void TransferAmount(string senderAccountId)
{
    string senderBankName = StaffService.GetBankNameFromAccount(senderAccountId);

    Console.WriteLine("Enter recievers Bank Name");

    string receiversBankName = "";

    InputAndValidateBank(ref receiversBankName);

    string receiverCustomerID = "";

    InputAndValidateCustomer(receiversBankName,ref receiverCustomerID);

    string receiverAccountID = "";

    InputAndValidateAccountId(receiverCustomerID, ref receiverAccountID);

    if (!StaffService.ValidateTransaction(receiversBankName, senderBankName))
    {
        Console.WriteLine("Cannot perform the transaction");
        Console.WriteLine(receiversBankName + " not accepts money from " + senderBankName);
    }

   
    Console.WriteLine("Enter amount to transfer");
    float moneyToTransfer = float.Parse(Console.ReadLine());
    Console.WriteLine("Enter Transaction Type");

    DisplayTransactionTypes();

    string transactionType = null;

    SelectTransactionType(ref transactionType);

    float transactionCharge = 0;

    StaffService.GenerateTransactionCharge(ref transactionCharge, receiversBankName, transactionType, moneyToTransfer,senderBankName);

    float receiverAccountAmountCredited = moneyToTransfer;

    moneyToTransfer += transactionCharge;

    float senderAccountAmountDebited = moneyToTransfer;

    if (!StaffService.ValidateAmountForDebit(senderAccountId,moneyToTransfer))
    {
        Console.WriteLine("Insufficient Funds");
    }
    else
    {
        CustomerService.Transfer(senderAccountId,receiverAccountID,senderBankName,receiversBankName,moneyToTransfer, transactionCharge, senderAccountAmountDebited, ref receiverAccountAmountCredited);
    }

}

void DisplayTransactionTypes()
{
    int i = 1;
    foreach (string transactionTypes in Enum.GetNames(typeof(EnumTypes.TransactionTypes)))
    {
        Console.WriteLine(i + "." + transactionTypes);
        i++;
    }
}

void SelectTransactionType(ref string transactionType)
{
    Boolean isCorrectTransaction = false;
    while (!isCorrectTransaction)
    {
        string transactionOption = Console.ReadLine();

        switch (transactionOption)
        {
            case "1":
                transactionType = EnumTypes.TransactionTypes.RTGS.ToString();
                isCorrectTransaction = true;
                break;
            case "2":
                transactionType = EnumTypes.TransactionTypes.IMPS.ToString();
                isCorrectTransaction = true;
                break;
            default:
                Console.WriteLine("Invalid options.\nPlease select valid option");
                break;
        }
    }
}


void CreateNewAccount(string staffBankName)
{
    Boolean isValidStaffOption = false;

    while (!isValidStaffOption)
    {
        Console.WriteLine("1.Create new Customer Account.\n2.Create Account for Existing Customer");
        string staffOption = Console.ReadLine();
        switch (staffOption)
        {
            case "1":
                Console.WriteLine("Creating new account");
                Console.WriteLine("Enter First Name");
                string firstName = Console.ReadLine();
                Console.WriteLine("Enter Last Name");
                string lastName = Console.ReadLine();

                string email = "";

                ValidateEmail(ref email);

                string password = "";

                ValidatePassword(ref password);

                StaffService.CreateAccountForNewCustomer(firstName, lastName, email, password, staffBankName);

                Console.WriteLine("Account created successfully");
                isValidStaffOption = true;
                break;
            case "2":

                Console.WriteLine("Please Enter your Customer ID");

                string customerID = "";

                InputAndValidateCustomer(staffBankName, ref customerID);
                

                StaffService.CreateAccountForExistingCustomer(customerID);

                Console.WriteLine("Another account created successfully\n");

                isValidStaffOption = true;
                break;
            default:
                Console.WriteLine("Invalid choice\nPlease enter a valid choice");
                break;

        }

    }

}

void UpdateCustomer(string staffBankName)
{
    Console.WriteLine("Updating account");

    Console.WriteLine("Please Enter Customer ID");

    string CustomerID = "";

    InputAndValidateCustomer(staffBankName, ref CustomerID);

    Console.WriteLine("Select the fields to update");

    Console.WriteLine("1.First Name\n2.Last Name\n3.Email\n4.Password");

    string updateOptionsForAccount = Console.ReadLine();

    bool correctOptionSelected = false;

    while (!correctOptionSelected)
    {
        switch (updateOptionsForAccount)
        {
            case "1":
                Console.WriteLine("Enter new first name");
                string firstName = Console.ReadLine();
                StaffService.UpdateFirstName(CustomerID, firstName);
                Console.WriteLine("First Name updated successfully");
                correctOptionSelected = true;
                break;
            case "2":
                Console.WriteLine("Enter new Last name");
                string lastName = Console.ReadLine();
                StaffService.UpdateLastName(CustomerID, lastName);
                Console.WriteLine("Last Name updated successfully");
                correctOptionSelected = true;
                break;
            case "3":
                Console.WriteLine("Enter new email"); 
                string email = Console.ReadLine();
                StaffService.UpdateEmail(CustomerID, email);
                Console.WriteLine("email updated successfully");
                correctOptionSelected = true;
                break;
            case "4":
                Console.WriteLine("Enter new password");
                string password = Console.ReadLine();
                StaffService.UpdatePassword(CustomerID, password); 
                Console.WriteLine("Password updated successfully");
                correctOptionSelected = true;
                break;
            default:
                Console.WriteLine("Please select a valid option");
                break;
        }
    }
}

void DeleteAccount(string staffBankName)
{
    Boolean isValidStaffOption = false;

    while (isValidStaffOption)
    {
        Console.WriteLine("1.Delete Customer Record.\n2.Delete single account of Existing Customer");

        string staffOption = Console.ReadLine();

        switch (staffOption)
        {
            case "1":

                string customerId = "";

                InputAndValidateCustomer(staffBankName, ref customerId);

                StaffService.DeleteCustomer(customerId);

                Console.WriteLine("Customer Record deleted successfully");

                isValidStaffOption = true;
                break;
            case "2":

                string customerID = "";

                InputAndValidateCustomer(staffBankName, ref customerID);

                string accountIDToDelete = "";

                InputAndValidateAccountId(customerID, ref accountIDToDelete);

                StaffService.DeleteAccount(accountIDToDelete);

                Console.WriteLine("Account deleted successfully");

                isValidStaffOption = true;
                break;
            default:
                Console.WriteLine("Invalid choice\nPlease enter a valid choice");
                break;
        }
    }

}

void AddCurrencyToAcceptedCurrencies(string bankName)
{
    Console.WriteLine("Existing Accepted currencies and their exchange values to Rupee are ");

    List<string> acceptedCurrencies =  AdminServices.GetAcceptedCurrencies(AdminServices.GetBank(bankName).Id);

    foreach (string currency in acceptedCurrencies)
    {
        Console.WriteLine(currency);
    }
    
    Console.WriteLine("Enter the Code of new Currency(Three letter code)");

    string currencyCode = Console.ReadLine();

    Console.WriteLine("Enter the Exchange value of new Currency to INR");

    float currencyExchangeValue = float.Parse(Console.ReadLine());

    AdminServices.AddCurrencyToAcceptedCurrencies(AdminServices.GetBank(bankName).Id, currencyCode, currencyExchangeValue);

    Console.WriteLine("New Accepted currency added successfully");

}

void AddServiceChargeForSameBank(string bankName)
{

    Console.WriteLine("Add RTGS Charge for the bank");
    float updatedRTGSCharge = float.Parse(Console.ReadLine());
    Console.WriteLine("Add IMPS Charge for the bank");
    float updatedIMPSCharge = float.Parse(Console.ReadLine());
    StaffService.AddServiceChargeForSameBank(bankName, updatedRTGSCharge, updatedIMPSCharge);
    Console.WriteLine("Charges updates successfully");
}

void AddServiceChargeForTransferringBank(string bankName)
{
    Console.WriteLine("Add RTGS Charge for other bank");
    float updatedRTGSChargeToOtherBank = float.Parse(Console.ReadLine());
    Console.WriteLine("Add IMPS Charge for other bank");
    float updatedIMPSChargeToOtherBank = float.Parse(Console.ReadLine());
    StaffService.AddServiceChargeForTransferringBank(bankName, updatedRTGSChargeToOtherBank, updatedIMPSChargeToOtherBank);
    Console.WriteLine("Charges updates successfully");
}

void RevertTransaction(string senderAccountId)
{

    Console.WriteLine("List of Transactions of are \n");

    List<Transaction> transactions = StaffService.GetTransaction(senderAccountId);

    foreach (Transaction transaction in transactions)
    {
        Console.WriteLine(transaction.Id);
    };

    Console.WriteLine("\nEnter transaction ID");

    string senderTransactionId = Console.ReadLine();

    while (!ValidationServices.ValidateTransaction(senderAccountId, senderTransactionId))
    {
        Console.WriteLine("Enter valid transaction id");

        senderTransactionId = Console.ReadLine();
    }

    float moneyTransferred = StaffService.GetMoneyTransferredInTransaction(senderTransactionId);
    
    CustomerService.Deposit(senderAccountId, moneyTransferred);

    Console.WriteLine("Successfully reverted transaction in sender");

    //revert receiver transaction id

    string receiverTransactionId = StaffService.GetReceiverTransactionID(senderTransactionId);

    moneyTransferred = StaffService.GetMoneyTransferredInTransaction(receiverTransactionId);

    string receiverAccountId = StaffService.GetReceiverAccountIdFromTransaction(receiverTransactionId);

    CustomerService.Withdraw(receiverAccountId, moneyTransferred);

    Console.WriteLine("Transaction reverted successfully");

    CustomerService.RemoveTransaction(senderTransactionId);

    CustomerService.RemoveTransaction(receiverTransactionId);
    

}


