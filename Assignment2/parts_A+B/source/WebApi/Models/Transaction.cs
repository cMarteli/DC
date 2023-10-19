using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LocalDBWebApiUsingEF.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        [Required]
        public int FromAccountNumber { get; set; }

        [Required]
        public int ToAccountNumber { get; set; }

        [Required]
        public double Amount { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Foreign Key to User
        [JsonIgnore]
        public virtual BankAccount? BankAccount { get; set; }
        public string? Description { get; internal set; }

        // Navigation property for the account that initiated the transaction
        [JsonIgnore]
        public virtual BankAccount? FromBankAccount { get; set; }

        // Navigation property for the account that received the transaction
        [JsonIgnore]
        public virtual BankAccount? ToBankAccount { get; set; }
    }
}