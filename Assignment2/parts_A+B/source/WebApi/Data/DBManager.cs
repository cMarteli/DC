using LocalDBWebApiUsingEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Models;

namespace LocalDBWebApiUsingEF.Data
{
    public class DBManager : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source = BankDB.db;");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; } // Add the AuditLogs DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Get randomly generated users
            List<User> users = FixedSizeUserList.GetInstance().GetUsers();
            List<BankAccount> bankAccounts = FixedSizeUserList.GetInstance().GetBankAccounts();
            modelBuilder.Entity<User>().HasData(users);

            // AdminUser Seed Data
            List<Admin> admin = new List<Admin>
            {
                new Admin("admin")
                {
                    Name = "Admin User",
                    Email = "email0@gmail.com",
                    Address = "Sydney",
                    Phone = "061-000-0200",
                    Password = "adminpassword",
                    Picture = UserGenerator.GetImageBytes()
                }
            };
            modelBuilder.Entity<Admin>().HasData(admin);

            // Transaction Seed Data
            List<Transaction> transactions = new List<Transaction>();
            Random random = new Random();

            int transactionIdCounter = 1; // Start the counter for TransactionId

            for (int i = 0; i < 15; i++)
            { //Create 15 transactions
              // Randomly pick a sender account
                BankAccount senderAccount = bankAccounts[random.Next(bankAccounts.Count)];

                // Randomly pick a receiver account. Ensures it's not the same as sender.
                BankAccount receiverAccount;
                do
                {
                    receiverAccount = bankAccounts[random.Next(bankAccounts.Count)];
                } while (senderAccount.AccountNumber == receiverAccount.AccountNumber);

                // Generate a random transaction amount that's less than the sender account's balance
                double transactionAmount = Math.Round(random.NextDouble() * senderAccount.Balance);

                // Creates transaction record
                transactions.Add(new Transaction
                {
                    TransactionId = transactionIdCounter,
                    FromAccountNumber = senderAccount.AccountNumber,
                    ToAccountNumber = receiverAccount.AccountNumber,
                    Amount = transactionAmount,
                    Description = $"Transfer of {transactionAmount:C} from {senderAccount.AccountNumber} to {receiverAccount.AccountNumber}"
                });

                // Increments TransactionId
                transactionIdCounter++;

                // Deducts amount from senders account and adds it to receiver
                senderAccount.Balance -= transactionAmount;
                receiverAccount.Balance += transactionAmount;
            }

            modelBuilder.Entity<Transaction>().HasData(transactions);


            // Set Username as the primary key for User
            modelBuilder.Entity<User>()
                    .HasKey(b => b.Username);

            // Set Username as the primary key for AdminUser
            modelBuilder.Entity<Admin>()
                    .HasKey(a => a.Username);

            // Set AccountNumber as the primary key for BankAccount
            modelBuilder.Entity<BankAccount>()
                    .HasKey(b => b.AccountNumber);

            // Set Transaction primary key
            modelBuilder.Entity<Transaction>().HasKey(t => t.TransactionId);

            // Set Transaction primary key to autoincrement
            modelBuilder.Entity<Transaction>().Property(t => t.TransactionId).ValueGeneratedOnAdd();

            // Configure the relationship between BankAccount and User
            modelBuilder.Entity<BankAccount>()
                    .HasOne(b => b.User)
                    .WithMany(u => u.BankAccounts)  // Indicates that a user can have multiple bank accounts
                    .HasForeignKey(b => b.UserUsername)
                    .OnDelete(DeleteBehavior.Cascade)  // If a user is deleted, their bank accounts are deleted as well
                    .IsRequired(true);  // UserId is required, but User navigation property is not

            // Configure the relationship between Transaction and BankAccount (FromAccount)
            modelBuilder.Entity<Transaction>()
                    .HasOne(t => t.FromBankAccount)
                    .WithMany(b => b.FromTransactions)
                    .HasForeignKey(t => t.FromAccountNumber)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict as even if an account is deleted we dont want to delete the transaction

            // Configure the relationship between Transaction and BankAccount (ToAccount)
            modelBuilder.Entity<Transaction>()
                    .HasOne(t => t.ToBankAccount)
                    .WithMany(b => b.ToTransactions)
                    .HasForeignKey(t => t.ToAccountNumber)
                    .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete

            modelBuilder.Entity<BankAccount>().HasData(bankAccounts);
        }

        /// <summary>
        /// Logging method
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = ChangeTracker.Entries().Where(entry => entry.State == EntityState.Modified);
            foreach (var entry in entries)
            {
                AddAuditLogIfModified(entry);
            }

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not save logs to the database.", ex);
            }
        }

        /// <summary>
        /// Helper method to add audit logs
        /// </summary>
        /// <param name="entry"></param>
        private void AddAuditLogIfModified(EntityEntry entry)
        {
            string entityName = null;
            Dictionary<string, object> original = new Dictionary<string, object>();
            Dictionary<string, object> current = new Dictionary<string, object>();
            Dictionary<string, object> modified = new Dictionary<string, object>();
            string action = "Modified";

            if (entry.Entity is Admin || entry.Entity is User || entry.Entity is BankAccount)
            {
                entityName = entry.Entity.GetType().Name;

                foreach (var property in entry.OriginalValues.Properties)
                {
                    // Exclude the 'Picture' field
                    if (property.Name == "Picture")
                    {
                        continue;
                    }
                    var originalValue = entry.OriginalValues[property];
                    var currentValue = entry.CurrentValues[property];
                    if (!object.Equals(originalValue, currentValue))
                    {
                        original[property.Name] = originalValue;
                        current[property.Name] = currentValue;
                        modified[property.Name] = new { Original = originalValue, Current = currentValue };
                    }
                }

                if (modified.Count > 0)
                {
                    var auditLog = new AuditLog
                    {
                        EntityName = entityName,
                        Action = action,
                        OriginalValues = JsonConvert.SerializeObject(original),
                        CurrentValues = JsonConvert.SerializeObject(current),
                        Timestamp = DateTime.UtcNow
                    };

                    AuditLogs.Add(auditLog);

                    // Save to File
                    string logMessage = $"Timestamp: {auditLog.Timestamp}, Entity: {entityName}, Action: {action}, Modified Fields: {JsonConvert.SerializeObject(modified)}\n";
                    File.AppendAllText("AuditLogs.txt", logMessage);
                }
            }
        }




    }
}
