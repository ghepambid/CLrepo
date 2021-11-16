using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Models
{
    public class AccountLoanViewModel
    {
        public string AccountNumber { get; set; }
        public int LoanId { get; set; }
        public Loan Loans { get; set; }
    }
}
