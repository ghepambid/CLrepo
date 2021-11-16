using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Dtos
{
    public class AccountDto
    {
        public string AccountNumber { get; set; }
        public string Password { get; set; }

        public int RequestType { get; set; } // 1 = For Activation, 2 = Reset password and secret key
    }
}
