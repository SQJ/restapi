using System.Diagnostics;
using System.Xml.Serialization;
using System;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Services;
using System.Net;

namespace Microsoft.Sax.SaxPassportLib.SoapProxy
{
	/// <summary>
	/// Summary description for ProxyBase.
	/// </summary>
	[System.Web.Services.WebServiceBindingAttribute()]
	public class ServiceApiSoapProxy: System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		public PPSoapHeader_wrapper PPSoapHeader = new PPSoapHeader_wrapper();
		public tagWSSECURITYHEADER WSSecurityHeader = new tagWSSECURITYHEADER();
		public string LastCaller = string.Empty;
		public string LastHeader = string.Empty;
		public string LastSiteID = string.Empty;
        public string NormalUrlNodeName = string.Empty;
        public string SecureUrlNodeName = string.Empty;

		public ServiceApiSoapProxy()
		{
			
		}

		protected override System.Net.WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(uri);
			request.KeepAlive = false;

			return request;
		}

	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/Passport/SoapServices/ProfileServiceAPI/V1")]
	[System.Xml.Serialization.XmlRootAttribute("WSSecurityHeader", Namespace="http://schemas.microsoft.com/Passport/SoapServices/ProfileServiceAPI/V1", IsNullable=false)]
	public class tagWSSECURITYHEADER : System.Web.Services.Protocols.SoapHeader 
	{
        
		/// <remarks/>
		public EnumSHVersion version;
        
		/// <remarks/>
		public string wssecurity;
        
		/// <remarks/>
		public string authorization;
        
		/// <remarks/>
		public string sitetoken;
        
		/// <remarks/>
		public string ppSoapHeader25;
        
		/// <remarks/>
		public string auditInfo;
        
		/// <remarks/>
		public string @delegate;
        
		/// <remarks/>
		public string originator;
	}
    
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/Passport/SoapServices/ProfileServiceAPI/V1")]
	public enum EnumSHVersion 
	{
        
		/// <remarks/>
		eshHeader30,
        
		/// <remarks/>
		eshHeader25,
        
		/// <remarks/>
		eshNone,
	}
    
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/Passport/SoapServices/ProfileServiceAPI/V1")]
	[System.Xml.Serialization.XmlRootAttribute("PPSoapHeader", Namespace="http://schemas.microsoft.com/Passport/SoapServices/ProfileServiceAPI/V1", IsNullable=false)]
	public class PPSoapHeader_wrapper : System.Web.Services.Protocols.SoapHeader 
	{
        
		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string[] Text;
	}
    
	/// <remarks/>
    
	/// <remarks/>
	    
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/Passport/SoapServices/CredentialServiceAPI/V1")]
	public class tagPASSID 
	{
        
		/// <remarks/>
		public PASSIDTYPE pit;
        
		/// <remarks/>
		public string bstrID;
	}
    
	/// <remarks/>

	[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.microsoft.com/Passport/SoapServices/CredentialServiceAPI/V1")]
	public enum PASSIDTYPE 
	{
        
		/// <remarks/>
		PASSID_PUID_SIGNINNAME,
        
		/// <remarks/>
		PASSID_ROLEID,
        
		/// <remarks/>
		PASSID_PPSACREDENTIALID,
        
		/// <remarks/>
		PASSID_NULL,
        
		/// <remarks/>
		PASSID_PUID,
        
		/// <remarks/>
		PASSID_SIGNINNAME,
	}
    
	/// <remarks/>
}
