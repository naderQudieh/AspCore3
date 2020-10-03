using AppZeroAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Linq;
using System.Globalization;
using System.Security.Cryptography;
using AppZeroAPI.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AppZeroAPI.Shared
{ 
   
    class JsonUnformatterBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new JsonUnformatterBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
        }
    }
    class JsonUnformatterBinder : IModelBinder
    {
        private readonly IModelBinder _fallbackBinder;

        public JsonUnformatterBinder(IModelBinder fallbackBinder)
        {
            _fallbackBinder = fallbackBinder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelname = bindingContext.ModelType.Name;
            if (bindingContext.ActionContext.HttpContext.Request.Path.HasValue)
            {
                string currMethod = bindingContext.ActionContext.HttpContext.Request.Method;

                if ("POST".Equals(currMethod) || "PUT".Equals(currMethod))
                {
                    dynamic model = null;
                    string bodyAsText = new StreamReader(bindingContext.HttpContext.Request.Body).ReadToEndAsync().Result;

                    if (modelname == "RegisterDto")
                    {
                        model = JsonConvert.DeserializeObject<RegisterDto>(bodyAsText); 
                    }
                    bindingContext.Result = ModelBindingResult.Success(model);
                } 
            }
            return _fallbackBinder.BindModelAsync(bindingContext);
        }
    }
    public static class Helper
    {
        private static readonly byte[] _ivBytes = { 180, 230, 246, 6, 138, 15, 193, 244, 76, 122, 254, 225, 236, 198, 17, 189 };

        private static readonly byte[] _keyBytes =
           {
            243, 211, 148, 241, 45, 56, 204, 72, 98, 129, 85, 9, 121, 233, 195, 236,
            202, 178, 210, 224, 188, 126, 134, 131, 67, 230, 232, 72, 42, 239, 191, 84
        };
        public static string Encrypt(string stringToEncrypt)
        {
            byte[] encryptedToken;
            using (var aes = Aes.Create())
            {
                aes.Key = _keyBytes;
                aes.IV = _ivBytes;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var mStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(stringToEncrypt);
                        }
                        encryptedToken = mStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encryptedToken);
        }

        public static string Decrypt(string encryptedString)
        {
            try
            {
                var tokenBytes = Convert.FromBase64String(encryptedString);
                string decryptedToken;
                using (var aes = Aes.Create())
                {
                    aes.Key = _keyBytes;
                    aes.IV = _ivBytes;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var mStream = new MemoryStream(tokenBytes))
                    {
                        using (var decryptoStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (var streamReader = new StreamReader(decryptoStream))
                            {
                                decryptedToken = streamReader.ReadToEnd();
                            }
                        }
                    }
                }

                return decryptedToken;
            }
            catch (Exception)
            {
                throw new Exception("Invalid Token");
            }
        }

        public static string getIPAddress(HttpRequest Request)
        {
            string ipaddress = "";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                ipaddress = Request.Headers["X-Forwarded-For"];
            else
            {
                if (string.IsNullOrEmpty(ipaddress))
                    ipaddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            return ipaddress;
        }
        public static string HashPassword(string password, string salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var bytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            return Convert.ToBase64String(bytes);
        }

       

        public static string GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        private static byte[] GenerateSaltBytes()
        {
            var salt = new byte[32];
            using (var rand = new RNGCryptoServiceProvider())
            {
                rand.GetNonZeroBytes(salt);
            }
            return salt;
        }

        public static (string hash, string salt) GetPasswordHash(string plainPassword)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
            var saltBytes = GenerateSaltBytes();
            var password = passwordBytes.Concat(saltBytes).ToArray();
            using (SHA256 sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(password);
                return (Convert.ToBase64String(hash), Convert.ToBase64String(saltBytes));
            }
        }

        public static string GetPasswordHash(string plainPassword, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
            var saltBytes = Convert.FromBase64String(salt);
            var password = passwordBytes.Concat(saltBytes).ToArray();
            using (SHA256 sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(password);
                return Convert.ToBase64String(hash);
            }
        }
        public static List<string> GetErrorListFromModelState(ModelStateDictionary modelState)
        {
            //var errors = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0).SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToArray();
            // var errorMsg = string.Join(" | ", actionContext.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            // var apiResponse = AppResponse.BadRequest(actionContext.ModelState);
            //IEnumerable<ModelError> allErrors = actionContext.ModelState.Values.SelectMany(v => v.Errors);
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            var errorList = query.ToList();
            return errorList;
        }
        
         
        
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
       
        public static long ToTimeStamp(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = dateTime.Subtract(new TimeSpan(epoch.Ticks));

            return time.Ticks / 10000;
        }
        public static DateTime GetExpiryClaimExpiryDate(string date)
        {
            if (double.TryParse(date, out double linuxTime))
            {
                return UnixTimeStampToDateTime(linuxTime);
            }
            return DateTime.MinValue;
        }
       
    }


    
}
