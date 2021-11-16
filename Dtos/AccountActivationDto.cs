using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CustomerLoans.Enum.Enums;

namespace CustomerLoans.Dtos
{
    public class AccountActivationDto
    {
        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string SecretKey { get; set; }

        public string AccountStatus { get; set; }
        public string Message { get; set; }
    }
}
