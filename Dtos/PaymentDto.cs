using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public string PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
