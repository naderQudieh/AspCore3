using AppZeroAPI.Interfaces;
using AppZeroAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppZeroAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        
        public IProductRepository Products { get; }
        public IUserRepository Users { get; }
        
        public UnitOfWork(  IProductRepository productRepository, IUserRepository userRepository )
        { 
            Products = productRepository;
            Users = userRepository;
            
        }
      
    }
}
