using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using PIType = Microsoft.Commerce.Proxy.PaymentInstrumentService;
using CTType = Microsoft.Commerce.Proxy.TransactionService.v201001;
using System.IO;
using System.ServiceModel.Channels;

namespace MintRestApi
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMintRESTfulAPI" in both code and config file together.
    [ServiceContract]
    public interface IMintRESTfulAPI
    {
        [OperationContract]
        [WebGet(UriTemplate = "TotalSent/{email}?access_token={token_value}")]
        string getSendTotalCSV(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "TotalReceived/{email}?access_token={token_value}")]
        string getReceiveTotalCSV(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "TotalCSV/{email}?access_token={token_value}")]
        string GetTotalCSV(string email, string token_value);        

        [OperationContract]
        [WebGet(UriTemplate = "Account/{email}?access_token={token_value}")]
        string GetAccount(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "PI/{email}?access_token={token_value}")]
        PIType.PaymentInstrument[] GetPI(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "History/{email}?access_token={token_value}")]
        HistoryItem[] getHistory(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "Purchase/{email}/{paymentMethodID}/{amount}?access_token={token_value}")]
        string Purchase(string email, string paymentMethodID, string amount, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "FundWithBTC/{email}/{amount}?access_token={token_value}")]
        string FundWithBTC(string email, string amount, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "PurchaseGift/{sender_email}/{sender_paymentMethodID}/{receiver_email}/{amount}?access_token={token_value}")]
        string PurchaseGift(string sender_email, string sender_paymentMethodID, string receiver_email, string amount, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "PurchaseWithBTC/{email}/{type}/{id}?access_token={token_value}")]
        Response PurchaseWithBTC(string email, string type, string id, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "PurchaseWithBTC/{email}/{token}?access_token={token_value}")]
        Response PurchaseWithBTCByToken(string email, string token, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "PurchaseWithCSV/{email}/{type}/{id}?access_token={token_value}")]
        string PurchaseWithCSV(string email, string type, string id, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "PurchaseWithCSV/{email}/{token}?access_token={token_value}")]
        string PurchaseWithCSVByToken(string email, string token, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "GetPurchaseOrder/{email}/{id}")]
        Response GetPurchaseOrder(string email, string id);

        [OperationContract]
        [WebGet(UriTemplate = "StatementEx/{email}?access_token={token_value}")]
        string GetSE(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "ParseToken/{token}?access_token={token_value}")]
        string GetParseToken(string token, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "Getgood/{token}?access_token={token_value}")]
        Goods GetGoodsDetailByToken(string token, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "GetgoodbyID/{type}/{id}?access_token={token_value}")]
        Goods GetGoodsDetailByID(string type, string id, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "DetailItem/{type}/{id}?access_token={token_value}")]
        DetailItem[] GetDetailItemByEXID(string type, string id, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "DetailItemByToken/{email}/{token}?access_token={token_value}")]
        DetailItem[] GetDetailItemByToken(string email, string token, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "GetOrder/{email}/{id}?access_token={token_value}")]
        OrderHistory GetOrder(string email, string id, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "GetOrderByToken/{email}/{token}?access_token={token_value}")]
        OrderHistory GetOrderByToken(string email, string token, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "GetOrderHistory/{email}?access_token={token_value}")]
        OrderHistory[] GetOrderList(string email, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "ReceiveRequest/{email}/{req}?access_token={token_value}")]
        String ReceiveRequest(string email, string req, string token_value);

        [OperationContract]
        [WebGet(UriTemplate = "GetReceiveRequest/{email}/{token}?access_token={token_value}")]
        String GetReceiveRequest(string email, string token, string token_value);

        [OperationContract]
        [WebInvoke(UriTemplate = "BTCResponse", Method="POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string BTCResponse(result res);

        [OperationContract]
        [WebInvoke(UriTemplate = "BTCResponseOp", Method = "POST")]
        string BTCResponseString(Stream stm);

        [OperationContract]
        [WebGet(UriTemplate = "CreditPayment/{puid}/{accountId}/{amount_s}")]
        string CreditPayment(string puid, string accountId, string amount_s);


        [OperationContract]
        [WebGet(UriTemplate = "updateURI/{email}?uri={uri}&access_token={token_value}")]
        string updateURI(string email, string uri, string token_value);

    }

    [DataContract]
    public class HistoryItem
    {
        [DataMember]
        public string sender_email { get; set; }

        [DataMember]
        public string receiver_email { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string amount { get; set; }

        [DataMember]
        public string time { get; set; }
    }

    [DataContract]
    public class result
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string externalKey { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public string currency { get; set; }
        [DataMember]
        public double btcPrice { get; set; }
        [DataMember]
        public string creationTime { get; set; }
        [DataMember]
        public string expirationTime { get; set; }
        [DataMember]
        public string status { get; set; }
    }

    [DataContract]
    public class Response
    {
        [DataMember]
        public result result { get; set; }
        [DataMember]
        public string id { get; set; }
    }

    [DataContract]
    public class Goods
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string merchant { get; set; }
        [DataMember]
        public string currency { get; set; }
        [DataMember]
        public string price { get; set; }
        [DataMember]
        public string tax { get; set; }
        [DataMember]
        public string info { get; set; }
        [DataMember]
        public string extrainfo { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public DetailItem[] detailitem { get; set; }
    }

    [DataContract]
    public class DetailItem
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string exid { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string merchant { get; set; }
        [DataMember]
        public string currency { get; set; }
        [DataMember]
        public string price { get; set; }
        [DataMember]
        public string tax { get; set; }
        [DataMember]
        public string info { get; set; }
        [DataMember]
        public string extrainfo { get; set; }
        [DataMember]
        public string status { get; set; }
    }

    [DataContract]
    public class OrderHistory
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string account { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string merchant { get; set; }
        [DataMember]
        public string price { get; set; }
        [DataMember]
        public string tax { get; set; }
        [DataMember]
        public string transtype { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string time { get; set; }
        [DataMember]
        public string extrainfo { get; set; }
        [DataMember]
        public string source { get; set; }
    }

    [DataContract]
    public class CreateOrderRequest
    {
        [DataMember]
        public OrderHistory orderhistory;
        public DetailItem[] detailitems;
    }
}
