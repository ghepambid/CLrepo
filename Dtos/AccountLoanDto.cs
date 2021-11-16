using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class AccountLoanDto
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountStatus { get; set; }

        public virtual List<LoanDto> Loans { get; set; } = new List<LoanDto>();
    }
}
