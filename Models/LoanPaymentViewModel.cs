using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static CustomerLoans.Enum.Enums;

namespace CustomerLoans.Models
{
    public class LoanPaymentViewModel
    {
        [Key]
        public int LoanId { get; set; }
        public LoanType LoanType { get; set; }
        public string LoanDate { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanInterest { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal Balance { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }
}
