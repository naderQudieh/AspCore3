using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AutoMapperConfiguration = AutoMapper.Configuration;
using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
 

namespace AppZeroAPI.Setup
{
    public static class AutoMapperConfig
    {
        public static void ConfigureAutoMapper(this IServiceCollection services )
        {
            services.AddAutoMapper(typeof(Startup));
            //var mapperConfig = new MapperConfiguration(mc =>
            //{
            //    mc.AddProfile(new MappingProfile());
            //});
        }
       
        
        
    }
    public class MappingProfile : Profile
    {
        //AutoMapper will scan our application and look for 
        //classes that inherit from the Profile class and load their mapping configurations.
        public MappingProfile()
        {
             CreateMap<UserProfile, UserInfo>();
            //CreateMap<UserProfile, UserInfo>()
            //      .ForMember(dest => dest.fname,opt => opt.MapFrom<string>((src, dst) =>
            //      {
            //          if ( string.IsNullOrEmpty(src.fname))
            //          {
            //              return $"fname_{src.user_id.ToString() }";
            //          }
            //          else
            //          {
            //              return src.fname;
            //          }
            //      })).ForMember(dest => dest.role, opt => opt.MapFrom<string>((src, dst) =>
            //      {
            //          if (string.IsNullOrEmpty(src.role))
            //          {
            //              return $"1";
            //          }
            //          else
            //          {
            //              return src.role;
            //          }
            //      }))
            //      .ForMember(dest => dest.lname,opt => opt.MapFrom(src => src.user_id.ToString())
            // ).AfterMap((src, dest) =>
            // {
            //     dest.lname  = "lname"; 

            // });
        }
    }
    
   
}
