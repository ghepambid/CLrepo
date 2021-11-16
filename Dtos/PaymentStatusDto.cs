using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class PaymentStatusDto
    {
        public int PaymentId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
