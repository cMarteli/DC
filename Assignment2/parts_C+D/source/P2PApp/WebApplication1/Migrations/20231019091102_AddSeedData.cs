using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataServer.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    CompletedJobs = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => new { x.IPAddress, x.Port });
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "IPAddress", "Port", "CompletedJobs" },
                values: new object[,]
                {
                    { "192.168.1.1", 8080, 0 },
                    { "192.168.1.2", 8081, 0 },
                    { "192.168.1.3", 8082, 0 },
                    { "192.168.1.4", 8083, 0 },
                    { "192.168.1.5", 8084, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
