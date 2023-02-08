using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public string SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public float MoneyTransferred { get; set; }
        
        [Required]
        public string TransactionInfo { get; set; }
        
        [Required]
        public string ReceiverTransactionID { get; set; }


        [ForeignKey("Account")]
        public string AccountId { get; set; }

        public Account Account { get; set; }

        public Transaction(string id, string senderId, string receiverId, float moneyTransferred, string transactionInfo, string receiverTransactionId, string accountId)
        { 
            this.Id = id;
            this.SenderId = senderId;
            this.ReceiverId = receiverId;
            this.MoneyTransferred = moneyTransferred;
            this.TransactionInfo = transactionInfo;
            this.ReceiverTransactionID = receiverTransactionId;  
            this.AccountId = accountId;
        }

    }
}
