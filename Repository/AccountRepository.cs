using CustomerLoans.Data;
using CustomerLoans.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomerLoans.Models;
using AutoMapper;
using CustomerLoans.IRepository;
using CustomerLoans.Enum;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace CustomerLoans.Repository
{
    public class AccountRepository : IAccountRepository
    {

        private readonly IDbContextFactory<ApplicationDBContext> _dbContextFactory;

        public string _errorMessage = "";

        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        public AccountRepository(IDbContextFactory<ApplicationDBContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        private string GenerateSecretKey(int accountId, string accountNumber)
        {
            string retValue = "";

            retValue = Utilities.CryptoHelper.Encrypt(accountNumber, accountNumber);

            return retValue;
        }

        private bool ActivateAccount(int accountId)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                if (result != null)
                {
                    result.AccountStatus = Enums.AccountStatus.Active;//set the account status to active
                    //update the database
                    context.SaveChangesAsync();
                    bolValue = true;
                }
            }
            return bolValue;
        }


        private async Task<bool> IsSecretKeyValid(string accountNumber, string usersecretKey)
        {
            bool bolValue = false;
            string secretKey = await Task.Run(() => Utilities.CryptoHelper.Encrypt(accountNumber, accountNumber)).ConfigureAwait(false);
            if (secretKey == usersecretKey)
                bolValue = true;
            return bolValue;
        }

        private async Task<bool> CheckCustomerAccount(string accountNumber)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Accounts
                                          .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
                if (result != null)
                    bolValue = true;
            }
            return bolValue;
        }


        public async Task<AccountActivationDto> GetAccountDetails(string accountNumber)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Accounts
                                           .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

                if (result != null)
                {
                    
                        //account is already active. just display account's information with a message
                        //informing the user that account is already active.
                        AccountActivationDto response = new AccountActivationDto();
                        response.AccountName = result.AccountName;
                        response.AccountNumber = result.AccountNumber;
                        response.AccountStatus = result.AccountStatus.ToString();
                        response.SecretKey = result.SecretKey;
                        //response.Message = "Your account is already activated.";
                        return response;
                    
                }
                else
                {
                    _errorMessage = "Account number not found.";
                }
                return null;
            }
        }

        public async Task<AccountActivationDto> CheckCustomerAccount(AccountDto account)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Accounts
                                           .FirstOrDefaultAsync(a => a.AccountNumber == account.AccountNumber);

                if (result != null)
                {
                    //if account exist in db, check status if not yet active. If account is already active do nothing
                    if (account.RequestType == (int)Enums.RequestType.Activation && result.AccountStatus != Enums.AccountStatus.Active)
                    {
                        //try to activate customer account
                        try
                        {
                            //Generate customer's secret key. this secret key together with the account number 
                            //must be supplied in the query string everytime a customer 
                            //makes an API request, except for activation of account.
                            string secretKey = Utilities.CryptoHelper.Encrypt(result.AccountNumber, result.AccountNumber);
                            string password = Utilities.CryptoHelper.Encrypt("password", account.Password);
                            if (secretKey != "")
                            {
                                //update the model values
                                result.AccountStatus = Enums.AccountStatus.Active;
                                result.Password = password;
                                result.SecretKey = secretKey;

                                //commit changes to database
                                await context.SaveChangesAsync();

                                //set response values
                                AccountActivationDto response = new AccountActivationDto();
                                response.AccountName = result.AccountName;
                                response.AccountNumber = result.AccountNumber;
                                response.AccountStatus = result.AccountStatus.ToString();
                                //response.Password = account.Password;
                                response.SecretKey = secretKey;
                                response.Message = "Your account was successfully activated.";

                                return response;
                            }
                        }
                        catch (Exception ex)
                        {
                            _errorMessage = ex.Message.ToString();
                        }
                    }
                    else
                    {
                        //account is already active. just display account's information with a message
                        //informing the user that account is already active.
                        AccountActivationDto response = new AccountActivationDto();
                        response.AccountName = result.AccountName;
                        response.AccountNumber = result.AccountNumber;
                        response.AccountStatus = result.AccountStatus.ToString();
                        response.SecretKey = result.SecretKey;
                        response.Message = "Your account is already activated.";
                        return response;
                    }
                }
                else
                {
                    _errorMessage = "Account number not found.";
                }
                return null;
            }
        }

        private async Task<IEnumerable<Payment>> GetPayments(int loanId)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                IQueryable<Payment> query = context.Payments;

                query = query.Where(p => p.LoanId == loanId);

                query = query.OrderByDescending(p => p.PaymentDate);

                var result = await query.ToListAsync();
                if (result != null)
                    return result;
            }
            return null;
        }

        public async Task<AccountLoanDto> GetLoans(string accountNumber)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Accounts
                                          .Include(loan => loan.Loans)
                                          .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
                if (result != null)
                {
                    AccountLoanDto loanDetails = new AccountLoanDto();
                    loanDetails.AccountId = result.AccountId;
                    loanDetails.AccountName = result.AccountName;
                    loanDetails.AccountNumber = result.AccountNumber;
                    loanDetails.AccountStatus = ((Enums.AccountStatus)result.AccountStatus).ToString();

                    //Loan details
                    foreach (var loan in result.Loans)
                    {
                        LoanDto objLoan = new LoanDto();

                        objLoan.LoanId = loan.LoanId;
                        objLoan.LoanType = ((Enums.LoanType)loan.LoanType).ToString();
                        objLoan.LoanDate = loan.LoanDate;
                        objLoan.PrincipalAmount = loan.LoanAmount;
                        objLoan.Interest = loan.LoanInterest;
                        objLoan.TotalLoanAmount = loan.LoanAmount + loan.LoanInterest;

                        //Loan payment details
                        IEnumerable<Payment> payment = await GetPayments(loan.LoanId);
                        decimal TotalPayment = 0;
                        if (payment != null)
                        {
                            foreach (var item in payment)
                            {
                                PaymentDto objPayment = new PaymentDto();
                                objPayment.PaymentId = item.PaymentId;
                                objPayment.PaymentDate = item.PaymentDate.ToString("MM/dd/yyyy");
                                objPayment.Amount = item.Amount;
                                objPayment.Status = ((Enums.PaymentStatus)item.Status).ToString();
                                objPayment.Remarks = item.Remarks;
                                objPayment.ReferenceNumber = item.ReferenceNumber;
                                TotalPayment += item.Amount;
                                objLoan.Payments.Add(objPayment);
                            }
                        }
                        objLoan.TotalPayment = TotalPayment;
                        objLoan.Balance = (loan.LoanAmount + loan.LoanInterest) - TotalPayment;

                        loanDetails.Loans.Add(objLoan);
                    }
                    return loanDetails;
                }
            }
            return null;
        }

        private async Task<bool> IsReferenceNumberInUse(string referenceNo)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {
                //var result = await Task.Run(() => context.Payments.FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNo)).ConfigureAwait(false);
                var result = await context.Payments.FirstOrDefaultAsync(
                                                    p => p.ReferenceNumber == referenceNo);
                if (result != null)
                    bolValue = true;
            }
            return bolValue;
        }

        private async Task<bool> IsReferenceNumberInUse(string referenceNumber, int paymentId)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {

                IQueryable<Payment> query = context.Payments;

                query = query.Where(p => p.PaymentId != paymentId &&
                                         p.ReferenceNumber == referenceNumber);

                var result = await query.ToListAsync();
                if (result != null)
                    if (result.Count() > 0)
                        bolValue = true;
            }
            return bolValue;
        }

        public async Task<IEnumerable<AddPaymentDtoResponse>> ValidateEntries(IEnumerable<AddPaymentDto> payment)
        {
            int errorCount = 0;
            List<String> lstRefNos = new List<String>();
            List<AddPaymentDtoResponse> addPayments = new List<AddPaymentDtoResponse>();
            string error = "";
            //Validate payment entrie/s.
            //1. all fields must not be empty
            //2. Reference number must be unique
            //3. Amount is greater than zero
            //4. Payment date should not be later/greater than the current date
            //5. Loan id must be greater zero
            foreach (var item in payment)
            {
                int errCtr = 0;
                error = "";
                AddPaymentDtoResponse objPayment = new AddPaymentDtoResponse();
                if (Convert.ToInt32(item.LoanId) < 1)
                {
                    objPayment.LoanId = $"error => Loan id {item.LoanId.ToString()} is invalid.";
                    errCtr++;
                }

                if (item.ReferenceNumber == string.Empty)
                {
                    objPayment.ReferenceNumber = "error => this field is required.";
                    errCtr++;
                }
                else
                {
                    //check reference number in the database if already in use. reference number should be unique accross account number
                    //var test = await IsReferenceNumberInUse(item.ReferenceNumber); ==>working
                    bool bolRefNoInUse = await IsReferenceNumberInUse(item.ReferenceNumber);
                    if (lstRefNos.Count == 0)
                    {
                        lstRefNos.Add(item.ReferenceNumber);
                        if (bolRefNoInUse)
                        {
                            objPayment.ReferenceNumber = $"error => reference number {item.ReferenceNumber} is already in use.";
                            errCtr++;
                        }
                        else
                            objPayment.ReferenceNumber = item.ReferenceNumber;
                    }
                    else
                    {

                        if (bolRefNoInUse)
                        {
                            objPayment.ReferenceNumber = $"error => reference number {item.ReferenceNumber} is already in use.";
                            errCtr++;
                        }
                        else
                        {
                            //iterate through the list of reference numbers and check if this current reference number is already added in the list
                            bool bolFound = false;
                            foreach (var refno in lstRefNos)
                            {
                                if (refno == item.ReferenceNumber)
                                {
                                    //if found, this reference number is already in use in the current request entries
                                    error = $"error => {item.ReferenceNumber} has duplicate entry in your request.";
                                    if (bolRefNoInUse)
                                        error += ". This reference number is already in use.";

                                    objPayment.ReferenceNumber = error;
                                    errCtr++;
                                    bolFound = true;
                                    break;
                                }
                            }

                            if (!bolFound)
                                objPayment.ReferenceNumber = item.ReferenceNumber;
                        }
                    }
                }

                if (item.PaymentDate > DateTime.Now)
                {
                    objPayment.PaymentDate = $"error => {item.PaymentDate.ToString("MM/dd/yyyy")} is invalid. Cannot be later than the current date.";
                    errCtr++;
                }
                else objPayment.PaymentDate = item.PaymentDate.ToString("MM/dd/yyyy");

                if (Convert.ToDecimal(item.Amount) <= 0)
                {
                    objPayment.Amount = "error => Value must be greater than zero.";
                    errCtr++;
                }
                else objPayment.Amount = item.Amount.ToString();

                if (errCtr > 0)
                {
                    objPayment.Status = "An error has encountered while processing this request. Please correct entries with error and submit again.";
                    errorCount++;
                }
                else
                    objPayment.Status = "success";

                if (objPayment.LoanId == null)
                    objPayment.LoanId = item.LoanId.ToString();

                if (objPayment.Amount == null)
                    objPayment.Amount = item.Amount.ToString();

                objPayment.Message = item.Message;
                addPayments.Add(objPayment);
            }

            return addPayments;
        }


        public async Task<IEnumerable<UpdatePaymentDtoResponse>> ValidateUpdateEntries(IEnumerable<UpdatePaymentDto> payment)
        {
            int errorCount = 0;
            List<String> lstRefNos = new List<String>();
            List<UpdatePaymentDtoResponse> addPayments = new List<UpdatePaymentDtoResponse>();
            string error = "";
            //Validate payment entrie/s.
            //1. all fields must not be empty
            //2. Reference number must be unique
            //3. Amount is greater than zero
            //4. Payment date should not be later/greater than the current date
            //5. Payment id must be greater zero
            //6. Payment statust is Open
            foreach (var item in payment)
            {
                int errCtr = 0;
                error = "";
                UpdatePaymentDtoResponse objPayment = new UpdatePaymentDtoResponse();
                if (Convert.ToInt32(item.PaymentId) < 1)
                {
                    objPayment.PaymentId = $"error => Payment id {item.PaymentId.ToString()} is invalid.";
                    errCtr++;
                }

                if (item.ReferenceNumber == string.Empty)
                {
                    objPayment.ReferenceNumber = "error => this field is required.";
                    errCtr++;
                }
                else
                {
                    //check reference number in the database if already in use. reference number should be unique accross account number
                    //var test = await IsReferenceNumberInUse(item.ReferenceNumber); ==> working
                    bool bolRefNoInUse = await IsReferenceNumberInUse(item.ReferenceNumber, item.PaymentId);
                    if (lstRefNos.Count == 0)
                    {
                        lstRefNos.Add(item.ReferenceNumber);
                        if (bolRefNoInUse)
                        {
                            objPayment.ReferenceNumber = $"error => reference number {item.ReferenceNumber} is already in use.";
                            errCtr++;
                        }
                        else
                            objPayment.ReferenceNumber = item.ReferenceNumber;
                    }
                    else
                    {
                        //iterate through the list of reference numbers and check if this current reference number is already added in the list
                        bool bolFound = false;
                        foreach (var refno in lstRefNos)
                        {
                            if (refno == item.ReferenceNumber)
                            {
                                //if found, this reference number is already in use in the current request entries
                                error = $"error => {item.ReferenceNumber} has duplicate entry in your request.";
                                if (bolRefNoInUse)
                                    error += ". This reference number is already in use.";

                                objPayment.ReferenceNumber = error;
                                errCtr++;
                                bolFound = true;
                                break;
                            }
                        }
                        if (!bolFound)
                            objPayment.ReferenceNumber = item.ReferenceNumber;
                    }
                }

                if (item.PaymentDate > DateTime.Now)
                {
                    objPayment.PaymentDate = $"error => {item.PaymentDate.ToString("MM/dd/yyyy")} is invalid. Cannot be later than the current date.";
                    errCtr++;
                }
                else objPayment.PaymentDate = item.PaymentDate.ToString("MM/dd/yyyy");

                if (Convert.ToDecimal(item.Amount) <= 0)
                {
                    objPayment.Amount = "error => Value must be greater than zero.";
                    errCtr++;
                }
                else objPayment.Amount = item.Amount.ToString();

                if (errCtr > 0)
                {
                    objPayment.Status = "An error has encountered while processing this request. Please correct entries with error and submit again.";
                    errorCount++;
                }
                else
                    objPayment.Status = "success";

                if (objPayment.PaymentId == null)
                    objPayment.PaymentId = item.PaymentId.ToString();

                if (objPayment.Amount == null)
                    objPayment.Amount = item.Amount.ToString();

                objPayment.Message = item.Message;
                addPayments.Add(objPayment);
            }
            return addPayments;
        }

        public async Task<IEnumerable<AddPaymentDtoResponse>> PostPayment(IEnumerable<AddPaymentDto> payment,
                                                  IEnumerable<AddPaymentDtoResponse> validatedEntries,
                                                  string accountNumber)
        {
            //Check and process only those validated entries with success status and insert it to database
            foreach (var item in payment)
            {
                foreach (var successEntry in validatedEntries)
                {
                    if (item.LoanId.ToString() == successEntry.LoanId.ToString() &&
                        item.ReferenceNumber == successEntry.ReferenceNumber)
                    {
                        if (successEntry.Status == "success")
                        {
                            //prepare record
                            Payment newPayment = new Payment();
                            newPayment.LoanId = item.LoanId;
                            newPayment.ReferenceNumber = item.ReferenceNumber;
                            newPayment.PaymentDate = item.PaymentDate;
                            newPayment.Remarks = item.Message;
                            newPayment.Status = Enums.PaymentStatus.Open;
                            newPayment.Amount = item.Amount;

                            using (var context = _dbContextFactory.CreateDbContext())
                            {
                                context.Payments.Add(newPayment); //add new entries
                                var result = await context.SaveChangesAsync().ConfigureAwait(false); //commit changes to database
                                if (result > 0)
                                {
                                    //Update validated entries status message
                                    successEntry.Status = "success: Payment was successfully saved";
                                }
                            }
                        }
                    }
                }
            }
            return validatedEntries;
        }

        public async Task<IEnumerable<UpdatePaymentDtoResponse>> UpdatePayment(IEnumerable<UpdatePaymentDto> payment,
                                                  IEnumerable<UpdatePaymentDtoResponse> validatedEntries,
                                                  string accountNumber)
        {
            //Check entries with success(no error) status only and update database
            foreach (var item in payment)
            {
                foreach (var successEntry in validatedEntries)
                {
                    if (item.PaymentId.ToString() == successEntry.PaymentId.ToString() &&
                        item.ReferenceNumber == successEntry.ReferenceNumber)
                    {
                        if (successEntry.Status == "success")
                        {
                            using (var context = _dbContextFactory.CreateDbContext())
                            {
                                var paymentInDB = await context.Payments
                                                               .FirstOrDefaultAsync(p => p.PaymentId == item.PaymentId);
                                if (paymentInDB != null)
                                {
                                    paymentInDB.ReferenceNumber = item.ReferenceNumber;
                                    paymentInDB.PaymentDate = item.PaymentDate;
                                    paymentInDB.Remarks = item.Message;
                                    paymentInDB.Amount = item.Amount;
                                }
                                var result = await context.SaveChangesAsync(); //commit changes to database
                                if (result > 0)
                                {
                                    //Update the status message
                                    successEntry.Status = "success: Payment details was successfully updated.";
                                }
                            }
                        }
                    }
                }
            }
            return validatedEntries;
        }


        private async Task<bool> UpdatePaymentStatus(int paymentId, Enum.Enums.PaymentStatus status, string closeRemark)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.Payments.Where(p => p.PaymentId == paymentId).FirstOrDefaultAsync();
                if (result != null)
                {
                    result.Status = status;
                    result.Remarks = result.Remarks + $". {closeRemark}";
                    try
                    {
                        await context.SaveChangesAsync();
                        bolValue = true;
                    }
                    catch (Exception ex)
                    {
                        _errorMessage = ex.Message.ToString();
                    }
                }
            }
            return bolValue;
        }

        public async Task<IEnumerable<PaymentStatusDto>> ValidateAndUpdatePaymentStatus(IEnumerable<PaymentStatusDto> payment, string accountNumber)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                foreach (var item in payment)
                {
                    //check loan owner, only payment under the underlying customer account must be closed/updated
                    var query = await context.Accounts
                                        .Where(a => a.AccountNumber == accountNumber)
                                        .Include(l => l.Loans)
                                            .ThenInclude(p => p.Payments)
                                        .FirstOrDefaultAsync();

                    foreach (Loan loan in query.Loans)
                    {
                        foreach (Payment payments in loan.Payments)
                        {
                            foreach (var custPayment in payment)
                            {
                                if (custPayment.PaymentId == payments.PaymentId)
                                {
                                    //if payment id found
                                    if (payments.Status == Enums.PaymentStatus.Closed)
                                        custPayment.Status = "failed. Cannot update, payment status is closed already.";
                                    else
                                    {
                                        bool bolStatus = await UpdatePaymentStatus(custPayment.PaymentId, Enums.PaymentStatus.Closed, custPayment.Message).ConfigureAwait(false);
                                        if (bolStatus)
                                            custPayment.Status = "success. Payment was successfully closed.";
                                        else
                                            custPayment.Status = "error. " + ErrorMessage;
                                    }
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            return payment;
        }

        private bool IsAccountLoanIdExist(string accountNumber, int loanId)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var result = context.Loans.FirstOrDefaultAsync(
                                       l => l.LoanId == loanId &&
                                       l.Account.AccountNumber == accountNumber);
                if (result != null)
                    bolValue = true;
            }

            return bolValue;
        }

        public async Task<bool> IsAuthenticationValid(string accountNumber, string secretKey)
        {
            bool bolValue = false;
            using (var context = _dbContextFactory.CreateDbContext())
            {
                //check if account exist in db

                IQueryable<Account> query = context.Accounts;

                query = query.Where(a => a.AccountNumber == accountNumber);

                if (query == null)
                {
                    _errorMessage = "Account number not found.";
                    return bolValue;
                }

                query = query.Where(a => a.AccountNumber == accountNumber &&
                                         a.AccountStatus == Enums.AccountStatus.Active);
                var result = await query.ToListAsync();
                if (result != null)
                    if (result.Count() < 1)
                    {
                        _errorMessage = "Account is not yet activated.";
                        return bolValue;
                    }

                //check if user's input secret key is matched with the secret key in the the database
                bool bolIsSecretKeyValid = await IsSecretKeyValid(accountNumber, secretKey);

                //if not valid
                if (!bolIsSecretKeyValid)
                {
                    _errorMessage = "Invalid secret key.";
                    return bolValue;
                }
                //if passed with the validations
                bolValue = true;
            }
            return bolValue;
        }
    }
}
