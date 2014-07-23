using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.CTP.SDK.Client
{
    public class CTPConfiguration
    {
        private string soapServer = "VM1BOX";
        private string certFile = "SPK_PARTNER_TEST.cer";

        public string SoapServer
        {
            get
            {
                return soapServer;
            }
            set
            {
                this.soapServer = value;
            }
        }

        public string CertFile
        {
            get
            {
                return certFile;
            }
            set
            {
                this.certFile = value;
            }
        }

        public CTPConfiguration(string soap, string certLocation)
        {
            SoapServer = soap;
            CertFile = certLocation;
        }
    }
}
