// ----------------------------------------------------------------------- 
// <copyright file="CertificateHelper.cs" company="Microsoft"> 
//      Copyright (c) Microsoft Corporation. All rights reserved. 
// </copyright> 
// ----------------------------------------------------------------------- 

namespace MS.Support.CMAT.Test.Framework.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    public class CertificateHelper
    {
        public static X509Certificate2 GetCertByThumbprint(string thumbprint)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            try
            {
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);
                X509Certificate2Collection collection = store.Certificates;
                if (collection != null && collection.Count != 0)
                {
                    for (int i = 0; i < collection.Count; i++)
                    {
                        if (collection[i].Thumbprint.Equals(thumbprint, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return collection[i];
                        }
                    }
                }

                return null;
            }
            finally
            {
                store.Close();
            }
        }
    }
}
