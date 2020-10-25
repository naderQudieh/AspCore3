using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DeerflyPatches.Modules.PayPal
{
    [DataContract]
    public class AccessToken
    {
        [DataMember(Name = "scope")]
        public string Scope { get; set; }

        [DataMember(Name = "nonce")]
        public string Nonce { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessTokenString { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "app_id")]
        public string AppId { get; set; }

        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }
    }
}
