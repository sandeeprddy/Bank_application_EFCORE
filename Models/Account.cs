using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Account
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public float Balance { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        
        public User User { get; set; }


        public Account(string userId)
        {
            this.Id =  DateTime.Now.ToString("");
            
            this.Balance = 0;

            this.UserId = userId;

        }

    }
}
