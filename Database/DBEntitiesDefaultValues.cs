

using Models;

namespace DataAcessLayer
{
    public class DBEntitiesDefaultValues
    {
        public static void InitializeDefaultValuesForCurrencies()
        {
            using BankDBContext context = new();

            Currency INR = new Currency("INR", 0);
            Currency EUR = new Currency("EUR", 90);
            Currency USD = new Currency("USD", 80);

            context.Currencies.Add(INR);
            context.Currencies.Add(EUR); 
            context.Currencies.Add(USD);

            context.SaveChanges();
        }
    }
}
