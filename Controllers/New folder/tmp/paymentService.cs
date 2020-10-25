using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Repository;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Repository
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentRepository> _logger;
        public PaymentService(IPaymentRepository paymentRepository, ILogger<PaymentRepository> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }
        public  bool  addPaymentDetails(Payment payment)
        {
            return   _paymentRepository.addPaymentDetails(payment);
        }

        public  int  authenticateApiKey(string apiKey)
        {
            try
            {
                return   _paymentRepository.authenticateApiKey(apiKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw new Exception("An error occured");
            }
        }

        public async Task<PaymentDetails>   getPaymentDetails(int merchantID, string payID)
        {
            PaymentDetails payment =  await _paymentRepository.getPaymentDetails(merchantID, payID);
            if (payment == null)
            {
                throw new Exception("Invalid identifier or not authorized!");
            }
            string cardNo = "";
            for(int i = 0; i < payment.Cardnumber.Length - 4; i++)
            {
                cardNo += "*";
            }
            cardNo +=  payment.Cardnumber.Substring(payment.Cardnumber.Length-4, 4);
            PaymentDetails paymentDetails = new PaymentDetails(payment.Paymentid, payment.Merchantid,cardNo, payment.Cardholdername, payment.Cardtype, payment.Expirydate, payment.Status, payment.Amount, payment.Paymentdate, payment.Currency);
            return paymentDetails;
        }

        public async Task<BankResponse> makePayment(PayRequest payReq)
        {
            
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(payReq), Encoding.UTF8, "application/json");
                try
                {
                    using (var response = await httpClient.PostAsync("https://localhost:44360/api/Bank", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        try
                        {
                            BankResponse bankResponse = new BankResponse();
                            bankResponse = JsonConvert.DeserializeObject<BankResponse>(apiResponse);
                            return bankResponse;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.StackTrace);
                            throw new Exception("An error occured");
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                    throw new Exception("Error: Could not connect to bank service");
                }
               
            }
            
        }
    }
}
