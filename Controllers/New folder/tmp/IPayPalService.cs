 
using System.Threading.Tasks;
using AppZeroAPI.Models;

namespace BrownNews.Services
{
    public interface IPayPalService
    {
        Task<string> ProcessPayment(PayPalTestViewModel model);
    }
}
