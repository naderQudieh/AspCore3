using AppZeroAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppZeroAPI.Shared
{
    
    public class BaseModel
    {
        public DateTime date_created
        {
            get
            {
                this.date_created = this.dateCreated ?? DateTime.UtcNow;
                return this.date_created ;
            }

            set { this.dateCreated = value; }
        }
       
        public string rec_id { get; set; }
        private DateTime? dateCreated;

        internal BaseModel(BaseEntity entity)
        {
            rec_id = entity.rec_id; 
            date_created = entity.date_created;
        }

        internal BaseModel() { }
    }
   
    public class TokenResponseViewModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string Username { get; set; }

        public string ReturnUrl{get;set;}
    }

    public class ProductDto : BaseModel
    {
        internal ProductDto(Product productEntity) : base(productEntity)
        {
            name = productEntity.name;
            unit_price = productEntity.unit_price;
            description = productEntity.description;
            qty_in_stock = productEntity.qty_in_stock;
        }

        public ProductDto() { }

        internal Product  MapTo()
        {
            var Product = new Product
            {
                name = name,
                qty_in_stock = qty_in_stock,
                unit_price = unit_price,
                description = description 
            };

            return Product;
        } 

        public string name { get; set; }
        public decimal unit_price { get; set; }
        public string description { get; set; }
        public int qty_in_stock { get; set; }
    }
}
