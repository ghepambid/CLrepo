using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerLoans.Migrations
{
    public partial class addforeignkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Loans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LoanId",
                table: "Payments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_AccountId",
                table: "Loans",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Accounts_AccountId",
                table: "Loans",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Loans_LoanId",
                table: "Payments",
                column: "LoanId",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Accounts_AccountId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Loans_LoanId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_LoanId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Loans_AccountId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Loans");
        }
    }
}
