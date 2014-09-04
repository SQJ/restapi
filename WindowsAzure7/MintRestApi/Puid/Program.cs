using Microsoft.CTP.SDK.Client;
using MS.Support.CMAT.Test.Framework.Utility;
using MS.Support.CMATGateway.Proxy.CredentialService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Configuration;
using System.Xml;

namespace MintRestApi
{
    class GetPUID
    {
        public string getpuid(string email)
        {
            //"dalonghenin@hotmail-int.com"
            CredentialServiceAPISoapServer proxy = new CredentialServiceAPISoapServer();

            proxy.Url = "https://api.login.live-int.com/pksecure/PPSACredentialPK.srf";
            proxy.ClientCertificates.Add(CertificateHelper.GetCertByThumbprint("326f24a35d0422116de00e770298a7a8bd802947"));
            proxy.UseDefaultCredentials = false;
            proxy.Proxy = new WebProxy("http://itgproxy.redmond.corp.microsoft.com");
            proxy.WSSecurityHeader = GenerateSoapHeader();

            StringReader sr = new StringReader(proxy.GetNetIDsForSigninNames(email));
            string res = sr.ReadToEnd();

            return res;
        }


        private const string SoapheaderURL = @"http://schemas.microsoft.com/Passport/SoapServices/SoapHeader";

        private const string SitetokenURL = @"http://schemas.microsoft.com/Passport/SiteToken";

        private const string CredentialAPISoapHeaderFormat = @"<s:ppSoapHeader xmlns:s='http://schemas.microsoft.com/Passport/SoapServices/SoapHeader' version='1.0'><s:clientIP /><s:lcid /><s:authorizationLicence /> <s:auditInfo /><s:delegate />  <s:sitetoken>    <t:siteheader xmlns:t='http://schemas.microsoft.com/Passport/SiteToken' id='' />  </s:sitetoken></s:ppSoapHeader>";


        private static tagWSSECURITYHEADER GenerateSoapHeader()
        {
            tagWSSECURITYHEADER header = new tagWSSECURITYHEADER();
            // Create an XmlDocument and associate the SoapHeader and SiteToken namespaces
            // with it.
            XmlDocument soapHeaderXmlFile = new XmlDocument();
            XmlNamespaceManager snamespace = new XmlNamespaceManager(soapHeaderXmlFile.NameTable);
            snamespace.AddNamespace("s", SoapheaderURL);
            XmlNamespaceManager tnamespace = new XmlNamespaceManager(soapHeaderXmlFile.NameTable);
            tnamespace.AddNamespace("t", SitetokenURL);

            using (XmlReader xmlReader = XmlReader.Create(new StringReader(CredentialAPISoapHeaderFormat)))
            {
                // Load the SOAP header template to construct authentication information.
                soapHeaderXmlFile.Load(xmlReader);
            }
            // Add the specified values to the SOAP header template.
            soapHeaderXmlFile.DocumentElement.SelectSingleNode("s:clientIP", snamespace).InnerText = string.Empty;
            soapHeaderXmlFile.DocumentElement.SelectSingleNode("s:lcid", snamespace).InnerText = "1033";
            soapHeaderXmlFile.DocumentElement.SelectSingleNode("s:sitetoken", snamespace).SelectSingleNode("t:siteheader", tnamespace).Attributes.GetNamedItem("id").InnerText = "289836";

            // Return the SOAP header
            header.ppSoapHeader25 = soapHeaderXmlFile.InnerXml;
            header.version = EnumSHVersion.eshHeader25;
            return header;
        }
        
    }

}
