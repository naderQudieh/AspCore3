using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AppZeroAPI.Shared
{

    public static class Langauge
    {
        public const int Arabic = 1;
        public const int English = 2;
        public const int Spanish = 3;
        public const int French = 4;
    }
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Client = "Client";
    }
    public enum AuthType
    {
        Simple,
        Email,
        Phone
    }
   
    public enum TokenType : int
    {
        [Description("None")]
        None = 0,

        [Description("Bearer")]
        Bearer = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TokenStatus : int
    {
        [Description("None")]
        None = 0,

        [Description("Active")]
        Active = 1,

        [Description("Terminate")]
        Terminate = 2,

        [Description("Pending")]
        Pending = 3
    }
    public enum TokenConfiguration
    {
        Issuer,
        Audience,
        AccessSecret,
        RefreshSecret,
        AccessExpire,
        RefreshExpire,
        Secret,
        Enabled
    }
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsSSLEnable { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class JwtOptions
    {
        public string JwtId { get; set; } 
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public DateTime IssuedAt => DateTime.UtcNow; 
        public DateTime NotBefore => DateTime.UtcNow;
        public string SecretKey { get; set; }
        public int TokenExpireInMints { get; set; }  
        public int  RefreshExpireInDays { get; set; } 
        public int JwtClockSkew { get; set; }
        public bool IsEnabled => true;
    }
  
}