using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
