using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Dapper.FluentMap;
using Dapper;
using Dapper.FluentMap.Mapping;

namespace AppZeroAPI.Setup
{
    public static class FluentMapperConfig
    {
        public static void ConfigureFluentMapper(this IServiceCollection services)
        { 
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new UserProfileMap());
            });
        } 
    }
  
public class UserProfileMap : EntityMap<UserProfile>
    {
        public UserProfileMap()
        { 
            // Map(X => X.date_modified).ToColumn("date_modified");
            // Map(p => p.verified).Ignore();

        }
    } 
}
