
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Bank
    {

        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public float RTGSCharge { get; set; } 

        [Required]
        public float IMPSCharge { get; set; } 

        [Required] 
        public float TransferRTGSCharge { get; set; } 

        [Required]
        public float TransferIMPSCharge { get; set; } 

        [Required]
        public string Currency { get; set; }

        public Bank(string Name, string Location) 
        { 
           this.Id = Name[..3] + DateTime.Now.ToString("");
           this.Name = Name;
           this.Location = Location;
           this.RTGSCharge = 0F;
           this.IMPSCharge = 0.05F;
            this.TransferIMPSCharge = 0.02F;
            this.TransferRTGSCharge = 0.06F;
            this.Currency = "INR";
        }
    }
}
