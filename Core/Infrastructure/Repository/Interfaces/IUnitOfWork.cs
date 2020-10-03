using AppZeroAPI.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppZeroAPI.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IUserRepository Users { get; }
       
    }
    
}
