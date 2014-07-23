using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
namespace Microsoft.CTP.SDK.Client
{
    public abstract class ApiClientBase : IDisposable
    {
        private string wsdlUrl;
        private X509Certificate2 cert;

        public ApiClientBase(string _wsdlUrl, X509Certificate2 _cert)
        {
            this.wsdlUrl = _wsdlUrl;
            this.cert = _cert;
        }

        public ApiClientBase(string _wsdlUrl, string certfileName)
        {
            this.wsdlUrl = _wsdlUrl;
            this.cert = FindCertificate(certfileName);
        }

        public string WsdlUrl
        {
            get
            {
                return this.wsdlUrl;
            }
            set
            {
                this.wsdlUrl = value;
            }
        }

        public X509Certificate2 Cert
        {
            get
            {
                return this.cert;
            }
            set
            {
                this.cert = value;
            }
        }

        public void Dispose()
        {
        }
        #region Cert Helper
        public static X509Certificate2 FindCertificate(string certSubject)
        {
            string certStore = "MY";
            if (String.IsNullOrEmpty(certSubject))
            {
                throw new ArgumentException("certSubject cannot be null or empty");
            }
            if (String.IsNullOrEmpty(certStore))
            {
                throw new ArgumentException("certStore cannot be null or empty");
            }

            // Only support localmachine
            X509Store store = new X509Store(certStore, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var x509CertCollection = store.Certificates.Find(X509FindType.FindBySubjectName, certSubject, true);
                if (x509CertCollection.Count > 1)
                {
                    List<X509Certificate2> certList = new List<X509Certificate2>();

                    // X509FindType.FindBySubjectName is an imprecise search, try to match the exact CN field in subject
                    foreach (X509Certificate2 cert in x509CertCollection)
                    {
                        if (MatchExactSubjectForCNPart(cert.Subject, certSubject))
                        {
                            certList.Add(cert);
                        }
                    }
                    if (certList.Count != 0)
                    {
                        return certList.OrderBy(cert => cert.NotAfter).ToArray<X509Certificate2>().FirstOrDefault();
                    }
                }
                else if (x509CertCollection.Count == 1)
                {
                    X509Certificate2 foundCert = x509CertCollection[0];
                    return foundCert;
                }
            }
            finally
            {
                store.Close();
            }

            return null;
        }

