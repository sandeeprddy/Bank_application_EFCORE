using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enum_Types;

namespace Models
{
     public  class User
    {
        public EnumTypes.UserTypes UserType { get; set; }

        [Key]
        public string Id { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [ForeignKey("Bank")]
        public string BankId { get; set; }

        public Bank Bank { get; set; }

        public User(string firstName,string lastName, string email, string password, string bankId)
        {
                this.Id = firstName[..3] + DateTime.Now.ToString("");
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Email = email;
                this.Password = password;
                this.BankId = bankId;
        }


    }
}
