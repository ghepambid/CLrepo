using CustomerLoans.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static CustomerLoans.Enum.Enums;

namespace CustomerLoans.Models
{
    
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [StringLength(15)]
        public string AccountNumber { get; set; }

        [StringLength(100)]
        public string AccountName { get; set; }

        [StringLength(250)]
        public string Password { get; set; }
        public string SecretKey { get; set; }
        public AccountStatus AccountStatus { get; set; }

        public virtual List<Loan> Loans { get; set; }
    }
}