        private static Regex CNInSubjectRegex = new Regex(@"CN=(?<CN>([\w.-]+\s*)+)[,$]?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static bool MatchExactSubjectForCNPart(string subjectName, string expectedCN)
        {
            Match m = CNInSubjectRegex.Match(subjectName);
            if (!m.Success)
            {
                return false;
            }
            string actualCN = m.Groups["CN"].Value;
            return actualCN.Equals(expectedCN, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
        public static string DataContractSerialize(object obj)
        {
            if (null == obj) return "NULL";
            DataContractSerializer dcs = new DataContractSerializer(obj.GetType());
            using (StringWriter sw = new StringWriter())
            using (XmlTextWriter xw = new XmlTextWriter(sw))
            {
                dcs.WriteObject(xw, obj);
                return sw.ToString();
            }
        }
        public static XmlDocument GenerateOutputXml(string methonName, object result, object[] inputValues, Exception exception)
        {
            XmlDocument outputXmlDocument = new XmlDocument();
            XmlDocument tempXmlDocument = new XmlDocument();
            if (exception != null)
            {
                XmlOutput xmlOutput = new XmlOutput(outputXmlDocument, null, null);
                xmlOutput.AddException(exception);
                return outputXmlDocument;
            }
            else
            {
                if (exception == null)
                {
                    outputXmlDocument = new XmlDocument();
                    XmlElement root = outputXmlDocument.CreateElement(methonName);
                    outputXmlDocument.AppendChild(root);
                    string xmlString = null;
                    xmlString = DataContractSerialize(result);
                    tempXmlDocument.LoadXml(xmlString);
                    XmlElement outputElement = outputXmlDocument.CreateElement("Output");
                    outputElement.InnerXml = tempXmlDocument.OuterXml;
                    root.AppendChild(outputElement);
                    if (inputValues != null)
                    {
                        for (int i = 0; i < inputValues.Length - 1; i++)
                        {
                            xmlString = DataContractSerialize(inputValues[i]);
                            if (xmlString != null)
                            {
                                tempXmlDocument.LoadXml(xmlString);
                                XmlElement inputElement = outputXmlDocument.CreateElement(inputValues[i].GetType().Name);
                                inputElement.InnerXml = tempXmlDocument.OuterXml;
                                root.AppendChild(inputElement);
                            }
                        }
                    }

                }
                if (inputValues != null)
                {
                    XmlOutput xmlOutput = new XmlOutput(outputXmlDocument, null, null);
                    xmlOutput.AddParameterNode("CallDuration", inputValues[inputValues.Length - 1].ToString());
                }
            }
            return outputXmlDocument;
        }
    }

    public sealed class CommerceApiClient : ApiClientBase
    {
        #region Constructors/Destructors
        public CommerceApiClient(string wsdlUrl, Type clientType, string contractName, X509Certificate2 cert)
            : base(wsdlUrl, cert)
        {
            client = clientType;
            contract = contractName;
            InitializeBinding();
        }
        public CommerceApiClient(string wsdlUrl, Type clientType, string contractName, string certFile)
            : base(wsdlUrl, certFile)
        {
            this.client = clientType;
            contract = contractName;
            InitializeBinding();
        }
        #endregion

        #region variants
        private BasicHttpBinding binding = null;
        private Type client;
        private string contract;
        #endregion

        #region Property
        public Type ClientType
        {
            get
            {
                return client;
            }
            set
            {
            }
        }
        public string ContractName
        {
            get
            {
                return this.contract;
            }
            set
            {
                this.contract = value;
            }
        }
        #endregion

        #region Help Method
        private void InitializeBinding()
        {
            binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(3);
            binding.SendTimeout = TimeSpan.FromMinutes(3);
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferPoolSize = 524288;
            binding.MaxReceivedMessageSize = 65536;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = Encoding.UTF8;
            binding.UseDefaultWebProxy = true;
            binding.AllowCookies = false;

            var readerQuotas = binding.ReaderQuotas;
            readerQuotas.MaxDepth = 32;
            readerQuotas.MaxStringContentLength = 16384;
            readerQuotas.MaxArrayLength = 16384;
            readerQuotas.MaxBytesPerRead = 4096;
            readerQuotas.MaxNameTableCharCount = 16384;

            var transport = binding.Security.Transport;
            transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            transport.ProxyCredentialType = HttpProxyCredentialType.None;
            transport.Realm = "";
        }

        private object CreateClient()
        {
            EndpointAddress endpointAddress = new EndpointAddress(WsdlUrl);
            Trace.TraceInformation("Endpoint Address {0}", endpointAddress);
            object serviceClient = Activator.CreateInstance(ClientType, new object[2] { binding, endpointAddress });
            ServiceEndpoint endpoint = (ServiceEndpoint)serviceClient.GetType().GetProperty("Endpoint").GetValue(serviceClient, null);
            endpoint.Behaviors.RemoveAll<ClientCredentials>();
            var cert = FindCert(X509FindType.FindByThumbprint, ((X509Certificate2)Cert).Thumbprint);
            Trace.TraceInformation("Cert {0}", cert.ClientCertificate.Certificate.Subject.ToString());
            endpoint.Behaviors.Add(cert);
            endpoint.Contract.Name = ContractName;
            return serviceClient;
        }

        private ClientCredentials FindCert(X509FindType type, string value)
        {
            var clientCredentials = new ClientCredentials();
            clientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine,
                StoreName.My, type, value);

            var serverCertAuth = clientCredentials.ServiceCertificate.Authentication;
            serverCertAuth.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            serverCertAuth.RevocationMode = X509RevocationMode.NoCheck;
            if (null == clientCredentials.ClientCertificate.Certificate)
            {
                string msg = string.Format("Cannot find certificate [{0}] by [{1}].", value, type.ToString());
                throw new ApplicationException(msg);
            }
            return clientCredentials;
        }
        #endregion

        public T InvokeMethod<T>(string methodName, object[] paramValues)
        {
            object output = null;
            MethodInfo methodToCall = null;
            if (null != ClientType)
            {
                methodToCall = ClientType.GetMethod(methodName);
                if (null == methodToCall)
                {
                    throw new Exception(methodName + " not found!");
                }
                else
                {
                    object wcfClient = CreateClient();
                    output = methodToCall.Invoke(wcfClient, paramValues);
                }
            }
            return (T)output;
        }

        public XmlDocument InvokeMethodAndGenerateOutput<T>(string methodName, object[] paramValues)
        {
            DateTime StartTime = DateTime.Now;
            MethodInfo methodToCall = ClientType.GetMethod(methodName);
            try
            {
                object result = InvokeMethod<T>(methodName, paramValues);
                DateTime EndTime = DateTime.Now;
                object[] newParamValues = new object[paramValues.Length + 1];
                paramValues.CopyTo(newParamValues, 0);
                newParamValues.SetValue(EndTime.Subtract(StartTime), paramValues.Length);
                return GenerateOutputXml(methodToCall.Name, result, newParamValues, null);
            }
            catch (Exception e)
            {
                return GenerateOutputXml(methodToCall.Name, null, paramValues, e);
            }
        }
    }

    public sealed class ScsApiClient : ApiClientBase
    {
        #region Constructors/Destructors
        public ScsApiClient(string wsdlUrl, X509Certificate2 cert)
            : base(wsdlUrl, cert)
        {
        }
        public ScsApiClient(string wsdlUrl, string certFile)
            : base(wsdlUrl, certFile)
        {
        }
        #endregion
        public object InvokeMethod(string methodName, object[] paramValues)
        {
            object output = null;
            MethodInfo methodToCall = null;
            bdk client = new bdk
            {
                Url = this.WsdlUrl
            };
            client.ClientCertificates.Add(Cert);
            methodToCall = typeof(bdk).GetMethod(methodName);
            if (null == methodToCall)
            {
                throw new Exception(methodName + " not found!");
            }
            else
            {
                output = methodToCall.Invoke(client, paramValues);
            }
            return output;
        }
        
    }

}
