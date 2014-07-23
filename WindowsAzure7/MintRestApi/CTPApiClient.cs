using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CAType = Microsoft.Commerce.Proxy.AccountService;
using PIType = Microsoft.Commerce.Proxy.PaymentInstrumentService;
using CTType = Microsoft.Commerce.Proxy.TransactionService.v201001;
using TaxType = Microsoft.Commerce.Proxy.TaxService;

namespace Microsoft.CTP.SDK.Client
{
    public sealed class CTPApiClient : CTPApiInterface
    {
        #region Field
        private CommerceApiClient commerceAccountClient;
        private CommerceApiClient commercePIClient;
        private CommerceApiClient commerceTaxClient;
        private CommerceApiClient commerceTransactionClient;
        private ScsApiClient scsApiClient;
        #endregion

        #region Constructor
        public CTPApiClient(CTPConfiguration Configuration)
        {
            InitializeClient(Configuration.SoapServer, Configuration.CertFile);
        }
        public void InitializeClient(string SoapServer, string CertFile)
        {
            commerceAccountClient = new CommerceApiClient(
                string.Format("https://{0}/Commerce/Account/AccountWebService.svc?wsdl", SoapServer),
                typeof(CAType.AccountServiceClient),
                "Microsoft.CommerceAccount.Interfaces.IAccountService",
                CertFile);
            commercePIClient = new CommerceApiClient(
                string.Format("https://{0}/Commerce/PaymentInstrument/PaymentInstrumentWebService.svc?wsdl", SoapServer),
                typeof(PIType.PaymentInstrumentServiceClient),
                "Microsoft.CommerceAccount.Interfaces.IPaymentInstrumentService",
                CertFile);
            commerceTransactionClient = new CommerceApiClient(
                string.Format("https://{0}/CommerceTransaction/v1/TransactionWebService.svc?wsdl", SoapServer),
                typeof(CTType.TransactionServiceClient),
                "Microsoft.Transaction.Interfaces.V201001.ITransactionService",
                CertFile);
            commerceTaxClient = new CommerceApiClient(
                string.Format("https://{0}/Commerce/Tax/TaxWebService.svc?wsdl", SoapServer),
                typeof(TaxType.TaxServiceClient),
                "Microsoft.CommerceTax.Interfaces.ITaxService",
                CertFile);
            scsApiClient = new ScsApiClient(
                string.Format("https://{0}/scs/scsapiwebservice.asmx?wsdl", SoapServer),
                CertFile);
        }
        #endregion

        #region CommerceAccount API

        public CAType.GetAccountOutput GetAccount(CAType.GetAccountInput getAccountInput)
        {
            return commerceAccountClient.InvokeMethod<CAType.GetAccountOutput>("GetAccount", new object[] { getAccountInput });
        }

        public CAType.CreateAccountOutput CreateAccount(CAType.CreateAccountInput createAccountInput)
        {
            return commerceAccountClient.InvokeMethod<CAType.CreateAccountOutput>("CreateAccount", new object[] { createAccountInput });
        }

        public PIType.GetPaymentInstrumentsOutput GetPaymentInstruments(PIType.GetPaymentInstrumentsInput getPaymentInstrumentsInput)
        {
            return commercePIClient.InvokeMethod<PIType.GetPaymentInstrumentsOutput>("GetPaymentInstruments", new object[] { getPaymentInstrumentsInput });
        }

        public CTType.PurchaseOutput PurchaseCSV(CTType.PurchaseInput purchaseInput)
        {
            return commerceTransactionClient.InvokeMethod<CTType.PurchaseOutput>("Purchase", new object[] { purchaseInput });
        }

        public void GetStatementEx(
            int delegateIdHigh,
            int delegateIdLow,
            int requesterIdHigh,
            int requesterIdLow,
            string objectId,
            uint beginBillingPeriodId,
            uint endBillingPeriodId,
            byte returnStatementSet,
            bool returnNotificationSet,
            string orderId,
            out string errorXml,
            out string accountStatementInfoSetXml,
            out string userNotificationSetXml
            )
        {
            object[] parameters = new object[]
                {
                   delegateIdHigh,
                   delegateIdLow,
                   requesterIdHigh,
                   requesterIdLow,
                   objectId, //objectId
                   (uint)beginBillingPeriodId,
                   (uint)endBillingPeriodId,
                   (byte)returnStatementSet,
                   true,
                   "0", //orderId
                   null,
                   null,
                   null,
                };
            scsApiClient.InvokeMethod("GetStatementEx", parameters);
            errorXml = parameters[10] as string;
            accountStatementInfoSetXml = parameters[11] as string;
            userNotificationSetXml = parameters[12] as string;
        }

        public void CreditPaymentInstrumentEx3(int lDelegateIdHigh, int lDelegateIdLow, string bstrTrackingGUID, string bstrPaymentInstrumentId, string bstrSkuReferenceXML, string bstrPropertyXML, int lFinancialReportingCode, string bstrAmount, string currency, bool fImmediatelySettle, string bstrCommentInfoXML, string bstrStoredValueLotExpirationDate, string bstrStoredValueLotType, string bstrStoredValueSku, out string pbstrError)
        {
            object[] parameters = new object[] {
                    lDelegateIdHigh,
                    lDelegateIdLow,
                    bstrTrackingGUID,
                    bstrPaymentInstrumentId,
                    bstrSkuReferenceXML,
                    bstrPropertyXML,
                    lFinancialReportingCode,
                    bstrAmount,
                    currency,
                    fImmediatelySettle,
                    bstrCommentInfoXML,
                    bstrStoredValueLotExpirationDate,
                    bstrStoredValueLotType,
                    bstrStoredValueSku,
                    null
            };
            scsApiClient.InvokeMethod("CreditPaymentInstrumentEx3", parameters);
            pbstrError = ((string)(parameters[14]));
        }
        #endregion
    }

}
