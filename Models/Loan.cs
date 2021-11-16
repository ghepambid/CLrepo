using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CustomerLoans.Enum.Enums;

namespace CustomerLoans.Models
{

    public class Loan
    {
        [Key]
        public int LoanId { get; set; }
        public LoanType LoanType { get; set; }
        public string LoanDate { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanInterest { get; set; }

        public virtual List<Payment> Payments { get; set; }
        public virtual Account Account { get; set; }
    }
}
