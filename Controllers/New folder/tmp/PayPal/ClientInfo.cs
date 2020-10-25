using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DeerflyPatches.Modules.PayPal
{
    [DataContract]
    public class ClientInfo
    {
        [DataMember(Name = "client_account")]
        public string ClientAccount { get; set; }

        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; set; }
    }
}
