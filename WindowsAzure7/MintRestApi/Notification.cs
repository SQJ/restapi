using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MintRestApi
{
    public class Notification
    {
        public string email;
        public string uri;
        public string content1;
        public string content2;
        public Notification(string email, string uri, string content1, string content2)
        {
            this.email = email;
            this.uri = uri;
            this.content1 = content1;
            this.content2 = content2;
        }
    }
}