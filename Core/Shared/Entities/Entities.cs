using AppZeroAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AppZeroAPI.Entities
{
    



    [Table("user_refresh_tokens")]
    public class UserTokenData
    {
        [Column("token_id")]
        public int TokenId { get; set; }

        [Column("user_id")]
        public int user_id { get; set; }

  
       
        [Column("access_token")]
        public string AccessToken { get; set; }

        [Column("refresh_token")]
        public string RefreshToken { get; set; }
      
        [Column("black_listed")]
        public bool BlackListed { get; set; }
       
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }
       
        [Column("created_by_ip")]
        public string CreatedByIP { get; set; }
        
        [NotMapped]
        public string ReplacedByToken { get; set; }
        [NotMapped]
        public bool IsActive => !BlackListed && !IsExpired;
        
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public UserTokenData( )
        {
             
        }
        public UserTokenData(string token, DateTime expires)
        {
            this.AccessToken = token;
            this.ExpiresAt  = expires;
        }
        public UserTokenData(string token, string refreshToken, DateTime expires)
        {
            this.AccessToken = token;
            this.RefreshToken = refreshToken;
            this.ExpiresAt = expires;
        }
    }

    

  
  
    public class UserTokensData
    {
      
        public string userId { get; set; } 
        public string email { get; set; }
        
        IList<UserToken> tokens { get; set; }
    }

    [Table("user_profiles")]
    public class UserProfile
    { 
        [Dapper.Contrib.Extensions.Key]
        [Key]
        [Required]
        [Column("user_id")]
        [JsonProperty("user_id")]
        public int user_id { get; set; }

        [Column("username")]
        [JsonProperty("username")]
        public string username { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        [JsonIgnore] 
        public string password { get; set; }
        [JsonIgnore]
        public string password_hash { get; set; }

        [JsonIgnore]
        public string password_salt { get; set; }
        public string role { get; set; }


        [NotMapped]
        public string verification_token { get; set; }
        [NotMapped]
        public DateTime? verified { get; set; }

        [NotMapped]
        public bool Isverified => verified.HasValue || password_reset.HasValue;
         
     
        [JsonIgnore]
        public DateTime? password_reset { get; set; }

        [Column("created_on")]
        public DateTime  created_on { get; set; }

        [Column("last_modified")]
        public DateTime last_modified { get; set; }
         
        [Column("language")]
        public int language { get; set; }
       
        [Column("profile_picture")]
        public string profile_picture { get; set; }

    }

    [Table("user_refresh_tokens")]
    public class UserToken
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("id_token")] 
        public int TokenId { get; set; }
       
        [Required]
        [Column("access_token")]
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Required]
        [Column("refresh_token")]
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public bool BlackListed { get; set; }
        [JsonProperty("created_at")]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("expires_at")]
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("is_active")]
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }
    }


    [Table("products")]
    public class Product
    {
        [Dapper.Contrib.Extensions.Key]
        [Column("Id")]
        [JsonProperty("Id")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public decimal Rate { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
    public class Detect
    {
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }

        public string DeviceType { get; set; }

        public string Os { get; set; }

        public int? UserId { get; set; }

        public string Browser { get; set; }

        public string UserIp { get; set; }
 
    }
}
