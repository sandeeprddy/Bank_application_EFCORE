using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Currency
    {

        [Key]
        public string Code { get; set; }

        [Required]
        public float ExchangeValue { get; set; }

        public Currency(string code, float exchangeValue)
        {
           this.Code = code;
           this.ExchangeValue = exchangeValue;
        }

    }
}
