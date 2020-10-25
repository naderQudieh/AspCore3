using AppZeroAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Interfaces
{
    public interface IPaymentRepository
    {

        bool addPaymentDetails(Payment payment);


        Task<PaymentDetails>  getPaymentDetails(int merchantID, String payID) ;

        int  authenticateApiKey(string apiKey);

    }
}
