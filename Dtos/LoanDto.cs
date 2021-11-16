using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class LoanDto
    {
        public int LoanId { get; set; }
        public string LoanType { get; set; }
        public string LoanDate { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalPayment { get; set; }

        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
