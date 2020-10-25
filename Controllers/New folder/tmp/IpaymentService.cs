using AppZeroAPI.Entities;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Models;
using AppZeroAPI.Repository;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces

{
    public interface IPaymentService
    {
         bool  addPaymentDetails(Payment payment);

        Task<BankResponse> makePayment(PayRequest payReq);

        Task<PaymentDetails> getPaymentDetails(int merchantID, String payID);

         int  authenticateApiKey(string apiKey);
    }
}
