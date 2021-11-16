using System;
using System.ComponentModel.DataAnnotations.Schema;
using static CustomerLoans.Enum.Enums;

namespace CustomerLoans.Models
{

    public class Payment
    {
        public int PaymentId { get; set; }
        [ForeignKey("LoanId")]
        public int LoanId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNumber { get; set; }

        public virtual Loan Loan { get; set; }
    }
}
