using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class UpdatePaymentDto
    {   
        public string Message { get; set; }
        public int PaymentId { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string status { get; set; }
    }
}
