using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MintRestApi
{
    [DataContract]
    public class PurchaseWithBTCRequest
    {
        [DataMember]
        public string email;

        [DataMember]
        public string type;

        [DataMember]
        public string id;

        [DataMember]
        public string token_value;

        [DataMember]
        public string token;

    }
}