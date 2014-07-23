using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace MintRestApi
{
    public class WebRole : BaseRole
    {
        public override bool OnStart()
        {
            return base.OnStart();
        }
    }
}
