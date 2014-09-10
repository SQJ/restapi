﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MintRestApi
{
    [DataContract]
    public class FundWithCCRequest
    {
        [DataMember]
        public string email;

        [DataMember]
        public string paymentMethodID;

        [DataMember]
        public string amount;

        [DataMember]
        public string token_value;
    }
}