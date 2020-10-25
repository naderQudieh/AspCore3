using ASP.NETCoreAuthWebApi.DataAcces;
using ASP.NETCoreAuthWebApi.DTOs;
using ASP.NETCoreAuthWebApi.Models;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NETCoreAuthWebApi.Services
{
    public class ProductServices
    {
        private readonly ProductsContext context;
        public ProductServices(ProductsContext context)
        {
            this.context = context;
        }
        public async Task<bool> ProductAddToCart(ProductDTOs product)
        {
            var foundProduct = await context.Products.SingleOrDefaultAsync(x => x.Name == product.Name && x.Description == product.Description && x.Coast == product.Coast);
            if(foundProduct == null)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Product>> GetProducts()
        {
            List<Product> products = await context.Products.ToListAsync();
            return products;
        }

        public string PaymenCreate(List<Product> products)
        {
            int total = 0;
            List<Item> items = new List<Item>();
            RedirectUrls urls = new RedirectUrls
            {
                return_url = "https://localhost:44368/return",
                cancel_url = "https://localhost:44368/cancel"
            };

            foreach(var element in products)
            {
                total += element.Coast;
                Item item = new Item() { quantity = "1", name = element.Name, currency = "USD", description = element.Description, sku = element.Name, price = element.Coast.ToString() };
                items.Add(item);
            }


            Amount amount = new Amount()
            {
                currency = "USD",
                total = total.ToString(),
            };

            var accessToken = new OAuthTokenCredential("Aay1UvDTKiPQubWqySy0N5eZNXfoKRt-pTIxE3FeQHYBH64VRlr4MRox_-eEsP1IVU5lB87XohzMT2Mv",
            "EGd82QV-Ifk_gjXLEJzH3D-U4XdMTfEcnUz-FYB0vULCtQLZcvRr49yfCrZbtxAki0PhE8ZdX31CWkQO").GetAccessToken();
            var apiContext = new APIContext(accessToken);

            // Make an API call
            var payment = Payment.Create(apiContext, new Payment
            {
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = new List<Transaction>
    {
        new Transaction
        {
            description = "Покупка с сайта.",
            invoice_number = "102 типо",
            amount = amount,
            item_list = new ItemList  {items = items}
        }},
         redirect_urls = urls
         });
            return payment.GetApprovalUrl();
        }
    }
}
