namespace LocalDBWebApiUsingEF.Models
{
    public class User
    {
        public User(string? Username)
        {
            this.Username = Username!;
        }

        public string Username { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public byte[]? Picture { get; set; }
        public string? Password { get; set; }
        public string? SessionID { get; set; }
        // Navigation Property for BankAccounts
        public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
    }
}
