using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ApplicationServer.Caching;

using PIType = Microsoft.Commerce.Proxy.PaymentInstrumentService;

namespace MintRestApi
{
    public class CacheData
    {
        public CacheData()
        {
            DataCacheFactory cacheFactory = new DataCacheFactory();
            cache = cacheFactory.GetDefaultCache();
        }
        #region Cache Operation
        /// <summary>
        /// Security : Device Operation
        /// </summary>
        public void addDeviceAuth(string email, string deviceID)
        {
            string id = email + "_DeviceAuth";
            cache.Add(id, deviceID, TimeSpan.FromMinutes(15));

        }
        public void updateDeviceAuth(string email, string deviceID)
        {
            string id = email + "_DeviceAuth";
            cache.Put(id, deviceID);
        }
        public object getDeviceAuth(string email)
        {
            string id = email + "_DeviceAuth";
            object data = cache.Get(id);
            return data;
        }

        /// <summary>
        /// Security: GUID Operation
        /// </summary>
        public void addguidAuth(string guid)
        {
            cache.Add(guid, "value", TimeSpan.FromDays(1));
        }
        public object getguidAuth(string guid)
        {
            object data = cache.Get(guid);
            return data;
        }

        /// <summary>
        /// Order History Operation
        /// </summary>
        public void addOrderlist(string email, OrderHistory[] historylist)
        {
            string id = email + "_Orderlist";
            cache.Add(id, historylist, TimeSpan.FromMinutes(30));

        }
        public void updateOrderlist(string email, OrderHistory[] historylist)
        {
            string id = email + "_Orderlist";
            cache.Put(id, historylist);
        }

        public bool removeOrderlist(string email)
        {
            string id = email + "_Orderlist";
            return cache.Remove(id);
        }

        public object getOrderlist(string email)
        {
            string id = email + "_Orderlist";
            object data = cache.Get(id);
            return data;
        }

        /// <summary>
        /// AccountInfo Operation
        /// </summary>
        public void addAccountInfo(string email, AccountInfo ainfo)
        {
            string id = email + "_Account";
            cache.Add(id, ainfo, TimeSpan.FromDays(30));
        }

        public void updateAccountInfo(string email, AccountInfo ainfo)
        {
            string id = email + "_Account";
            cache.Put(id, ainfo);
        }

        public object getAccountInfo(string email)
        {
            string id = email + "_Account";
            object data = cache.Get(id);
            return data;

        }

        /// <summary>
        ///  email to PUID operation
        /// </summary>
        public void addPUID(string email, string PUID)
        {
            string id = email + "_PUID";
            cache.Add(id, PUID, TimeSpan.FromDays(30));
        }
        public object getPUID(string email)
        {
            string id = email + "_PUID";
            object data = cache.Get(id);
            return data;
        }
        public bool removePUID(string email)
        {
            string id = email + "_PUID";
            return cache.Remove(id);
        }

        /// <summary>
        ///  PUID to email operation
        /// </summary>
        public void addp2e(string puid, string email)
        {
            string id = puid + "_p2e";
            cache.Add(id, email, TimeSpan.FromDays(30));
        }
        public object getp2e(string puid)
        {
            string id = puid + "_p2e";
            object data = cache.Get(id);
            return data;
        }
        public bool removep2e(string puid)
        {
            string id = puid + "_p2e";
            return cache.Remove(id);
        }

        /// <summary>
        /// PUID to Accountid operation
        /// </summary>
        public void addaccountid(string puid, string account)
        {
            string id = puid + "_account";
            cache.Add(id, account, TimeSpan.FromDays(30));
        }
        public object getaccountid(string puid)
        {
            string id = puid + "_account";
            object data = cache.Get(id);
            return data;
        }
        public bool removeaccountid(string puid)
        {
            string id = puid + "_account";
            return cache.Remove(id);
        }


        /// <summary>
        /// PI Operation
        /// </summary>
        public void addPI(string email, PIType.PaymentInstrument[] PIlist)
        {
            string id = email + "_PI";
            cache.Add(id, PIlist, TimeSpan.FromDays(30));
        }
        public object getPI(string email)
        {
            string id = email + "_PI";
            object data = cache.Get(id);
            return data;
        }
        public bool removePI(string email)
        {
            string id = email + "_PI";
            return cache.Remove(id);
        }

        /// <summary>
        /// 
        /// </summary>


        #endregion


        public DataCache cache;
    }
}