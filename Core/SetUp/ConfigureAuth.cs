using AppZeroAPI.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppZeroAPI.Setup
{
    public static partial class SetUp
    {

        public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("JwtOptions");
            JwtOptions jwtOptions = appSettingsSection.Get<JwtOptions>();
            var key = Encoding.ASCII.GetBytes(jwtOptions.SecretKey);
            var IssuerSigningKey = new SymmetricSecurityKey(key);
            bool isEnabled = jwtOptions.IsEnabled;
            if (isEnabled)
            {
                services.Configure<JwtOptions>(options =>
                {
                    if (jwtOptions.TokenExpireInMints > 0)
                    {
                        options.TokenExpireInMints = jwtOptions.TokenExpireInMints;
                    }

                    if (jwtOptions.RefreshExpireInDays > 0)
                    {
                        options.RefreshExpireInDays = jwtOptions.RefreshExpireInDays;
                    }
                    options.Issuer = jwtOptions.Issuer;
                    options.Audience = jwtOptions.Audience;
                    options.SecretKey = jwtOptions.SecretKey;
                });

                services.AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                  .AddJwtBearer(configureOptions =>
                  {
                      configureOptions.Validate();
                      configureOptions.RequireHttpsMetadata = false;
                      configureOptions.SaveToken = true;
                      configureOptions.ClaimsIssuer = jwtOptions.Issuer;
                      configureOptions.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = false,
                          ValidateAudience = false,
                          ValidateIssuerSigningKey = true,
                          ValidIssuer = jwtOptions.Issuer,
                          ValidAudience = jwtOptions.Audience,
                          IssuerSigningKey = IssuerSigningKey,
                          RequireExpirationTime = true,
                          ValidateLifetime = true,
                          ClockSkew = TimeSpan.Zero
                      };
                      configureOptions.Validate();
                      configureOptions.Events = new JwtBearerEvents
                      {
                          OnMessageReceived = context =>
                          {
                              var authorization = context.Request.Headers["Authorization"];
                              var accessToken = authorization.FirstOrDefault();
                              if (accessToken != null)
                              {
                                  if (accessToken.IndexOf("Bearer") > -1)
                                  {
                                      accessToken = accessToken.Substring("Bearer ".Length).Trim();
                                  }

                                  context.Request.Headers.Remove("Authorization");
                                  context.Request.Headers.Add("Authorization", $"Bearer {accessToken}");


                                  var identity = context.HttpContext.User.Identity as ClaimsIdentity;
                                  var handler = new JwtSecurityTokenHandler();
                                  JwtSecurityToken obj = handler.ReadJwtToken(accessToken);
                                  DateTime validTo = obj.ValidTo;
                                  DateTime validfrom = obj.ValidFrom;
                                  var durMin = (validTo - validfrom).TotalMinutes;
                                  var durHour = (validTo - validfrom).TotalHours;
                              }

                              // var accessToken = context.HttpContext.Request.Headers["Authorization"];
                              //// var accessToken = context.Request.Query["access_token"];

                              // var path = context.HttpContext.Request.Path;
                              // if (!string.IsNullOrEmpty(accessToken) 
                              // //&& path.StartsWithSegments("/signalr")
                              // )
                              // {
                              //     context.Token = accessToken;
                              // }

                              return Task.CompletedTask;
                          },
                          OnForbidden = ctx =>
                          {
                              Console.WriteLine(ctx.Response.StatusCode);
                              //throw new AppException("Forbidden"); 
                              return Task.CompletedTask;
                          },
                          OnAuthenticationFailed = context =>
                         {

                             string err = "";

                             var exType = context.Exception.GetType();

                             if (exType == typeof(SecurityTokenValidationException))
                             {
                                 context.Response.Headers.Add("Token-Invalid", "true");
                                 err += "invalid token";
                             }
                             else if (exType == typeof(SecurityTokenInvalidIssuerException))
                             {
                                 context.Response.Headers.Add("Token-Invalid-Issuer", "true");
                                 err += "invalid issuer";
                             }
                             else if (exType == typeof(SecurityTokenExpiredException))
                             {
                                 context.Response.Headers.Add("Token-Expired", "true");
                                 err += "token expired";
                             }
                             else if (exType == typeof(SecurityTokenKeyWrapException))
                             {
                                 context.Response.StatusCode = 200;
                                 err += "token key fail";
                             }
                             else if (exType == typeof(SecurityTokenInvalidSignatureException))
                             {
                                 context.Response.Headers.Add("Token-Invalid-Signature", "true");
                                 err += "token key invalid signature";
                             }      //  context.NoResult();
                                    //  context.Response.ContentType = "application/json";
                                    //  context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                    // var resp = AppResponse.UnauthorizedUser("Authorization Rejection -" + $"{ err }");
                                    //await context.Response.WriteAsync(JsonConvert.SerializeObject(resp, Formatting.Indented))  ;


                              return Task.CompletedTask;

                         },
                          OnTokenValidated = context =>
                          {
                              var accessToken = context.SecurityToken as JwtSecurityToken;
                              if (accessToken != null)
                              {
                                  var identity = context.HttpContext.User.Identity as ClaimsIdentity;
                                  if (identity != null)
                                  {
                                      identity.AddClaim(new Claim("access_token", accessToken.RawData));
                                  }

                              }
                              var userIdentity = context.Principal.Identity.Name;
                              //var exp = AppZeroAPI.Shared.CommonLib.GetExpiryClaimExpiryDate(context.Principal.Claims.Where(x => x.Type == ExpiryClaimDefinition).FirstOrDefault().Value);
                              //var connectionString = Configuration.GetConnectionString(ThermoDatabaseContext);
                              var isUserValid = true; //IsUserAuthorized(userIdentity, connectionString, exp);

                              if (!isUserValid)
                                  context.Fail("Unauthorized");

                              return Task.CompletedTask;
                          }
                      };

                  });
            };
        }

    }
}
