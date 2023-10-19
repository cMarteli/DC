using System.Text.Json.Serialization;

namespace LocalDBWebApiUsingEF.Models
{
    public class BankAccount
    {
        public BankAccount(int AccountNumber, string UserUsername)
        {
            this.AccountNumber = AccountNumber;
            this.UserUsername = UserUsername!;
        }

        public int AccountNumber { get; set; }
        public string? AccountHolderName { get; set; }
        public double Balance { get; set; }

        // Foreign Key to User
        public string UserUsername { get; set; }

        // Navigation Property to User (optional but recommended for EF operations)
        [JsonIgnore]
        public virtual User? User { get; set; }

        // Transactions initiated from this account
        public virtual ICollection<Transaction> FromTransactions { get; set; } = new List<Transaction>();

        // Transactions received to this account
        public virtual ICollection<Transaction> ToTransactions { get; set; } = new List<Transaction>();
    }
}
