using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class BankCurrency
    {
        public int Id { get; set; }

        [Required]
        public string BankId { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        public BankCurrency(string bankId, string currencyCode)
        {
            this.BankId = bankId;
            this.CurrencyCode = currencyCode;
        }

    }
}
