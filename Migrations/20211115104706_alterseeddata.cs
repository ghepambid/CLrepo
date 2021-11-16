using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerLoans.Migrations
{
    public partial class alterseeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountNumber", "AccountName", "Password", "SecretKey", "AccountStatus" },
                values: new object[] { 1, "AN7103327640640", "Nate Collins", null, null, 0 }
                );

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountNumber", "AccountName", "Password", "SecretKey", "AccountStatus" },
                values: new object[] { 2, "AN7103327640641", "Elly Grant", null, null, 0 }
                );

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountNumber", "AccountName", "Password", "SecretKey", "AccountStatus" },
                values: new object[] { 3, "AN7103327640642", "Aurelia Shepherd", null, null, 0 }
                );

            //Loans
            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "LoanId", "LoanType", "LoanDate","LoanAmount","LoanInterest", "AccountId"},
                values: new object[] { 1, 1, "1/10/2021", 200000.00, 2000.00, 1 }
                );

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "LoanId", "LoanType", "LoanDate", "LoanAmount", "LoanInterest", "AccountId" },
                values: new object[] { 2, 1, "1/10/2021", 300000.00, 3000.00, 2 }
                );

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "LoanId", "LoanType", "LoanDate", "LoanAmount", "LoanInterest", "AccountId" },
                values: new object[] { 3, 1, "1/10/2021", 400000.00, 4000.00, 3 }
                );

            //Payments
            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "LoanId", "PaymentDate", "Amount", "Status", "Remarks", "ReferenceNumber" },
                values: new object[] { 1, 1, "2/20/2021", 5000.00, 1, "Payment for February 2021", "RF000001" }
                );

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "LoanId", "PaymentDate", "Amount", "Status", "Remarks", "ReferenceNumber" },
                values: new object[] { 2, 2, "2/20/2021", 5000.00, 1, "Payment for February 2021", "RF000002" }
                );

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "LoanId", "PaymentDate", "Amount", "Status", "Remarks", "ReferenceNumber" },
                values: new object[] { 3, 3, "2/20/2021", 5000.00, 1, "Payment for February 2021", "RF000003" }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3);

            migrationBuilder.DeleteData(
               table: "Loans",
               keyColumn: "LoanId",
               keyValue: 1);

            migrationBuilder.DeleteData(
               table: "Loans",
               keyColumn: "LoanId",
               keyValue: 2);

            migrationBuilder.DeleteData(
               table: "Loans",
               keyColumn: "LoanId",
               keyValue: 3);
        }
    }
}
