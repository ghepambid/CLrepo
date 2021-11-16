This application is just a simple demonstration of how to manage customer loans using web api call. The application
was built using ASP.Net core web API. It also uses MS SQL for as data storage.

.Net files dependencies
1. Microsoft.AspNetCore.Mvc.NewtonsoftJson (5.0.11)
2. Microsoft.EntityFrameworkCore (5.0.11)
3. Microsoft.EntityFrameworkCore.SqlServer (5.0.11)
4. Microsoft.EntityFrameworkCore.Tools (5.0.11)


Configure to run on your local machine
1. In appsettings.json, change (local sql server) with the name of your ms sql server name.
	- "MyDB": "Data Source = (local sql server); Initial Catalog = CustomerLoans; Integrated Security=True;MultipleActiveResultSets=true;",
2. In Package console manager, create the database by typing update-database (install .net dependencies if necessary). If this is the first
   time to run the migration, the application will seed the customers data for testing.
3. Run the application and leave it open
4. Test using Postman(test using version 8.12.5)
   - Test data were seeded when we first run the migration, however customer account needs to be activated before making any request.
   
   - How to activate account
	 To activate customer account use the ff configuration
	 Verb/Action: Put
	 Path: https://localhost:{portno}/api/loans/accounts
	 
	 Request Body
	 Field			Type		Description
	 accountNumber	string		Customer's account number
	 password		string		password
	 requestType	int			1 = Activation, for activation of account
								2 = Reset, for resetting of password
								3 = Others, for resetting of secretKey (Not yet implemented)

	 ex: request Body: Json format
					{
						"accountNumber": "AN7103327640640",  // Refer to accounts.txt for list of account number
						"password": "password",				 // Set customer password
						"requestType": 1                     // Request Types: Activation = 1, Reset = 2
					}
	 
	 Response 1: when successfully activated, please keep a copy of the secret key as we will be needing it everytime we makes an API request call except for the activation of account
		{
			"accountNumber": "AN7103327640640",
			"accountName": "Nate Collins",
			"secretKey": "1Xu7Q7zPxU2tSVTitLAA+Q==",
			"accountStatus": "Active",
			"message": "Your account was successfully activated."
		}
	
	 Response 2: when account is already active
		{
			"accountNumber": "AN7103327640640",
			"accountName": "Nate Collins",
			"secretKey": null,
			"accountStatus": "Active",
			"message": "Your account is already activated."
		}
	
	 Response 3: when account number not found, does not exist in the database
		{
			"title": "Not Found",
			"statusCode": "404",
			"message": "Account number AN7103327640640222 not found."
		}

   - How to display/view loan details
	 To view loan details use the ff configuration
	 Verb/Action: Get
	 Path: https://localhost:{portno}/api/loans?AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q==
	 Query string: AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q== , should be colon(:) delimeted
				   where  AN7103327640640 is a customer's account number
						  1Xu7Q7zPxU2tSVTitLAA+Q== is the customer's secret key that was created upon activation of account
	 Request Body: N/A

	 Response 1: when customer account found and if customer has an existing loan. payment details is sorted by newest payment date
		{
            "accountId": 1,
            "accountNumber": "AN7103327640640",
            "accountName": "Nate Collins",
            "accountStatus": "Active",
            "loans": [
                {
                    "loanId": 1,
                    "loanType": "Personal",
                    "loanDate": "1/10/2021",
                    "principalAmount": 50000.00,
                    "interest": 3000.00,
                    "totalLoanAmount": 53000.00,
                    "balance": 51550.00,
                    "totalPayment": 1450.00,
                    "payments": [
                        {
                            "paymentId": 4,
                            "paymentDate": "11/15/2021",
                            "amount": 100.00,
                            "status": "Open",
                            "remarks": "Payment for November 2021",
                            "referenceNumber": "RN999999"
                        },
                        {
                            "paymentId": 3,
                            "paymentDate": "11/10/2021",
                            "amount": 350.00,
                            "status": "Open",
                            "remarks": "Update Payment for November 2021",
                            "referenceNumber": "55555"
                        },
                        {
                            "paymentId": 2,
                            "paymentDate": "03/10/2021",
                            "amount": 500.00,
                            "status": "Open",
                            "remarks": null,
                            "referenceNumber": "123"
                        },
                        {
                            "paymentId": 1,
                            "paymentDate": "02/10/2021",
                            "amount": 500.00,
                            "status": "Closed",
                            "remarks": ". Payment is confirmed.",
                            "referenceNumber": "456"
                        }
                    ]
                }
            ]
        }

		Response 2: if customer has multiple loans. payment is sorted by the newest payment date
		{
            "accountId": 1,
            "accountNumber": "AN7103327640640",
            "accountName": "Nate Collins",
            "accountStatus": "Active",
            "loans": [
                {
                    "loanId": 1,
                    "loanType": "Personal",
                    "loanDate": "1/10/2021",
                    "principalAmount": 50000.00,
                    "interest": 3000.00,
                    "totalLoanAmount": 53000.00,
                    "balance": 51550.00,
                    "totalPayment": 1450.00,
                    "payments": [
                        {
                            "paymentId": 4,
                            "paymentDate": "11/15/2021",
                            "amount": 100.00,
                            "status": "Open",
                            "remarks": "Payment for November 2021",
                            "referenceNumber": "RN999999"
                        },
                        {
                            "paymentId": 3,
                            "paymentDate": "11/10/2021",
                            "amount": 350.00,
                            "status": "Open",
                            "remarks": "Update Payment for November 2021",
                            "referenceNumber": "55555"
                        },
                        {
                            "paymentId": 2,
                            "paymentDate": "03/10/2021",
                            "amount": 500.00,
                            "status": "Open",
                            "remarks": null,
                            "referenceNumber": "123"
                        },
                        {
                            "paymentId": 1,
                            "paymentDate": "02/10/2021",
                            "amount": 500.00,
                            "status": "Closed",
                            "remarks": ". Payment is confirmed.",
                            "referenceNumber": "456"
                        }
                    ]
                }
            ]
        }

      - How to post/create new payment
        To create/post new payment use the ff configurations
	     Verb/Action: Post
	     Path: https://localhost:{portno}/api/loans/payment?AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q==
	     Query string: AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q== , should be colon(:) delimeted
				       where  AN7103327640640 is a customer's account number
						      1Xu7Q7zPxU2tSVTitLAA+Q== is the customer's secret key that was created upon activation of account
         Request Body
	     Field			 Type		    Description
	     loanId 	     int		    Customer's loan unique identifier
	     message		 string		    Customer's note
         paymentDate     date           Date of payment
         amount          decimal(18,2)  Amount paid
         referenceNumber string         Payment reference number(unique, duplicate not allowed)
         
         ex: request body: in json format
            for single loan payment request
                     [
                       {
    	                    "loanId": 1,
    	                    "Message": "Payment for November 2021",
    	                    "paymentDate": "11/15/2021",
    	                    "amount": 100.00,
    	                    "referenceNumber": "RN999999"
                       }
                    ]
            for multiple loan payment request
                    [
                        {
                            "loanId": 1,
                            "Message": "Payment for November 2021",
                            "paymentDate": "11/10/2021",
                            "amount": 500.00,
                            "referenceNumber": "RN123456"
                        },
                        {
                            "loanId": 2,
                            "Message": "Payment for November 2021",
                            "paymentDate": "11/10/2021",
                            "amount": 500.00,
                            "referenceNumber": "RN1234567"
                        }
                    ]
        
        Response 1: if single payment reques was successfully save/created
                [
                    {
                        "status": "success: Payment was successfully saved",
                        "loanId": "1",
                        "referenceNumber": "RN000100",
                        "paymentDate": "11/11/2021",
                        "amount": "100.00",
                        "message": "Payment for November 2021"
                    }
                ]

        Response 2: if multiple payment request was successfully save/created 
                [
                    {
                        "status": "success: Payment was successfully saved",
                        "loanId": "1",
                        "referenceNumber": "RN000223124",
                        "paymentDate": "11/12/2021",
                        "amount": "100.00",
                        "message": "Payment for November 2021"
                    },
                    {
                        "status": "success: Payment was successfully saved",
                        "loanId": "2",
                        "referenceNumber": "RN888812421",
                        "paymentDate": "11/12/2021",
                        "amount": "100.00",
                        "message": "Payment for November 2021"
                    }
                ]

        Response 3: if single payment request with error
            [
                {
                    "status": "An error has encountered while processing this request. Please correct entries with error and submit again.",
                    "loanId": "1",
                    "referenceNumber": "error => reference number RN999999 is already in use.",
                    "paymentDate": "11/15/2021",
                    "amount": "100.00",
                    "message": "Payment for November 2021"
                }
            ]

       Response 4: if multiple payment request errors
            [
                {
                    "status": "An error has encountered while processing this request. Please correct entries with error and submit again.",
                    "loanId": "1",
                    "referenceNumber": "error => reference number RN999999 is already in use.",
                    "paymentDate": "11/15/2021",
                    "amount": "100.00",
                    "message": "Payment for November 2021"
                },
                {
                    "status": "An error has encountered while processing this request. Please correct entries with error and submit again.",
                    "loanId": "2",
                    "referenceNumber": "error => reference number RN88888888 is already in use.",
                    "paymentDate": "11/15/2021",
                    "amount": "100.00",
                    "message": "Payment for November 2021"
                }
            ]

        Response 5: multiple payment request with success and error message
            [
                {
                    "status": "An error has encountered while processing this request. Please correct entries with error and submit again.",
                    "loanId": "1",
                    "referenceNumber": "error => reference number RN000223124 is already in use.",
                    "paymentDate": "11/12/2021",
                    "amount": "100.00",
                    "message": "Payment for November 2021"
                },
                {
                    "status": "success: Payment was successfully saved",
                    "loanId": "2",
                    "referenceNumber": "RN888812465",
                    "paymentDate": "11/13/2021",
                    "amount": "100.00",
                    "message": "Payment for November 2021"
                }
            ]

      - How to update payment 
        To update/put/modify existing payment use the ff configurations
	     Verb/Action: Put
	     Path: https://localhost:{portno}/api/loans/payment?AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q==
	     Query string: AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q== , should be colon(:) delimeted
				       where  AN7103327640640 is a customer's account number
						      1Xu7Q7zPxU2tSVTitLAA+Q== is the customer's secret key that was created upon activation of account
         Request Body
	     Field			 Type		    Description
	     paymentId 	     int		    Customer's payment unique identifier
	     message		 string		    Customer's note
         paymentDate     date           Date of payment
         amount          decimal(18,2)  Amount paid
         referenceNumber string         Payment reference number(unique, duplicate not allowed)

         ex: request body 
          for single payment update request
            [
                {
                    "paymentId": 3,
                    "paymentDate": "2021-11-10",
                    "amount": 350.00,
                    "message": "Update Payment for November 2021",
                    "referenceNumber": "55555"
                }
            ]

        for multiple payment update request
            [
                {
                    "paymentId": 3,
                    "paymentDate": "2021-11-10",
                    "amount": 350.00,
                    "message": "Update Payment for November 2021",
                    "referenceNumber": "55555"
                },
                {
                    "paymentId": 4,
                    "paymentDate": "2021-11-10",
                    "amount": 350.00,
                    "message": "Update Payment for November 2021",
                    "referenceNumber": "888888"
                }
            ]

        Response 1: success response
            [
                {
                    "status": "success: Payment details was successfully updated.",
                    "paymentId": "1",
                    "referenceNumber": "RNB0001",
                    "paymentDate": "11/10/2021",
                    "amount": "350.00",
                    "message": "Update Payment for November 2021"
                }
            ]

        Response 2: with error response
            [
                {
                    "status": "An error has encountered while processing this request. Please correct entries with error and submit again.",
                    "paymentId": "1",
                    "referenceNumber": "error => reference number 55555 is already in use.",
                    "paymentDate": "11/10/2021",
                    "amount": "350.00",
                    "message": "Update Payment for November 2021"
                }
            ]

       - How to close payment status
        To close existing payment use the ff configurations
	     Verb/Action: Put
	     Path: https://localhost:{portno}/api/loans/payment/close?AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q==
	     Query string: AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q== , should be colon(:) delimeted
				       where  AN7103327640640 is a customer's account number
						      1Xu7Q7zPxU2tSVTitLAA+Q== is the customer's secret key that was created upon activation of account
         Request Body
	     Field			 Type		    Description
	     paymentId 	     int		    Customer's payment unique identifier
	     message		 string		    Customer's note
         

         ex: request body
           for single payment request
            [
               {
    	            "paymentId": 1,
    	            "Message": "Payment is confirmed."
               }
            ]

            for single payment request
            [
               {
    	            "paymentId": 1,
    	            "Message": "Payment is confirmed."
               },
               {
    	            "paymentId": 2,
    	            "Message": "Payment is confirmed."
               }
            ]

        Response 1: success response
            [
                {
                    "paymentId": 1,
                    "message": "Payment is confirmed.",
                    "status": "success. Payment was successfully closed."
                }
            ]


       Response 2: with error response
            [
                {
                    "paymentId": 1,
                    "message": "Payment is confirmed.",
                    "status": "failed. Cannot update, payment status is closed already."
                }
            ]

       
    - - How to retrieve accounts including security key 
        To retrieve account details use the ff configurations
	     Verb/Action: Get
	     Path: https://localhost:{portno}/api/loans/accounts?AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q==
	     Query string: AN7103327640640:1Xu7Q7zPxU2tSVTitLAA+Q== , should be colon(:) delimeted
				       where  AN7103327640640 is a customer's account number
						      1Xu7Q7zPxU2tSVTitLAA+Q== is the customer's secret key that was created upon activation of account
         Request Body: N/A

         Response:
                {
                    "accountNumber": "AN7103327640640",
                    "accountName": "Nate Collins",
                    "secretKey": "1Xu7Q7zPxU2tSVTitLAA+Q==",
                    "accountStatus": "Active",
                    "message": null
                }