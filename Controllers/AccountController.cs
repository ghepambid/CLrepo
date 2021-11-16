using CustomerLoans.Data;
using CustomerLoans.Dtos;
using CustomerLoans.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerLoans.Controllers
{
    [Route("api/loans")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private readonly 
            IAccountRepository _accountRepository;

        private string _accountNumber = "";
        private string _badRequestMessage = "";
        private string _notFoundMessage = "";

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        private string AccountNumber { get => _accountNumber; set => value = _accountNumber; }
        private string BadRequestMessage { get => _badRequestMessage; set => value = _badRequestMessage; }
        private string NotFoundRequestMessage { get => _notFoundMessage; set => value = _notFoundMessage; }

        private RequestResponseDto GetBadRequestMessage()
        {
            return new RequestResponseDto()
            {
                Title = "Bad Request",
                StatusCode = StatusCodes.Status400BadRequest.ToString(),
                Message = BadRequestMessage
            };
        }

        private RequestResponseDto GetNotFoundRequestMessage()
        {
            return new RequestResponseDto()
            {
                Title = "Not Found",
                StatusCode = StatusCodes.Status404NotFound.ToString(),
                Message = NotFoundRequestMessage
            };
        }

        /// <summary>
        /// This method will handle the activation of account and resetting of pasword and secret key
        /// </summary>
        /// <param name="account"></param>
        /// <returns>
        /// returns a json
        /// returns status
        /// </returns>
        [Route("accounts")]
        [HttpPut]
        public async Task<IActionResult> Activate([FromBody] AccountDto account)
        {
            //check if payload has no value
            if (account == null)
            {
                _badRequestMessage = "Your request is invalid. Request body is empty.";
                return BadRequest(GetBadRequestMessage());
            }

            //check if account exist in the database
            var accountInDB = await _accountRepository.CheckCustomerAccount(account);

            //if account number not found in the database
            if (accountInDB == null)
            {
                _notFoundMessage = $"Account number {account.AccountNumber} not found.";
                return NotFound(GetNotFoundRequestMessage());
            }
            return Ok(accountInDB);
        }

        [Route("accounts")]
        [HttpGet]
        public async Task<IActionResult> GetAccountDetails()
        {
            //check if parameters has value
            if (Request.QueryString.HasValue)
            {
                //get parameters values
                string queryString = Request.QueryString.Value;

                string[] param = queryString.Split(":");

                //check the length, should be equal to 2
                if (param.Length < 1)
                {
                    //invalid or incomplete parameters
                    _badRequestMessage = "Invalid account or secret key.";
                    return BadRequest(GetBadRequestMessage());
                }
                _accountNumber = param[0].Substring(1, param[0].Length - 1);
                //validate account number and secret key
                bool bolIsAccountValid = await _accountRepository.
                                                IsAuthenticationValid(AccountNumber,
                                                                      param[1].ToString()).
                                                ConfigureAwait(false);
                if (!bolIsAccountValid)
                {
                    //supplied account and secret key is invalid
                    _notFoundMessage = "Incorrect account or secret key.";
                    return NotFound(GetNotFoundRequestMessage());
                }
            }
            else
            {
                //if required parameters are missing
                _badRequestMessage = "Account or secret key is invalid";
                return BadRequest(GetBadRequestMessage());
            }
            var account = await _accountRepository.GetAccountDetails(AccountNumber);

            return Ok(account);
        }

        /// <summary>
        /// This method will set the payment status to close
        /// </summary>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        [Route("payment/close")]
        [HttpPut]
        public async Task<IActionResult> ClosePayment(IEnumerable<PaymentStatusDto> paymentStatus)
        {
            //check if parameters has value
            if (Request.QueryString.HasValue)
            {
                //get parameters values
                string queryString = Request.QueryString.Value;

                string[] param = queryString.Split(":");

                //check the length, should be equal to 2
                if (param.Length < 1)
                {
                    //invalid or incomplete parameters
                    _badRequestMessage = "Invalid account or secret key.";
                    return BadRequest(GetBadRequestMessage());
                }
                _accountNumber = param[0].Substring(1, param[0].Length - 1);
                //validate account number and secret key
                bool bolIsAccountValid = await _accountRepository.
                                                IsAuthenticationValid(AccountNumber,
                                                                      param[1].ToString()).
                                                ConfigureAwait(false);
                if (!bolIsAccountValid)
                {
                    //supplied account and secret key is invalid
                    _notFoundMessage = "Incorrect account or secret key.";
                    return NotFound(GetNotFoundRequestMessage());
                }
            }
            else
            {
                //if required parameters are missing
                _badRequestMessage = "Account or secret key is invalid";
                return BadRequest(GetBadRequestMessage());
            }

            if (paymentStatus.Count() == 0)
            {
                _badRequestMessage = "Your request is invalid. Request body is empty.";
                return BadRequest(GetBadRequestMessage());
            }

            var result = await _accountRepository.ValidateAndUpdatePaymentStatus(paymentStatus, AccountNumber).ConfigureAwait(false);

            return Ok(result);
        }


        /// <summary>
        /// This method will update payment entries
        /// </summary>
        /// <param name="updatePaymentDto"></param>
        /// <returns></returns>
        [Route("payment")]
        [HttpPut]
        public async Task<IActionResult> UpdatePayment([FromBody] IEnumerable<UpdatePaymentDto> updatePaymentDto)
        {
            //check if parameters has value
            if (Request.QueryString.HasValue)
            {
                //get parameters values
                string queryString = Request.QueryString.Value;

                string[] param = queryString.Split(":");

                //check the length, should be equal to 2
                if (param.Length < 1)
                {
                    //invalid or incomplete parameters
                    _badRequestMessage = "Invalid account or secret key.";
                    return BadRequest(GetBadRequestMessage());
                }
                _accountNumber = param[0].Substring(1, param[0].Length - 1);
                //validate account number and secret key
                bool bolIsAccountValid = await _accountRepository.
                                                IsAuthenticationValid(AccountNumber,
                                                                      param[1].ToString()).
                                                ConfigureAwait(false);
                if (!bolIsAccountValid)
                {
                    //supplied account and secret key is invalid
                    _notFoundMessage = "Incorrect account or secret key.";
                    return NotFound(GetNotFoundRequestMessage());
                }
            }
            else
            {
                //if required parameters are missing
                _badRequestMessage = "Account or secret key is invalid";
                return BadRequest(GetBadRequestMessage());
            }

            if (updatePaymentDto.Count() == 0)
            {
                _badRequestMessage = "Your request is invalid. Request body is empty.";
                return BadRequest(GetBadRequestMessage());
            }

            //if parameters are all present and valid. check payment entries
            var entries = await _accountRepository.ValidateUpdateEntries(updatePaymentDto).ConfigureAwait(false);

            var payment = await _accountRepository.UpdatePayment(updatePaymentDto, entries, AccountNumber).ConfigureAwait(false);
            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        /// <summary>
        /// This method will return all customers loan including it's payment details/history.
        /// Payment details is sorted by the newest payment date.
        /// </summary>
        /// <param name="accountNumber:secretKey">
        /// A key pair should be included in the query string with (:) delimeter
        /// accountNumber = is the customers account number
        /// secretKey = is generated upon activation of the account
        /// </param>
        /// <returns>
        /// Returns Ok Status with the list of customers loans(in json format)
        /// Return NotFound Status, if customer account not found
        /// Return Bad request, if parameters are not supplied
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetLoans()
        {
            //check if parameters has value
            if (Request.QueryString.HasValue)
            {
                //get parameters values
                string queryString = Request.QueryString.Value;

                string[] param = queryString.Split(":");

                //check the length, should be equal to 2
                if (param.Length < 1)
                {
                    //invalid or incomplete parameters
                    _badRequestMessage = "Invalid account or secret key.";
                    return BadRequest(GetBadRequestMessage());
                }
                _accountNumber = param[0].Substring(1, param[0].Length - 1);
                //validate account number and secret key
                bool bolIsAccountValid = await _accountRepository.
                                                IsAuthenticationValid(AccountNumber,
                                                                      param[1].ToString()).
                                                ConfigureAwait(false);
                if (!bolIsAccountValid)
                {
                    //supplied account and secret key is invalid
                    _notFoundMessage = _accountRepository.ErrorMessage;
                    return NotFound(GetNotFoundRequestMessage());
                }
            }
            else
            {
                //if required parameters are missing
                _badRequestMessage = "Account or secret key is missing";
                return BadRequest(GetBadRequestMessage());
            }

            var result = await _accountRepository.GetLoans(AccountNumber);
            return Ok(result);
        }

        
        /// <summary>
        /// This method will create/post new payment
        /// </summary>
        /// <param name="addPaymentDto"></param>
        /// <returns></returns>
        [Route("payment")]
        [HttpPost]
        public async Task<IActionResult> PostPayment([FromBody] IEnumerable<AddPaymentDto> addPaymentDto)
        {
            //check if parameters has value
            if (Request.QueryString.HasValue)
            {
                //get parameters values
                string queryString = Request.QueryString.Value;

                string[] param = queryString.Split(":");

                //check the length, should be equal to 2
                if (param.Length < 1)
                {
                    //invalid or incomplete parameters
                    _badRequestMessage = "Invalid account or secret key.";
                    return BadRequest(GetBadRequestMessage());
                }
                _accountNumber = param[0].Substring(1, param[0].Length - 1);
                //validate account number and secret key
                bool bolIsAccountValid = await _accountRepository.
                                                IsAuthenticationValid(AccountNumber,
                                                                      param[1].ToString()).
                                                ConfigureAwait(false);
                if (!bolIsAccountValid)
                {
                    //supplied account and secret key is invalid
                    _notFoundMessage = "Incorrect account or secret key.";
                    return NotFound(GetNotFoundRequestMessage());
                }

            }
            else
            {
                //if required parameters are missing
                _badRequestMessage = "Account or secret key is invalid";
                return BadRequest(GetBadRequestMessage());
            }

            if (addPaymentDto.Count() == 0)
            {
                _badRequestMessage = "Your request is invalid. Request body is empty.";
                return BadRequest(GetBadRequestMessage());
            }

            var entries = await Task.Run(() => _accountRepository.ValidateEntries(addPaymentDto)).ConfigureAwait(false);

            var payment = await _accountRepository.PostPayment(addPaymentDto, entries, AccountNumber);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }
    }
}
