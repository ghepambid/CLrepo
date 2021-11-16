using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Enum
{
    public class Enums
    {
        public enum AccountStatus { InActive = 0, Active = 1 }
        public enum LoanType { Personal = 1, Salary = 2, Others = 3 }
        public enum RequestType { Activation = 1, Reset = 2, Others = 3 }
        public enum PaymentStatus { Closed = 0, Open = 1 }
    }
}
