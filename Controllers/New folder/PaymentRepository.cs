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

namespace AppZeroAPI.Repository
{
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {

        private readonly ILogger<PaymentRepository> logger;
        public PaymentRepository(IConfiguration configuration, ILogger<PaymentRepository> logger) : base(configuration)
        {
            this.logger = logger;
        }

        public bool getId(int id)
        {
             throw new ArgumentNullException();
        }
        public bool  addPaymentDetails(Payment payment)
        {
            

            return true;
        }

        public  int  authenticateApiKey(string apiKey)
        {
            int merchantID;
            try
            {
                return -1;// merchantID = await _context.Merchant.Where(e => e.Apikey == apiKey).Select(e=>e.Merchantid).FirstOrDefaultAsync();
            }
            catch (ArgumentNullException)
            {
                return -1;
            }
            
        }
       
        public   Task<PaymentDetails> getPaymentDetails(int merchantID, string payID)
        {
            
            try
            {

                return null;// await _context.Payment.Where(p => p.Paymentid == payID && p.Merchantid== merchantID).FirstOrDefaultAsync();
            }
            catch (ArgumentNullException)
            {
                throw new Exception("Invalid identifier or not authorized!");
            }
            catch(Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw new Exception("An error occured");
            }
            
        }

      

       

    }
}
