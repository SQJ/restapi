using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CTType = Microsoft.Commerce.Proxy.TransactionService.v201001;
using CAType = Microsoft.Commerce.Proxy.AccountService;
using PIType = Microsoft.Commerce.Proxy.PaymentInstrumentService;
using TaxType = Microsoft.Commerce.Proxy.TaxService;

namespace Microsoft.CTP.SDK.Client
{
    public interface CTPApiInterface
    {

        CAType.GetAccountOutput GetAccount(CAType.GetAccountInput getAccountInput);

       
    }

}
