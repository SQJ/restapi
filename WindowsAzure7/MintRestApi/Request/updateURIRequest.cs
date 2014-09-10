using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MintRestApi
{
    [DataContract]
    public class updateURIRequest
    {
        [DataMember]
        public string email;

        [DataMember]
        public string uri;

        [DataMember]
        public string token_value;
    }
}