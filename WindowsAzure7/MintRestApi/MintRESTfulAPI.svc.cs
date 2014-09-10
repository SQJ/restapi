using Microsoft.Commerce.Proxy.AccountService;
using PIType = Microsoft.Commerce.Proxy.PaymentInstrumentService;
using CTType = Microsoft.Commerce.Proxy.TransactionService.v201001;
using Microsoft.CTP.SDK.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Collections;

using System.Data.SqlClient;
using System.Data;

using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;


using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System.Globalization;
using CMATType = MS.Support.CMATGateway.Proxy.SCS;
using System.Xml.Serialization;
using System.ServiceModel.Web;
using System.Xml;
using MintRestApi.BTCC;

namespace MintRestApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MintRESTfulAPI" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MintRESTfulAPI.svc or MintRESTfulAPI.svc.cs at the Solution Explorer and start debugging.

    public class MintRESTfulAPI : IMintRESTfulAPI
    {
        private static string soapServer = "sps.msn-int.com";
        //private static string soapServer = "dihong1box";

        private static string certFile = "spk-partner-azurecsr-int";
        //private static string certFile = "SPK_PARTNER_TEST";

        private static string defaultIdentityValue = "367284128424247";

        private const int RetryTime = 2;

        private const string liveglobalUrl = "https://apis.live.net/v5.0/me";
        private const string apiglobalUrl = "http://mintrestapi2.cloudapp.net/MintRESTfulAPI.svc/";

        private const string accessKey = "6351c6e2-f6d0-4403-a2ab-f1fee2fc782e";
        private const string secretKey = "bb7cf19c-29be-4a52-bebe-8ed1e3caad77";

        private const string skuReferenceXmlFormat = @"<bdk:SkuReference>{0}</bdk:SkuReference><bdk:SkuReferenceType>{1}</bdk:SkuReferenceType>";
        private const string skuReferenceInfoXmlFormat = @"<bdk:SkuReferenceInfo xmlns:bdk='urn:schemas-microsoft-com:billing-data'>{0}</bdk:SkuReferenceInfo>";
        private const string bstrPropertyXmlForMandateRefund = "<bdk:PropertyWithNamespace xmlns:bdk=\"urn:schemas-microsoft-com:billing-data\"><bdk:Namespace>StoredValue</bdk:Namespace><bdk:Name>EnableMandateRefund</bdk:Name><bdk:Value>True</bdk:Value></bdk:PropertyWithNamespace>";
        public const int AddArbitraryCreditOrChargeCommentCode = 462849;
        private static readonly string nameSpace = "urn:schemas-microsoft-com:billing-data";

        private Random RNG = new Random();
        private static URLMessage urlcheck = new URLMessage();

        private static CTPConfiguration config = new CTPConfiguration(soapServer, certFile);

        public enum QueryStatus { Success, NoUser, WrongDevice, NoHeader };

        private static CacheData cachedata = new CacheData();
        

        private static string userName = "t-qishen";
        private static string password = "#Bugfor$";
        private static string dataSource = "ikolypwpyi.database.windows.net,1433";
        private static string sampleDatabaseName = "mintapp_db";
        private static SqlConnectionStringBuilder connString2Builder = new SqlConnectionStringBuilder
        {
            DataSource = dataSource,
            InitialCatalog = sampleDatabaseName,
            Encrypt = true,
            TrustServerCertificate = false,
            UserID = userName,
            Password = password,
        };

        private int getEpochTime()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);
            long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;
            return (int)a;
        }

        private string translateEpochTime(int timestamp)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            var res = epoch.AddSeconds(timestamp);
            return res.ToString();
        }

        #region Security 
        private String requestLive(string accesstoken)
        {
            WebClient webClient = new WebClient();
            string curtime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:sszzz");
            string url = liveglobalUrl + "?access_token=" + accesstoken + "&time=" + curtime;
            return webClient.DownloadString(url);
        }

        public bool verifyLive(string accesstoken, string email)
        {
            return true;
            try
            {
                string rawJson = requestLive(accesstoken);
                JObject json = JObject.Parse(rawJson);
                if (email == json["emails"]["account"].ToString())
                {
                    return true;
                }
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        public bool verifyURL()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;        

            string guid = headers["GUID"];

            if (cachedata.getguidAuth(guid) == null)
            {
                cachedata.addguidAuth(guid);
                return true;
            }
            else return false;
      
        }

        public QueryStatus verifyCookie(string email, ref string deviceID)
        {
            // RETURN status: 
            // Success: verity success, need update;
            // NoUser: verity fail, No this email account, need create if token verified;
            // WrongDevice: verity fail, Wrong device id, need update if token verified
            // Noheader: error, No device header
            try
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;

                string query_id = null;
                if (headers["UDID"] != null)
                {
                    deviceID = headers["UDID"];

                    //query in cache, If email not exist or not fit with the deviceID, return false;   
                    // only if the email fit the deviceID a, return true; Notice! return true .                  
                    DeviceID_Query(email, ref query_id);

                    if (query_id == null) return QueryStatus.NoUser;
                    else if (query_id != deviceID )
                        return QueryStatus.WrongDevice;
                    else return QueryStatus.Success;
               }

                return QueryStatus.NoHeader;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return QueryStatus.NoHeader;
        }

        public bool veritySecurity(string accesstoken, string email)
        {
            return true;
            try
            {
                string deviceID = null;
                bool urlflag = false;

                QueryStatus status = verifyCookie(email, ref deviceID);

                if (status == QueryStatus.Success)
                {

                    DeviceID_Update(deviceID, email);
                    urlflag = verifyURL();
                    return urlflag;
                }
                else
                {
                    bool flag = verifyLive(accesstoken, email);
                    if (!flag) return false;
                    else
                    {
                        if (status == QueryStatus.NoDevice) DeviceID_Insert(deviceID, email);
                        else if (status == QueryStatus.WrongInfo) DeviceID_Update(deviceID, email);
                    }
                    urlflag = verifyURL();
                    return urlflag;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return false;

        }

        #endregion

        #region Account Builder
        private Identity BuildIdentityFromAccountWithPuid(string puid)
        {
            return new Identity
            {
                IdentityType = "PUID",
                IdentityValue = puid,
                IdentityProperty = new Property[]
                {
                    new Property
                    {
                        Name = "PassportMemberName",
                        Namespace = "Namespace",
                        Value = "qijun@microsoft.com"
                    }
                }
            };
        }

        private PIType.Identity BuildPIIdentityFromAccountWithPuid(string puid)
        {
            return new PIType.Identity
            {
                IdentityType = "PUID",
                IdentityValue = puid,
                IdentityProperty = new PIType.Property[]
                {
                    new PIType.Property
                    {
                        Name = "PassportMemberName",
                        Namespace = "Namespace",
                        Value = "qijun@microsoft.com"
                    }
                }
            };
        }

        private CTType.Identity BuildCTIdentityFromAccountWithPuid(string puid)
        {
            return new CTType.Identity
            {
                IdentityType = "PUID",
                IdentityValue = puid,
                //IdentityValue = "367284128424247",
                IdentityProperty = new CTType.Property[]
                {
                    new CTType.Property
                    {
                        Name = "PassportMemberName",
                        Namespace = "Namespace",
                        Value = "Andy7697@microsoft.com"
                    }
                }
            };
        }
        #endregion

        #region Account Service
        public int getHighId(string PUID)
        {
            long temp = Int64.Parse(PUID);
            int res = (int)(temp >> 32);
            return res;
        }

        public int getLowId(string PUID)
        {
            long temp = Int64.Parse(PUID);
            long high = (temp >> 32) << 32;
            int res = (int)(temp - high);
            return res;
        }

        public AccountInfo GetAccount(string email, string token_value)
        {
            //var version = WebOperationContext.Current.IncomingRequest.Headers["User-Agent"];
            //return version;
            try
            {
                AccountInfo accinfo = new AccountInfo();
                
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {

                    accinfo.securitystatus = "Untrusted Client Request";
                    return accinfo;
                }
                var puid = EmailToPuid(email);
                var request = new GetAccountInput
                {
                    CallerInfo = new CallerInfo
                    {
                        Requester = BuildIdentityFromAccountWithPuid(puid),
                        Delegator = new Identity
                        {
                            IdentityType = "PUID",
                            IdentityValue = puid
                        }
                    },
                    SearchCriteria = new AccountSearchCriteria
                    {
                        AccountId = null,
                        Identity = BuildIdentityFromAccountWithPuid(puid)
                    },
                    APIContext = new APIContext
                    {
                        TrackingGuid = Guid.NewGuid(),
                    },
                    Filters = new List<Property>
                    {
                        new Property
                        {
                            Name = "Role",
                            Value = "All"
                        }
                    }.ToArray()
                };
                CTPApiClient client = new CTPApiClient(config);

                int attempt = 0;
                var res = client.GetAccount(request);
                while (!(res.Ack == AckCodeType.Success) && (attempt <= RetryTime))
                {
                    attempt += 1;
                    res = client.GetAccount(request);
                }

                if (res.Ack == AckCodeType.Success)
                {
                    accinfo.account = res.AccountOutputInfo.FirstOrDefault().AccountID;
                    accinfo.puid = puid;
                    accinfo.securitystatus = "Secure";
                    return accinfo;
                }
                else
                {
                    accinfo.securitystatus = res.Error.ErrorShortMessage;
                    return accinfo;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string GetAddPIUrl(string email, string token_value)
        {
            try{
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                string puid  =  EmailToPuid(email);
                string account = GetAccountWithPuid(puid);

                string url = string.Format("https://buy.live-int.com/pcsv2/default/liverps/pcs/managepi?accountid={0}&piid=0&mkt=en-us&ctprpsauth=1&wa=wsignin1.0",  account);

                return url;

            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
                
        }


        public string GetAccountWithPuid(string puid)
        {
            try
            {
                var request = new GetAccountInput
                {
                    CallerInfo = new CallerInfo
                    {
                        Requester = BuildIdentityFromAccountWithPuid(puid),
                        Delegator = new Identity
                        {
                            IdentityType = "PUID",
                            IdentityValue = puid
                        }
                    },
                    SearchCriteria = new AccountSearchCriteria
                    {
                        AccountId = null,
                        Identity = BuildIdentityFromAccountWithPuid(puid)
                    },
                    APIContext = new APIContext
                    {
                        TrackingGuid = Guid.NewGuid(),
                    },
                    Filters = new List<Property>
                    {
                        new Property
                        {
                            Name = "Role",
                            Value = "All"
                        }
                    }.ToArray()
                };
                CTPApiClient client = new CTPApiClient(config);

                int attempt = 0;
                var res = client.GetAccount(request);
                while (!(res.Ack == AckCodeType.Success) && (attempt <= RetryTime))
                {
                    attempt += 1;
                    res = client.GetAccount(request);
                }

                if (res.Ack == AckCodeType.Success)
                {
                    return res.AccountOutputInfo.FirstOrDefault().AccountID;
                }
                else
                {
                    return res.Error.ErrorShortMessage;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion

        #region DB operation
        public string urlconfig(string version)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "version_to_url";
                        command.Parameters.AddWithValue("@version", version);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return reader["url"].ToString().Trim();
                            }
                        }
                        conn.Close();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void DeviceID_Query( string email ,ref string query_id)
        {
            try
            {
                 query_id = (cachedata.getDeviceAuth(email)).ToString();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void DeviceID_Insert(string deviceID, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();

                        command.CommandText = "insert_deviceID";
                        command.Parameters.AddWithValue("@id", deviceID);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("ExpireDate", DateTime.UtcNow.AddMinutes(30));
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void DeviceID_Update(string deviceID, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();

                        command.CommandText = "update_deviceID";
                        command.Parameters.AddWithValue("@id", deviceID);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("ExpireDate", DateTime.UtcNow.AddMinutes(30));
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void insertNotification(string email, string content1, string content2)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "insert_Notification";
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@content1", content1);
                        command.Parameters.AddWithValue("@content2", content2);
                        command.CommandType = CommandType.StoredProcedure;
                        int rowsAdded = command.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public Notification[] getNotification(string email)
        {
            try
            {
                ArrayList list = new ArrayList();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_Notification";
                        command.Parameters.AddWithValue("@email", email);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Loop over the results
                            while (reader.Read())
                            {
                                Notification noti = new Notification(reader["email"].ToString().Trim(), reader["uri"].ToString().Trim(), reader["content1"].ToString().Trim(), reader["content2"].ToString().Trim());
                                list.Add(noti);
                            }
                        }
                        conn.Close();
                    }
                }
                Notification[] resArray = (Notification[])list.ToArray(typeof(Notification));
                return resArray;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void deleteNotification(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "delete_Notification";
                        command.Parameters.AddWithValue("@email", email);
                        command.CommandType = CommandType.StoredProcedure;
                        int rowsAdded = command.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string EmailToPuid(string email)
        {
            try
            {
                if (email.Contains("-int.com"))
                {
                    string puid = null;
                    GetPUID getP = new GetPUID();
                    string idString = getP.getpuid(email);
                    
                    using (XmlReader reader = XmlReader.Create(new StringReader(idString)))
                    {
                        reader.ReadToFollowing("SigninName");
                        reader.MoveToFirstAttribute();
                        puid = reader.Value;
                        puid = Convert.ToString(Convert.ToInt64(puid, 16),10);
                        

                    }

                    return puid;

                }
                int count;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "email_to_puid";
                        command.Parameters.AddWithValue("@email", email);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Loop over the results
                            count = 0;
                            while (reader.Read())
                            {
                                count = count + 1;
                                return reader["puid"].ToString().Trim();
                            }
                        }
                        if (count == 0)
                        {
                            string puid = null;
                            command.CommandText = "assign_new_puid";
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                count = 0;
                                while (reader.Read())
                                {
                                    count = count + 1;
                                    return reader["puid"].ToString().Trim();
                                }
                            }
                            return puid;
                        }

                    }
                }
                return defaultIdentityValue;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        
        public string PuidToEmail(string puid)
        {
            try
            {
                int count = 0;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "puid_to_email";
                        command.Parameters.AddWithValue("@puid", puid);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Loop over the results
                            count = 0;
                            while (reader.Read())
                            {
                                count = count + 1;
                                return reader["email"].ToString().Trim();
                            }
                        }
                        conn.Close();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string EmailToURI(string email)
        {
            try
            {
                int count = 0;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_URI";
                        command.Parameters.AddWithValue("@email", email);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Loop over the results
                            count = 0;
                            while (reader.Read())
                            {
                                count = count + 1;
                                return reader["uri"].ToString().Trim();
                            }
                        }
                        conn.Close();
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void updateURI(string email, string uri)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "update_URI";
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@uri", uri);
                        command.CommandType = CommandType.StoredProcedure;
                        int rowsAdded = command.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        //public void insertPuchaseCSV(string email, decimal amount)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_PurchaseCSV";
        //                command.Parameters.AddWithValue("@email", email);
        //                command.Parameters.Add("@amount", SqlDbType.Decimal);
        //                command.Parameters["@amount"].Value = amount;
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.CommandType = CommandType.StoredProcedure;
        //                int rowsAdded = command.ExecuteNonQuery();
        //                conn.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public void insertPuchaseGiftWithCSV(string sender_email, string receiver_email, decimal amount)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_PuchaseGift_WithCSV";
        //                command.Parameters.AddWithValue("@sender_email", sender_email);
        //                command.Parameters.AddWithValue("@receiver_email", receiver_email);
        //                command.Parameters.AddWithValue("@amount", amount);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.CommandType = CommandType.StoredProcedure;
        //                int rowsAdded = command.ExecuteNonQuery();
        //                conn.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public void insertPuchaseGiftWithCC(string sender_email, string receiver_email, decimal amount)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_PuchaseGift_WithCC";
        //                command.Parameters.AddWithValue("@sender_email", sender_email);
        //                command.Parameters.AddWithValue("@receiver_email", receiver_email);
        //                command.Parameters.AddWithValue("@amount", amount);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.CommandType = CommandType.StoredProcedure;
        //                int rowsAdded = command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public void insertPuchaseWithBTC(string sender_email, decimal amount)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_Puchase_WithBTC";
        //                command.Parameters.AddWithValue("@sender_email", sender_email);
        //                command.Parameters.AddWithValue("@amount", amount);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.CommandType = CommandType.StoredProcedure;
        //                int rowsAdded = command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public void insertFundWithBTC(string sender_email, decimal amount)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_Fund_WithBTC";
        //                command.Parameters.AddWithValue("@sender_email", sender_email);
        //                command.Parameters.AddWithValue("@amount", amount);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.CommandType = CommandType.StoredProcedure;
        //                int rowsAdded = command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public void insertPuchaseWithCSV(string sender_email, decimal amount)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_Purchase_WithCSV";
        //                command.Parameters.AddWithValue("@sender_email", sender_email);
        //                command.Parameters.AddWithValue("@amount", amount);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.CommandType = CommandType.StoredProcedure;
        //                int rowsAdded = command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}


        //public string insertOrderWithCSV(string sender_email, string id, string goodtype, decimal amount, decimal tax, string status, string extrainfo)
        //{
        //    try
        //    {
        //        string res = null;
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_OrderHistory";
        //                command.Parameters.AddWithValue("@account_email", sender_email);
        //                command.Parameters.AddWithValue("@externalid", id);
        //                command.Parameters.AddWithValue("@goodtype", goodtype);
        //                command.Parameters.AddWithValue("@currency", "CSV"); 
        //                command.Parameters.AddWithValue("@price", amount);
        //                command.Parameters.AddWithValue("@tax", tax);
        //                command.Parameters.AddWithValue("@ordertype", "PurchaseWithCSV");
        //                command.Parameters.AddWithValue("@status", status);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.Parameters.AddWithValue("@extrainfo", extrainfo);
        //                command.CommandType = CommandType.StoredProcedure;
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        res = reader["resid"].ToString().Trim();
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public string insertOrderWithBTC(string sender_email, string id, string goodtype, decimal amount, decimal tax, string status,string extrainfo)
        //{
        //    try
        //    {
        //        string res = null;
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "insert_OrderHistory";
        //                command.Parameters.AddWithValue("@account_email", sender_email);
        //                command.Parameters.AddWithValue("@externalid", id);
        //                command.Parameters.AddWithValue("@goodtype", goodtype);
        //                command.Parameters.AddWithValue("@currency", "BTC");
        //                command.Parameters.AddWithValue("@price", amount);
        //                command.Parameters.AddWithValue("@tax", tax);
        //                command.Parameters.AddWithValue("@ordertype", "PurchaseWithBTC");
        //                command.Parameters.AddWithValue("@status", status);
        //                command.Parameters.AddWithValue("@time", getEpochTime());
        //                command.Parameters.AddWithValue("@extrainfo", extrainfo);
        //                command.CommandType = CommandType.StoredProcedure;
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        res = reader["resid"].ToString().Trim();
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        public HistoryItem[] getHistory(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return null;
                }
                ArrayList list = new ArrayList();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_History";
                        command.Parameters.AddWithValue("@email", email);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HistoryItem item = new HistoryItem();
                                item.receiver_email = reader["receiver_email"].ToString().Trim();
                                item.sender_email = reader["sender_email"].ToString().Trim();
                                item.type = reader["type"].ToString().Trim();
                                item.time = reader["time"].ToString().Trim();
                                item.amount = reader["amount"].ToString().Trim();
                                item.time = translateEpochTime(Int32.Parse(item.time));
                                list.Add(item);
                            }
                        }
                    }
                }
                HistoryItem[] resArray = (HistoryItem[])list.ToArray(typeof(HistoryItem));
                return resArray;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        //public string getReceiveTotalCSVfromCSVOnly(string email)
        //{
        //    try
        //    {
        //        string res = null;
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "get_ReceivedSum_WithCSV";
        //                command.Parameters.AddWithValue("@email", email);
        //                command.CommandType = CommandType.StoredProcedure; 
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        res = reader["sum"].ToString().Trim();
        //                        break;
        //                    }
        //                }
        //                conn.Close();
        //            }
        //        }
        //        if (res == null)
        //        {
        //            res = "0";
        //        }
        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public string getBTCFundTotal(string email)
        //{
        //    try
        //    {
        //        string res = null;
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "get_FundSum_WithBTC";
        //                command.Parameters.AddWithValue("@email", email);
        //                command.CommandType = CommandType.StoredProcedure;
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        res = reader["sum"].ToString().Trim();
        //                        break;
        //                    }
        //                }
        //                conn.Close();
        //            }
        //        }
        //        if (res == null)
        //        {
        //            res = "0";
        //        }
        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        //public string getCSVPurchaseTotal(string email)
        //{
        //    try
        //    {
        //        string res = null;
        //        using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
        //        {
        //            using (SqlCommand command = conn.CreateCommand())
        //            {
        //                conn.Open();
        //                command.CommandText = "get_PurchaseSum_WithCSV";
        //                command.Parameters.AddWithValue("@email", email);
        //                command.CommandType = CommandType.StoredProcedure;
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        res = reader["sum"].ToString().Trim();
        //                        break;
        //                    }
        //                }
        //                conn.Close();
        //            }
        //        }
        //        if (res == null)
        //        {
        //            res = "0";
        //        }
        //        return res;
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.TraceError(e.ToString());
        //        throw e;
        //    }
        //}

        public string getSumByTransType(string email, string transtype)
        {
            try
            {
                string[] tmp = new string[2];
                string res = null;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_SumByTransType";
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@transtype", transtype);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tmp[0] = reader["sum"].ToString().Trim();
                                tmp[1] = reader["sumtax"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                try
                {
                    res = (Double.Parse(tmp[0]) + Double.Parse(tmp[1])).ToString();
                }
                catch (Exception e)
                {
                }
                if (res == null)
                {
                    res = "0";
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string getSendTotalCSV(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                string res = getSendTotalCSV_Inner(email);
                if (res == null)
                {
                    res = "0";
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string getSendTotalCSV_Inner(string email)
        {
            try
            {
                string res = getSumByTransType(email, "Purchase Gift CSV With CSV");
                if (res == null)
                {
                    res = "0";
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string getReceiveTotalCSV(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                string res = getReceiveTotalCSV_Inner(email);
                if (res == null)
                {
                    res = "0";
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string getReceiveTotalCSV_Inner(string email)
        {
            try
            {
                string res = getSumByTransType(email, "Receive Gift CSV");
                if (res == null)
                {
                    res = "0";
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string GetTotalCSV(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }

                string res = GetTotalCSV_Inner(email); 
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        
        public string GetTotalCSV_Inner(string email)
        {
            try
            {
                double funded_btc = Double.Parse(getSumByTransType(email, "Fund CSV With Bitcoin"));
                double funded_cc = Double.Parse(getSumByTransType(email, "Fund CSV With CreditCard"));
                double used = Double.Parse(getSumByTransType(email, "Purchase With CSV"));
                double received = Double.Parse(getReceiveTotalCSV_Inner(email));
                double sent = Double.Parse(getSendTotalCSV_Inner(email));
                var res = funded_btc + funded_cc - used + received - sent;
                return res.ToString();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string GetParseToken(string token, string token_value)
        {
            string res = null;
            try
            {
                res = parse_token(token, token_value)[0];
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Goods GetGoodsDetailByID(string type, string id, string token_value)
        {
            try
            {
                ArrayList list = new ArrayList();
                Goods res = new Goods();
                res = GetGoodsByID(id, token_value);
                DetailItem[] detailitem = GetDetailItemByEXID(type, id, token_value);
                res.detailitem = detailitem;
                return res;
            }
            catch (Exception e)
            {
                return null;
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public Goods GetGoodsDetailByToken(string token, string token_value)
        {
            try
            {
                ArrayList list = new ArrayList();
                Goods res = new Goods();
                string[] parse_result = parse_token(token, token_value);
                string id = parse_result[0];
                string type = parse_result[1];
                res = GetGoodsByID(id,token_value);
                DetailItem[] detailitem = GetDetailItemByEXID(type, id, token_value);
                res.detailitem = detailitem;
                return res;
            }
            catch (Exception e)
            {
                return null;
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public Goods GetGoodsByID(string id, string token_value)
        {
            try
            {
                Goods res = new Goods();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_GoodsByID";
                        command.Parameters.AddWithValue("@id", id);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.id = reader["id"].ToString().Trim();
                                res.name = reader["name"].ToString().Trim();
                                res.description = reader["description"].ToString().Trim();
                                res.merchant = reader["merchant"].ToString().Trim();
                                res.currency = reader["currency"].ToString().Trim();
                                res.price = reader["price"].ToString().Trim();
                                res.tax = reader["tax"].ToString().Trim();
                                res.info = reader["info"].ToString().Trim();
                                res.extrainfo = reader["extrainfo"].ToString().Trim();
                                res.status = reader["status"].ToString().Trim();
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                return null;
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string[] parse_token(string token, string token_value)
        {
            try
            {
                //bool trusted = veritySecurity(token_value, email);
                //if (!trusted)
                //{
                //    return "Untrusted Client Request";
                //}
                string[] res = new string[2];
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_parse_Token";
                        command.Parameters.AddWithValue("@token", token);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //ss = res[0] = reader["id"].ToString().Trim();
                                res[0] = reader["id"].ToString().Trim();
                                res[1] = reader["goodstype"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public DetailItem[] GetDetailItemByEXID(string type, string id, string token_value)
        {
            try
            {
                //bool trusted = veritySecurity(token_value, email);
                //if (!trusted)
                //{
                //    return "Untrusted Client Request";
                //}
                ArrayList list = new ArrayList();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_DetailItemByGoodsID";
                        command.Parameters.AddWithValue("@id", id);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetailItem item = new DetailItem();
                                item.id = reader["id"].ToString().Trim();
                                item.exid = reader["exid"].ToString().Trim();
                                item.name = reader["name"].ToString().Trim();
                                item.description = reader["description"].ToString().Trim();
                                item.merchant = reader["merchant"].ToString().Trim();
                                item.currency = reader["currency"].ToString().Trim();
                                item.price = reader["price"].ToString().Trim();
                                item.tax = reader["tax"].ToString().Trim();
                                item.info = reader["info"].ToString().Trim();
                                item.extrainfo = reader["extrainfo"].ToString().Trim();
                                item.status = reader["status"].ToString().Trim();
                                list.Add(item);
                            }
                        }
                    }
                }
                DetailItem[] resArray = (DetailItem[])list.ToArray(typeof(DetailItem));
                return resArray;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public int insertToken(string token, string id, string goodstype)
        {
            try
            {
                int res = 0;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "insert_Token";
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@goodstype", goodstype);
                        command.CommandType = CommandType.StoredProcedure;
                        res = command.ExecuteNonQuery();
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public int insertDetailItem(string exid, string name, string description, string merchant, string currency, decimal price, decimal tax, string info, string status, string extrainfo)
        {
            try
            {
                int res = 0;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "insert_DetailItem";
                        command.Parameters.AddWithValue("@exid", exid);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@merchant", merchant);
                        command.Parameters.AddWithValue("@currency", currency);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@tax", tax);
                        command.Parameters.AddWithValue("@info", info);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@extrainfo", extrainfo);
                        command.CommandType = CommandType.StoredProcedure;
                        res = command.ExecuteNonQuery();
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string insertOrderHistory(string email, string name, string description, string merchant, decimal price, decimal tax, string transtype, string status, string extrainfo, string source)
        {
            try
            {
                string res = null;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "insert_OrderHistory";
                        command.Parameters.AddWithValue("@account", email);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@merchant", merchant);
                        command.Parameters.AddWithValue("@price", price);
                        command.Parameters.AddWithValue("@tax", tax);
                        command.Parameters.AddWithValue("@transtype", transtype);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@time", getEpochTime());
                        command.Parameters.AddWithValue("@extrainfo", extrainfo);
                        command.Parameters.AddWithValue("@source", source);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res = reader["resid"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public int commitOrderHistory(string id, string email, string transtype, string status, string source)
        {
            try
            {
                int res = 0;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "commit_OrderHistory";
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@account", email);
                        command.Parameters.AddWithValue("@transtype", transtype);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@time", getEpochTime());
                        command.Parameters.AddWithValue("@source", source);
                        command.CommandType = CommandType.StoredProcedure;
                        res = command.ExecuteNonQuery();
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public int updateOrderHistory(string id, string email, string transtype, string status, string source)
        {
            try
            {
                int res = 0;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "update_OrderHistory";
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@account", email);
                        command.Parameters.AddWithValue("@transtype", transtype);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@time", getEpochTime());
                        command.Parameters.AddWithValue("@source", source);
                        command.CommandType = CommandType.StoredProcedure;
                        res = command.ExecuteNonQuery();
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public OrderHistory GetOrder(string email, string id, string token_value)
        {
            try
            {
                //bool trusted = veritySecurity(token_value, email);
                //if (!trusted)
                //{
                //    return "Untrusted Client Request";
                //}
                var res = new OrderHistory();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_Order";
                        command.Parameters.AddWithValue("@id", id);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.id = reader["id"].ToString().Trim();
                                res.account = reader["account"].ToString().Trim();
                                res.name = reader["name"].ToString().Trim();
                                res.description = reader["description"].ToString().Trim();
                                res.merchant = reader["merchant"].ToString().Trim();
                                res.price = reader["price"].ToString().Trim();
                                res.tax = reader["tax"].ToString().Trim();
                                res.transtype = reader["transtype"].ToString().Trim();
                                res.status = reader["status"].ToString().Trim();
                                res.time = reader["time"].ToString().Trim();
                                res.extrainfo = reader["extrainfo"].ToString().Trim();
                                res.source = reader["source"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                if (res == null)
                {
                    res = null;
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public OrderHistory[] GetOrderList(string email, string token_value)
        {
            try
            {
                //bool trusted = veritySecurity(token_value, email);
                //if (!trusted)
                //{
                //    return "Untrusted Client Request";
                //}
                ArrayList list = new ArrayList();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_OrderList";
                        command.Parameters.AddWithValue("@account", email);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var res = new OrderHistory();
                                res.id = reader["id"].ToString().Trim();
                                res.account = reader["account"].ToString().Trim();
                                res.name = reader["name"].ToString().Trim();
                                res.description = reader["description"].ToString().Trim();
                                res.merchant = reader["merchant"].ToString().Trim();
                                res.price = reader["price"].ToString().Trim();
                                res.tax = reader["tax"].ToString().Trim();
                                res.transtype = reader["transtype"].ToString().Trim();
                                res.status = reader["status"].ToString().Trim();
                                res.time = reader["time"].ToString().Trim();
                                res.extrainfo = reader["extrainfo"].ToString().Trim();
                                res.source = reader["source"].ToString().Trim();
                                list.Add(res);
                            }
                        }
                    }
                }
                OrderHistory[] resArray = (OrderHistory[])list.ToArray(typeof(OrderHistory));
                return resArray;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        //get directly by token part
        public DetailItem[] GetDetailItemByToken(string email, string token, string token_value)
        {
            try
            {
                //bool trusted = veritySecurity(token_value, email);
                //if (!trusted)
                //{
                //    return "Untrusted Client Request";
                //}
                string[] parse_result = parse_token(token, token_value);
                string id = parse_result[0];
                string type = parse_result[1];
                ArrayList list = new ArrayList();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_DetailItemByGoodsID";
                        command.Parameters.AddWithValue("@id", id);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DetailItem item = new DetailItem();
                                item.id = reader["id"].ToString().Trim();
                                item.exid = reader["exid"].ToString().Trim();
                                item.name = reader["name"].ToString().Trim();
                                item.description = reader["description"].ToString().Trim();
                                item.merchant = reader["merchant"].ToString().Trim();
                                item.currency = reader["currency"].ToString().Trim();
                                item.price = reader["price"].ToString().Trim();
                                item.tax = reader["tax"].ToString().Trim();
                                item.info = reader["info"].ToString().Trim();
                                item.extrainfo = reader["extrainfo"].ToString().Trim();
                                item.status = reader["status"].ToString().Trim();
                                list.Add(item);
                            }
                        }
                    }
                }
                DetailItem[] resArray = (DetailItem[])list.ToArray(typeof(DetailItem));
                return resArray;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public OrderHistory GetOrderByToken(string email, string token, string token_value)
        {
            try
            {
                //bool trusted = veritySecurity(token_value, email);
                //if (!trusted)
                //{
                //    return "Untrusted Client Request";
                //}
                string[] parse_result = parse_token(token, token_value);
                string id = parse_result[0];
                string type = parse_result[1];
                var res = new OrderHistory();
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_Order";
                        command.Parameters.AddWithValue("@id", id);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res.id = reader["id"].ToString().Trim();
                                res.account = reader["account"].ToString().Trim();
                                res.name = reader["name"].ToString().Trim();
                                res.description = reader["description"].ToString().Trim();
                                res.merchant = reader["merchant"].ToString().Trim();
                                res.price = reader["price"].ToString().Trim();
                                res.tax = reader["tax"].ToString().Trim();
                                res.transtype = reader["transtype"].ToString().Trim();
                                res.status = reader["status"].ToString().Trim();
                                res.time = reader["time"].ToString().Trim();
                                res.extrainfo = reader["extrainfo"].ToString().Trim();
                                res.source = reader["source"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                if (res == null)
                {
                    res = null;
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion

        #region PI Service
        //return a list of Payment Instruments registed under the given email
        public PIType.PaymentInstrument[] GetPI(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return null;
                }
                var puid = EmailToPuid(email);
                var accountId = GetAccountWithPuid(puid);
                var request = new PIType.GetPaymentInstrumentsInput
                {
                    CallerInfo = new PIType.CallerInfo
                    {
                        Requester = BuildPIIdentityFromAccountWithPuid(puid),
                        AccountId = accountId,
                    },

                    APIContext = new PIType.APIContext
                    {
                        TrackingGuid = Guid.NewGuid(),
                    },

                    Criteria = new PIType.PaymentInstrumentCriteria
                    {
                        ReturnPending = false,
                        ReturnRemoved = false,
                    },

                    PagingOption = null,

                    GetPaymentInstrumentFlag = 0,

                    PaymentMethodID = null,


                };
                CTPApiClient client = new CTPApiClient(config);

                int attempt = 0;
                var res = client.GetPaymentInstruments(request);
                while (!(res.Ack == PIType.AckCodeType.Success) && (attempt <= RetryTime))
                {
                    attempt += 1;
                    res = client.GetPaymentInstruments(request);
                }

                if (res.Ack == PIType.AckCodeType.Success)
                {
                    return res.PaymentInstrumentSet;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        //return the Stored Value Payment Instrument ID as a string of the given account
        //acount is specifiedy by puid and accountId
        public PIType.PaymentInstrument GetSVPI(string puid, string accountId)
        {
            try
            {
                var request = new PIType.GetPaymentInstrumentsInput
                {
                    CallerInfo = new PIType.CallerInfo
                    {
                        Requester = BuildPIIdentityFromAccountWithPuid(puid),
                        AccountId = accountId,
                    },

                    APIContext = new PIType.APIContext
                    {
                        TrackingGuid = Guid.NewGuid(),
                    },

                    Criteria = new PIType.PaymentInstrumentCriteria
                    {
                        ReturnPending = false,
                        ReturnRemoved = false,
                    },

                    PagingOption = null,

                    GetPaymentInstrumentFlag = 0,

                    PaymentMethodID = null,


                };
                CTPApiClient client = new CTPApiClient(config);

                int attempt = 0;
                var res = client.GetPaymentInstruments(request);
                while (!(res.Ack == PIType.AckCodeType.Success) && (attempt <= RetryTime))
                {
                    attempt += 1;
                    res = client.GetPaymentInstruments(request);
                }

                if (res.Ack == PIType.AckCodeType.Success)
                {
                    foreach(var pi in res.PaymentInstrumentSet) 
                    {
                        if (pi.Type == "StoredValuePaymentInstrument")
                        {
                            return pi;
                        }
                    };
                    return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public static string Serialize(object obj, string prefix = null)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                ns.Add(string.Empty, nameSpace);
            }
            else
            {
                ns.Add(prefix, nameSpace);
            }

            var serializer = new XmlSerializer(obj.GetType());
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, obj, ns);
                stream.Seek(0, SeekOrigin.Begin);
                var bytes = stream.ToArray();
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
        }

        public string CreditPayment(string puid, string accountId, string amount_s)
        {
            try
            {
                decimal amount = Decimal.Parse(amount_s);
                CTPApiClient client = new CTPApiClient(config);
                string errorXml;
                string skuReferenceXml = string.Empty;
                string skuReferenceInfoXml = string.Empty;
                string BstrPropertyXml = bstrPropertyXmlForMandateRefund;
                int ReasonCode = 252121094;
                string currency = "USD";
                var commentInfo = new CMATType.CommentInfo();
                commentInfo.CommentCode = AddArbitraryCreditOrChargeCommentCode;
                commentInfo.CommentText = "Fund csv by bitcoin";
                var commentXml = Serialize(commentInfo);
                string StoredValueLotExpirationDate = "2016-06-21T03:17:41";
                string StoredValueLotType = "Promotional";
                string StoredValueSku = "CXBXD-00001";

                client.CreditPaymentInstrumentEx3(
                    1798928818, 
                    1798928818, 
                    System.Guid.NewGuid().ToString(),
                    GetSVPI(puid,accountId).PaymentMethodID,
                    skuReferenceInfoXml,
                    BstrPropertyXml,
                    ReasonCode,
                    amount.ToString("f2", CultureInfo.InvariantCulture),
                    currency,
                    false,
                    commentXml,
                    StoredValueLotExpirationDate,
                    StoredValueLotType,
                    StoredValueSku,
                    out errorXml);
                return errorXml;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion

        #region Purchase CSV Service
        public CTType.PurchaseProductItemInput GeneratePurchaseProductItemInput(string SVPIId, decimal amount)
        {
            var res = new CTType.PurchaseStoredValueProductItemInput
            {
                RevenueInfo = new CTType.RevenueInfo
                {
                    RevenueAllocationPercentage = 1.0m,
                    FinancialReportingAllocationPriceType = CTType.FinancialReportingAllocationPriceType.Amount,
                    RevenueAllocationAmount = 0,
                    RevenueAllocationPriceType = CTType.RevenueAllocationPriceType.Percentage,
                    RevenueSKU = "SKU-12345",
                },
                ExternalProductItemId = "e6d63ecb-6e3e-4772-9ccc-7b4023c5119f",
                ProductType = "StoredValue",
                Description = "storedvalue-description",
                SalesModelType = CTType.SalesModelType.Reseller,
                Title = "storedvalue-title",
                Amount = amount,
                FundType = "Standard",
                StoredValuePaymentInstrumentId = SVPIId,
                //StoredValuePaymentInstrumentId = "sDYAAAAAAAAFAACA",
            };
            return res;
        }

        public CTType.PurchaseBundleInput GenerateBundleInput(int bundleNum, int productNum, string SVPIId, decimal amount)
        {
            var res = new CTType.PurchaseBundleInput
            {
                Description = "Description",
                ExternalBundleCatalogId = "100",
                ExternalBundleId = "1",
                Title = "Title",
                IsTaxExemptionAllowed = false,
                IsTaxIncluded = false,
                Amount = amount,
                Currency = "USD",
            };
            res.PurchaseProductItemInputSet = new CTType.PurchaseProductItemInput[productNum];
            for (int i = 0; i < productNum; i++)
            {
                res.PurchaseProductItemInputSet[i] = GeneratePurchaseProductItemInput(SVPIId, amount);
            }
            return res;
        }

        public CTType.PurchaseBundleInput[] GenerateBundleInput(string SVPIId, decimal amount)
        {
            return new CTType.PurchaseBundleInput[1] { GenerateBundleInput(1, 1, SVPIId, amount) };
        }

        public CTType.PurchaseInput GeneratePurchaseInput(string puid, string accountID, string paymentMethodID, string SVPIId, decimal amount)
        {
            var res = new CTType.PurchaseInput
            {
                APIContext = new CTType.APIContext
                {
                    TrackingGuid = Guid.NewGuid(),
                },
                CallerInfo = new CTType.CallerInfo
                {
                    Requester = BuildCTIdentityFromAccountWithPuid(puid),
                    AccountId = accountID,
                    Delegator = BuildCTIdentityFromAccountWithPuid(puid),
                },
                BillingInfo = new CTType.BillingInfo
                {
                    //TO-DO : BillingMode randomly chosen currently
                    BillingMode = CTType.BillingMode.ImmediateSettle,
                    PaymentMethod = new CTType.RegisteredPaymentMethod[1] { new CTType.RegisteredPaymentMethod { PaymentMethodID = paymentMethodID } },
                },
                Marketplace = new CTType.Marketplace
                {
                    MarkerplaceGuid = Guid.Parse("AF5C3309-9515-40B0-9B13-CEEF3559CA24"),
                    MarketplaceName = "Test Partner: 84DCDEB0-194C-4335-AD6C-6779F97EDE17",
                },

            };
            res.PurchaseContext = new CTType.PurchaseContext();
            res.PurchaseContext.ComputeOnly = false;
            res.PurchaseContext.TimeStamp = System.DateTime.UtcNow;
            res.PurchaseInfoInput = new CTType.PurchaseInfoInput();
            res.PurchaseInfoInput.ExternalPurchaseId = "1";
            res.PurchaseInfoInput.Description = "purchaseinfoinput_description";
            res.PurchaseInfoInput.Title = "PurchaseInfoInput_Title";
            res.PurchaseInfoInput.PurchaseBundleInputSet = new CTType.PurchaseBundleInput[1];
            res.PurchaseInfoInput.PurchaseBundleInputSet = GenerateBundleInput(SVPIId, amount);
            return res;
        }
        //Purchase Stored Value using credit card for a given account 
        //email : to specify the account
        //paymentMethodID : the PIID of the paying PI, e.g. credit card
        //SVPIId : the 
        //account : amount of SV to be bought
        public string Purchase(string email, string paymentMethodID, string amount, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                var puid = EmailToPuid(email);
                var accountId = GetAccountWithPuid(puid);
                string SVPIId = GetSVPI(puid, accountId).PaymentMethodID;
                CTType.PurchaseInput purchaseInput = GeneratePurchaseInput(puid, accountId, paymentMethodID, SVPIId, Decimal.Parse(amount));
                CTPApiClient client = new CTPApiClient(config);

                int attempt = 0;
                var res = client.PurchaseCSV(purchaseInput);
                while (!(res.Ack == CTType.AckCodeType.Success) && (attempt <= RetryTime))
                {
                    attempt += 1;
                    res = client.PurchaseCSV(purchaseInput);
                }

                if (res.Ack == CTType.AckCodeType.Success)
                {
                    //insertPuchaseCSV(email, Decimal.Parse(amount));
                    var res_string = insertOrderHistory(email, "CSV", "CSV", "Microsoft", Decimal.Parse(amount), 0, "Fund CSV With CreditCard", "Complete", "Fund CSV With CreditCard", "CreditCard");
                    return res_string;
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion 

        #region Purchase Gift Service
        public CTType.PurchaseProductItemInput GenerateGiftProductItemInput(string receiver_puid, string receiver_accountId, decimal amount)
        {
            var res = new CTType.GiftStoredValueProductItemInput
            {
                RevenueInfo = new CTType.RevenueInfo
                {
                    RevenueAllocationPercentage = 1.0m,
                    FinancialReportingAllocationPriceType = CTType.FinancialReportingAllocationPriceType.Amount,
                    RevenueAllocationAmount = 0,
                    RevenueAllocationPriceType = CTType.RevenueAllocationPriceType.Percentage,
                    RevenueSKU = "SKU-12345",
                },
                ExternalProductItemId = "e6d63ecb-6e3e-4772-9ccc-7b4023c5119f",
                ProductType = "StoredValue",
                Title = "storedvalue-title",
                Amount = amount,
                ReceiverBilliableAcctId = receiver_accountId,
                ReceiverIdentityInfo = new CTType.Identity
                {
                    IdentityType = "PUID",
                    IdentityValue = receiver_puid,
                },
                Description = "storedvalue-description",
                FundType = "Standard",
            };
            return res;
        }

        public CTType.PurchaseBundleInput GenerateGiftBundleInput(int bundleNum, int productNum, string receiver_puid, string receiver_accountId, decimal amount)
        {
            var res = new CTType.PurchaseBundleInput
            {
                Description = "Description",
                ExternalBundleCatalogId = "100",
                ExternalBundleId = "1",
                Title = "Title",
                IsTaxExemptionAllowed = false,
                IsTaxIncluded = false,
                Amount = amount,
                Currency = "USD",
            };
            res.PurchaseProductItemInputSet = new CTType.PurchaseProductItemInput[productNum];
            for (int i = 0; i < productNum; i++)
            {
                res.PurchaseProductItemInputSet[i] = GenerateGiftProductItemInput(receiver_puid, receiver_accountId, amount);
            }
            return res;
        }

        public CTType.PurchaseBundleInput[] GenerateGiftBundleInput(string receiver_puid, string receiver_accountID, decimal amount)
        {
            return new CTType.PurchaseBundleInput[1] { GenerateGiftBundleInput(1, 1, receiver_puid, receiver_accountID, amount) };
        }

        public CTType.PurchaseInput GenerateGiftPurchaseInput(string sender_puid, string sender_accountID, string paymentMethodID, string receiver_puid, string receiver_accountID, decimal amount)
        {
            var res = new CTType.PurchaseInput
            {
                APIContext = new CTType.APIContext
                {
                    TrackingGuid = Guid.NewGuid(),
                },
                CallerInfo = new CTType.CallerInfo
                {
                    Requester = BuildCTIdentityFromAccountWithPuid(sender_puid),
                    AccountId = sender_accountID,
                    Delegator = BuildCTIdentityFromAccountWithPuid(sender_puid),
                },
                BillingInfo = new CTType.BillingInfo
                {
                    //TO-DO : BillingMode randomly chosen currently
                    BillingMode = CTType.BillingMode.ImmediateSettle,
                    PaymentMethod = new CTType.RegisteredPaymentMethod[1] { new CTType.RegisteredPaymentMethod { PaymentMethodID = paymentMethodID } },
                },
                Marketplace = new CTType.Marketplace
                {
                    MarkerplaceGuid = Guid.Parse("AF5C3309-9515-40B0-9B13-CEEF3559CA24"),
                    MarketplaceName = "Test Partner: 84DCDEB0-194C-4335-AD6C-6779F97EDE17",
                },

            };
            res.PurchaseContext = new CTType.PurchaseContext();
            res.PurchaseContext.ComputeOnly = false;
            res.PurchaseContext.TimeStamp = System.DateTime.UtcNow;
            res.PurchaseInfoInput = new CTType.PurchaseInfoInput();
            res.PurchaseInfoInput.ExternalPurchaseId = "1";
            res.PurchaseInfoInput.Description = "purchaseinfoinput_description";
            res.PurchaseInfoInput.Title = "PurchaseInfoInput_Title";
            res.PurchaseInfoInput.PurchaseBundleInputSet = new CTType.PurchaseBundleInput[1];
            res.PurchaseInfoInput.PurchaseBundleInputSet = GenerateGiftBundleInput(receiver_puid, receiver_accountID, amount);
            return res;
        }

        //Purchase Gift Stored Value for receiver_email account by sender_email account using sender_paymentMethodID PI
        //sender_email : sender's account
        //receiver_email : receiver's account
        //sender_paymentMethodID : the PI sender used for payment
        //amount : amount of Stored Value to purchase
        public string PurchaseGift(string sender_email, string sender_paymentMethodID, string receiver_email, string amount, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, sender_email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                string sender_puid = EmailToPuid(sender_email);
                string sender_accountId = GetAccountWithPuid(sender_puid);
                string receiver_puid = EmailToPuid(receiver_email);
                string receiver_accountId = GetAccountWithPuid(receiver_puid);
                CTType.PurchaseInput purchaseInput = GenerateGiftPurchaseInput(sender_puid, sender_accountId, sender_paymentMethodID, receiver_puid, receiver_accountId, Decimal.Parse(amount));
                CTPApiClient client = new CTPApiClient(config);

                int attempt = 0;
                var res = client.PurchaseCSV(purchaseInput);
                while (!(res.Ack == CTType.AckCodeType.Success) && (attempt <= RetryTime))
                {
                    attempt += 1;
                    res = client.PurchaseCSV(purchaseInput);
                }

                if (res.Ack == CTType.AckCodeType.Success)
                {
                    //insertPuchaseGiftWithCC(sender_email, receiver_email, Decimal.Parse(amount));
                    var res_string = insertOrderHistory(sender_email, "Gift CSV", "Gift CSV", receiver_email, Decimal.Parse(amount), 0, "Purchase Gift CSV With CreditCard", "Complete", "Purchase Gift CSV With CreditCard", "CreditCard");
                    insertOrderHistory(receiver_email, "Gift CSV", "Gift CSV", sender_email, Decimal.Parse(amount), 0, "Receive Gift CSV", "Complete", "Gift CSV With CreditCard from " + sender_email, "CreditCard");
                    var uri = EmailToURI(receiver_email);
                    sendNotification(receiver_email, uri, sender_email, "has sent " + amount.ToString() + " CSV to you.");
                    return res_string;
                }
                else
                {
                    //insertPuchaseGiftWithCSV(sender_email, receiver_email, Decimal.Parse(amount));
                    Decimal totalCSV = Decimal.Parse(GetTotalCSV_Inner(sender_email));
                    if (totalCSV < (Decimal.Parse(amount)))
                        return "Failed";
                    var res_string = insertOrderHistory(sender_email, "Gift CSV", "Gift CSV", receiver_email, Decimal.Parse(amount), 0, "Purchase Gift CSV With CSV", "Complete", "Purchase Gift CSV With CSV", "CSV");
                    insertOrderHistory(receiver_email, "Gift CSV", "Gift CSV", sender_email, Decimal.Parse(amount), 0, "Receive Gift CSV", "Complete", "Gift CSV With CreditCard from " + sender_email, "CSV");
                    var uri = EmailToURI(receiver_email);
                    sendNotification(receiver_email, uri, sender_email, "has sent " + amount.ToString() + " CSV to you.");
                    return res_string;
                }
                
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion

        #region BTC Service
        public string BTCResponse(result res)
        {
            string exid = res.externalKey;
            string status = res.status;
            insertOrderHistory(exid.ToString(), "", "", "", 0, 0, "Purchase With CSV", "Complete", "", "CSV");
            OrderHistory order = GetOrder("placeholder", exid, "placeholder");

            if (status == "paid" && order.transtype == "Fund CSV With Bitcoin")
            {
                string account_email = order.account;
                string amount_s = order.price;
                string puid = EmailToPuid(account_email);
                string accountId = GetAccountWithPuid(puid);
                CreditPayment(puid, accountId, '-' + amount_s);
            }
            if (status == "paid")
            {
                status = "Complete";
            }
            int t = updateOrderHistory(exid, order.account, order.transtype, status, "BTC"); 
            return t.ToString();
        }

        public string BTCResponseString(Stream res)
        {
            try
            {
                var reader = new StreamReader(res);
                string ss = reader.ReadToEnd();

                result m = JsonConvert.DeserializeObject<result>(ss);
                string exid = m.externalKey;
                string status = m.status;
                OrderHistory order = GetOrder("placeholder", exid, "placeholder");
                if (status == "paid" && order.transtype == "Fund CSV With Bitcoin")
                {
                    string account_email = order.account;
                    string amount_s = order.price;
                    string puid = EmailToPuid(account_email);
                    string accountId = GetAccountWithPuid(puid);
                    CreditPayment(puid, accountId, '-' + amount_s);
                }
                if (status == "paid")
                {
                    status = "Complete";
                }
                int t = updateOrderHistory(exid, order.account, order.transtype, status, "BTC");
                return t.ToString();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string FundWithBTC(string email, string amount, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                string id;
                id = insertOrderHistory(email, "CSV", "CSV", "Microsoft", Decimal.Parse(amount), 0, "Fund CSV With Bitcoin", "Created", "Fund CSV With Bitcoin", "BTC");

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                string method = "createPurchaseOrder";
                string callback_url = "http://mintrestapi2.cloudapp.net/MintRESTfulAPI.svc/BTCResponse";
                string param = amount + ",USD," + callback_url + "," + callback_url + "," + id + ",Funding CSV";
                TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
                long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds * 1000);
                string tonce = Convert.ToString(milliSeconds);
                NameValueCollection parameters = new NameValueCollection() { 
                    { "tonce", tonce },
                    { "accesskey", accessKey },
                    { "requestmethod", "post" },
                    { "id", "1" },
                    { "method", method },
                    { "params", param } 
                };
                string paramsHash = BTCService.GetHMACSHA1Hash(secretKey, BTCService.BuildQueryString(parameters));
                string base64String = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
                string url = "https://api.btcchina.com/api.php/payment";
                string postData = "{\"method\": \"" + method + "\", \"params\": [" + amount + ",\"USD\",\"" + callback_url + "\",\"" + callback_url + "\",\"" + id + "\",\"Funding CSV\"], \"id\": 1}";
                //res.id = postData + param;
                Response res = BTCService.SendPostByWebRequest(url, base64String, tonce, postData);
                if (res.result != null)
                {
                    //insert into database
                }
                return res.result.url;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public Response PurchaseWithBTC(string email, string type, string id, string token_value)
        {
            Response res = new Response();
            double total;
            if (type == "Good")
            {
                Goods goods = GetGoodsByID(id, token_value);
                res.id = insertOrderHistory(email, goods.name, goods.description, goods.merchant, Decimal.Parse(goods.price), Decimal.Parse(goods.tax), "Purchase With Bitcoin", "Created", id, "BTC");
                id = res.id;
                total = Double.Parse(goods.price) + Double.Parse(goods.tax);
            }
            else
            {
                OrderHistory order = GetOrder(email, id, token_value);
                total = Double.Parse(order.price) + Double.Parse(order.tax);
                int t = commitOrderHistory(id, email, "Purchase With Bitcoin", "Created", "BTC");
                if (t > 0)
                    res.id = id;
                else
                    res.id = "Failed";
            }
            // For https.
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string method = "createPurchaseOrder";
            string callback_url = "http://mintrestapi2.cloudapp.net/MintRESTfulAPI.svc/BTCResponse";
            string param = total.ToString() + ",USD," + callback_url + "," + callback_url + "," + id + ",BTC Order";
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds * 1000);
            string tonce = Convert.ToString(milliSeconds);
            NameValueCollection parameters = new NameValueCollection() { 
                { "tonce", tonce },
                { "accesskey", accessKey },
                { "requestmethod", "post" },
                { "id", "1" },
                { "method", method },
                { "params", param } 
            };
            string paramsHash = GetHMACSHA1Hash(secretKey, BuildQueryString(parameters));
            string base64String = Convert.ToBase64String(
            Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
            string url = "https://api.btcchina.com/api.php/payment";
            string postData = "{\"method\": \"" + method + "\", \"params\": [" + total.ToString() + ",\"USD\",\"" + callback_url + "\",\"" + callback_url + "\",\"" + id + "\",\"BTC Order\"], \"id\": 1}";
            //res.id = postData + param;
            res = SendPostByWebRequest(url, base64String, tonce, postData);
            if (res.result != null)
            {
                //insert into database
            }
            return res;
        }

        public Response PurchaseWithBTCByToken(string email, string token, string token_value)
        {
            string[] parse_result = parse_token(token, token_value);
            string id = parse_result[0];
            string type = parse_result[1];
            Response res = new Response();
            double total;
            if (type == "Good")
            {
                Goods goods = GetGoodsByID(id, token_value);
                res.id = insertOrderHistory(email, goods.name, goods.description, goods.merchant, Decimal.Parse(goods.price), Decimal.Parse(goods.tax), "Purchase With Bitcoin", "Created", id, "BTC");
                id = res.id;
                total = Double.Parse(goods.price) + Double.Parse(goods.tax);
            }
            else
            {
                OrderHistory order = GetOrder(email, id, token_value);
                total = Double.Parse(order.price) + Double.Parse(order.tax);
                int t = commitOrderHistory(id, email, "Purchase With Bitcoin", "Created", "BTC");
                if (t > 0)
                    res.id = id;
                else
                    res.id = "Failed";
            }
            // For https.
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string method = "createPurchaseOrder";
            string callback_url = "http://mintrestapi2.cloudapp.net/MintRESTfulAPI.svc/BTCResponse";
            string param = total.ToString() + ",USD," + callback_url + "," + callback_url + "," + id + ",TestOrder";
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds * 1000);
            string tonce = Convert.ToString(milliSeconds);
            NameValueCollection parameters = new NameValueCollection() { 
                { "tonce", tonce },
                { "accesskey", accessKey },
                { "requestmethod", "post" },
                { "id", "1" },
                { "method", method },
                { "params", param } 
            };
            string paramsHash = BTCService.GetHMACSHA1Hash(secretKey, BTCService.BuildQueryString(parameters));
            string base64String = Convert.ToBase64String(
            Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
            string url = "https://api.btcchina.com/api.php/payment";
            string postData = "{\"method\": \"" + method + "\", \"params\": [" + total.ToString() + ",\"USD\",\"" + callback_url + "\",\"" + callback_url + "\",\"" + id + "\",\"TestOrder\"], \"id\": 1}";
            //res.id = postData + param;
            res = BTCService.SendPostByWebRequest(url, base64String, tonce, postData);
            return res;
        }

        public Response GetPurchaseOrder(string email, string id)
        {
            // For https.
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string method = "getPurchaseOrder";
            string param = string.Format("{0}", id);
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long milliSeconds = Convert.ToInt64(timeSpan.TotalMilliseconds * 1000);
            string tonce = Convert.ToString(milliSeconds);
            NameValueCollection parameters = new NameValueCollection() { 
                { "tonce", tonce },
                { "accesskey", accessKey },
                { "requestmethod", "post" },
                { "id", "1" },
                { "method", method },
                { "params", param } 
            };
            string paramsHash = GetHMACSHA1Hash(secretKey, BuildQueryString(parameters));
            string base64String = Convert.ToBase64String(
            Encoding.ASCII.GetBytes(accessKey + ':' + paramsHash));
            string url = "https://api.btcchina.com/api.php/payment";
            string postData = "{\"method\": \"" + method + "\", \"params\": ["+ id + "], \"id\": 1}";
            var res = SendPostByWebRequest(url, base64String, tonce, postData);
            if (res.result != null)
            {
                //update database
            }
            return res;
        }


        #endregion

        #region Purchase with CSV
        //get infomation by id
        public string PurchaseWithCSV(string email, string type, string id, string token_value)
        {
            try
            {
                //check for access auth
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                //check for whether CSV is enough
                string res = null;
                if (type == "Good")
                {
                    Goods goods = GetGoodsByID(id, token_value);
                    Decimal totalCSV = Decimal.Parse(GetTotalCSV_Inner(email));
                    if (totalCSV < (Decimal.Parse(goods.price) + Decimal.Parse(goods.tax)))
                        return "Failed";
                    res = insertOrderHistory(email, goods.name, goods.description, goods.merchant, Decimal.Parse(goods.price), Decimal.Parse(goods.tax), "Purchase With CSV", "Complete", id, "CSV");
                    return res;
                }
                if (type == "Order")
                {
                    OrderHistory order = GetOrder(email, id, token_value);
                    Decimal totalCSV = Decimal.Parse(GetTotalCSV_Inner(email));
                    if (totalCSV < (Decimal.Parse(order.price) + Decimal.Parse(order.tax)))
                        return "Failed";
                    int t = commitOrderHistory(id, email, "Purchase With CSV", "Complete", "CSV");
                    if (t > 0)
                        res = id;
                    else
                        res = "Failed";
                    return res;
                }
                else
                    return "Failed";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        //get information by token part
        public string PurchaseWithCSVByToken(string email, string token, string token_value)
        {
            try
            {
                //check for access auth
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                //check for whether CSV is enough
                string[] parse_result = parse_token(token, token_value);
                string id = parse_result[0];
                string type = parse_result[1];
                string res = null;
                if (type == "Good")
                {
                    Goods goods = GetGoodsByID(id, token_value);
                    Decimal totalCSV = Decimal.Parse(GetTotalCSV_Inner(email));
                    if (totalCSV < (Decimal.Parse(goods.price) + Decimal.Parse(goods.tax)))
                        return "Failed";
                    res = insertOrderHistory(email, goods.name, goods.description, goods.merchant, Decimal.Parse(goods.price), Decimal.Parse(goods.tax), "Purchase With CSV", "Complete", id, "CSV");
                    return res;
                }
                if (type == "Order")
                {
                    OrderHistory order = GetOrder(email, id, token_value);
                    Decimal totalCSV = Decimal.Parse(GetTotalCSV_Inner(email));
                    if (totalCSV < (Decimal.Parse(order.price) + Decimal.Parse(order.tax)))
                        return "Failed";
                    int t = commitOrderHistory(id, email, "Purchase With CSV", "Complete", "CSV");
                    if (t > 0)
                        res = id;
                    else
                        res = "Failed";
                    return res;
                }
                else
                    return "Failed";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion
        //GetStatementEx call
        public string GetSE(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                var puid = EmailToPuid(email);
                CTPApiClient client = new CTPApiClient(config);
                string errorXml;
                string accountStatementInfoSetXml;
                string userNotificationSetXml;
                client.GetStatementEx(1798928818, 1798928818, getHighId(puid), getLowId(puid), null, 0, 0, 2, true, null, out errorXml, out accountStatementInfoSetXml, out userNotificationSetXml);
                return accountStatementInfoSetXml;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        #region Receive Operation
        public string ReceiveRequest(string email, string req, string token_value)
        {
            try
            {
                string res = null;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        string token = System.Guid.NewGuid().ToString();
                        conn.Open();
                        command.CommandText = "insert_ReceiveRequest";
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@req", req);
                        command.CommandType = CommandType.StoredProcedure;
                        int rowsAdded = command.ExecuteNonQuery();
                        res = token;
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string GetReceiveRequest(string email, string token, string token_value)
        {
            try
            {
                string res = null;
                using (SqlConnection conn = new SqlConnection(connString2Builder.ToString()))
                {
                    using (SqlCommand command = conn.CreateCommand())
                    {
                        conn.Open();
                        command.CommandText = "get_ReceiveRequest";
                        command.Parameters.AddWithValue("@token", token);
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                res = reader["req"].ToString().Trim();
                                break;
                            }
                        }
                    }
                }
                return res;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion 

        public string CreateOrder(string email, string token_value, CreateOrderRequest request)
        {
            
            try
            {
                //check for access auth
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                var order = request.orderhistory;
                var detailitemlist = request.detailitems;
                var order_id = insertOrderHistory(order.account, order.name, order.description, order.merchant, Decimal.Parse(order.price), Decimal.Parse(order.tax), null, "Pending", null, null);
                foreach(DetailItem item in detailitemlist)
                {
                    insertDetailItem(order_id, item.name, item.description, item.merchant, item.currency, Decimal.Parse(item.price), Decimal.Parse(item.tax), item.info, item.status, item.extrainfo);
                }
                string token = System.Guid.NewGuid().ToString();
                insertToken(token, order_id, "Order");
                return token;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        #region Notification
        public void sendNotification(string email, string subscriptionUri, string content1,string content2)
        {
            try
            {
                // Get the URI that the Microsoft Push Notification Service returns to the push client when creating a notification channel.
                // Normally, a web service would listen for URIs coming from the web client and maintain a list of URIs to send
                // notifications out to.

                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

                // Create an HTTPWebRequest that posts the toast notification to the Microsoft Push Notification Service.
                // HTTP POST is the only method allowed to send the notification.
                sendNotificationRequest.Method = "POST";

                // The optional custom header X-MessageID uniquely identifies a notification message. 
                // If it is present, the same value is returned in the notification response. It must be a string that contains a UUID.
                // sendNotificationRequest.Headers.Add("X-MessageID", "<UUID>");

                // Create the toast message.
                string toastMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<wp:Notification xmlns:wp=\"WPNotification\">" +
                   "<wp:Toast>" +
                        "<wp:Text1>" + content1.Split('@')[0] + "</wp:Text1>" +
                        "<wp:Text2>" + content2 + "</wp:Text2>" +
                        "<wp:Param>/Views/LoadingPage.xaml</wp:Param>" +
                   "</wp:Toast> " +
                "</wp:Notification>";

                // Set the notification payload to send.
                byte[] notificationMessage = Encoding.Default.GetBytes(toastMessage);

                // Set the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "toast");
                sendNotificationRequest.Headers.Add("X-NotificationClass", "2");


                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                // Send the notification and get the response.
                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                if (notificationStatus.Equals("Received") == false)
                {
                    storeNotification(email, content1, content2);
                }
                // Display the response from the Microsoft Push Notification Service.  
                // Normally, error handling code would be here. In the real world, because data connections are not always available,
                // notifications may need to be throttled back if the device cannot be reached.
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return;
            }
        }

        public string updateURI(string email, string uri, string token_value)
        {
            try
            {
                //check for access auth
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                updateURI(email, uri);
                return "Success";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public void storeNotification(string email, string content1, string content2)
        {
            try
            {
                insertNotification(email, content1, content2);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }

        public string resendNotification(string email, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                Notification[] noti_array = getNotification(email);
                deleteNotification(email);
                foreach(var noti in noti_array)
                {
                    sendNotification(noti.email, noti.uri, noti.content1, noti.content2);
                }
                return "success";
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion

        #region VersionConfig
        public string ConfigSync(string email, string version, string token_value)
        {
            try
            {
                bool trusted = veritySecurity(token_value, email);
                if (!trusted)
                {
                    return "Untrusted Client Request";
                }
                string url = urlconfig(version);
                return url;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw e;
            }
        }
        #endregion
    }
}
