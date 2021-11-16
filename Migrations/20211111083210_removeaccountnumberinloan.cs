using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerLoans.Migrations
{
    public partial class removeaccountnumberinloan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Loans");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
