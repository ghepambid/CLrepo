using CustomerLoans.Dtos;
using CustomerLoans.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.IRepository
{
    public interface IAccountRepository
    {
        Task<AccountActivationDto> CheckCustomerAccount(AccountDto accountDto);

        Task<AccountLoanDto> GetLoans(string accountNumber);

        Task<IEnumerable<AddPaymentDtoResponse>> PostPayment(IEnumerable<AddPaymentDto> payment,
                                     IEnumerable<AddPaymentDtoResponse> validatedEntries,
                                     string accountNumber);

        Task<IEnumerable<UpdatePaymentDtoResponse>> UpdatePayment(IEnumerable<UpdatePaymentDto> payment,
                                     IEnumerable<UpdatePaymentDtoResponse> validatedEntries,
                                     string accountNumber);

        Task<IEnumerable<AddPaymentDtoResponse>> ValidateEntries(IEnumerable<AddPaymentDto> payment);

        Task<IEnumerable<UpdatePaymentDtoResponse>> ValidateUpdateEntries(IEnumerable<UpdatePaymentDto> payment);

        Task<IEnumerable<PaymentStatusDto>> ValidateAndUpdatePaymentStatus(IEnumerable<PaymentStatusDto> payment, string accountNumber);

        Task<bool> IsAuthenticationValid(string accountNumber, string secretKey);

        Task<AccountActivationDto> GetAccountDetails(string accountNumber);

        string ErrorMessage { get; set; }
    }
}
