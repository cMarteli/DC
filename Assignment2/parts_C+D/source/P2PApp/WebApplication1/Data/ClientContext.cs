using DataServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataServer.Data {
    public class ClientContext : DbContext {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite(@"Data Source = ClientDB.db;");
        }
        public DbSet<Models.Client> Clients { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            List<Client> users = new List<Client> {
                new Client { Id = 1, IPAddress = "192.168.1.1", Port = 8080 },
                new Client { Id = 2, IPAddress = "192.168.1.2", Port = 8081 },
                new Client { Id = 3, IPAddress = "192.168.1.3", Port = 8082 },
                new Client { Id = 4, IPAddress = "192.168.1.4", Port = 8083 },
                new Client { Id = 5, IPAddress = "192.168.1.5", Port = 8084 }
            };

            modelBuilder.Entity<Client>().HasData(users);

            // Set ID as the primary key for Client
            modelBuilder.Entity<Client>()
                    .HasKey(a => a.Id);
        }


    }
}
