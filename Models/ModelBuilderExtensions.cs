using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CustomerLoans.Models
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(a =>
               new Account
               {
                   AccountId = 1,
                   AccountNumber = "AN7103327640640",
                   AccountName = "Nate Collins",
                   Password = null,
                   SecretKey = null,
                   AccountStatus = Enum.Enums.AccountStatus.InActive,
                   Loans = new List<Loan>
                   {
                        new Loan()
                        {
                            LoanId = 1,
                            LoanType = Enum.Enums.LoanType.Personal,
                            LoanAmount = 25000,
                            LoanInterest = 500,
                            LoanDate = "01/10/2021",
                            Payments = new List<Payment>
                            {
                                new Payment()
                                {
                                    PaymentId = 1,
                                    PaymentDate = Convert.ToDateTime("02/10/2021"),
                                    Amount = 5500,
                                    ReferenceNumber = "RN000001",
                                    Remarks = "Payment for February 2021",
                                    Status = Enum.Enums.PaymentStatus.Open,
                                    LoanId = 1
                                }
                            }
                        }
                   }
               });

            modelBuilder.Entity<Account>(a =>
               new Account
               {
                   AccountId = 2,
                   AccountNumber = "AN7103327640640",
                   AccountName = "Elly Grant",
                   Password = null,
                   SecretKey = null,
                   AccountStatus = Enum.Enums.AccountStatus.InActive,
                   Loans = new List<Loan>
                   {
                        new Loan()
                        {
                            LoanId = 2,
                            LoanType = Enum.Enums.LoanType.Personal,
                            LoanAmount = 25000,
                            LoanInterest = 500,
                            LoanDate = "01/10/2021",
                            Payments = new List<Payment>
                            {
                                new Payment()
                                {
                                    PaymentId = 2,
                                    PaymentDate = Convert.ToDateTime("02/10/2021"),
                                    Amount = 5500,
                                    ReferenceNumber = "RN000001",
                                    Remarks = "Payment for February 2021",
                                    Status = Enum.Enums.PaymentStatus.Open,
                                    LoanId = 2
                                }
                            }
                        }
                   }
               });
        }

    }
}
