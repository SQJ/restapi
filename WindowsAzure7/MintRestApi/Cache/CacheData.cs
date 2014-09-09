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
        /// 
        /// </summary>
        public void addDeviceAuth(string email,string deviceID)
        {
            string id = email + "_DeviceAuth";
            cache.Add(id, deviceID,TimeSpan.FromMinutes(15));

        }
        public void updateDeviceAuth(string email,string deviceID)
        {
            string id = email + "_DeviceAuth";
            cache.Put(id,deviceID);
        }
        public object getDeviceAuth(string email)
        {
            string id = email + "_DeviceAuth";
            object data = cache.Get(id);
            return data;
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void addHistory(string email, HistoryItem[] historylist)
        {
            string id = email + "_History";
            cache.Add(id, historylist, TimeSpan.FromMinutes(30));

        }
        public void updateHistory(string email, HistoryItem[] historylist)
        {
            string id = email + "_History";
            cache.Put(id, historylist);
        }
        public object getHistory(string email)
        {
            string id = email + "_History";
            object data = cache.Get(id);
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void addAccountInfo(string email, AccountInfo ainfo)
        {
            string id = email + "_Account";
            cache.Add(id, ainfo, TimeSpan.FromDays(30));
        }

        public void updateAccountInfo(string email, HistoryItem[] historylist)
        {
            string id = email + "_Account";
            cache.Put(id, historylist);
        }
        public object getAccountInfo(string email)
        {
            string id = email + "_Account";
            object data = cache.Get(id);
            return data;
        }

        /// <summary>
        /// 
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