﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class AddPaymentDtoResponse
    {
        public string Status { get; set; }
        public string LoanId { get; set; }
        public string ReferenceNumber { get; set; }
        public string PaymentDate { get; set; }
        public string Amount { get; set; }
        public string Message { get; set; }
    }
}
