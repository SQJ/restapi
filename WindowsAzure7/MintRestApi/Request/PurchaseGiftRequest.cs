using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MintRestApi
{
    [DataContract]
    public class PurchaseGiftRequest
    {
        [DataMember]
        public string sender_email;

        [DataMember]
        public string sender_paymentMethodID;

        [DataMember]
        public string receiver_email;

        [DataMember]
        public string amount;

        [DataMember]
        public string token_value;
    }
}