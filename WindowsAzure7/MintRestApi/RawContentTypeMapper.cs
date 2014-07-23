using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace MintRestApi
{
    public class RawContentTypeMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {

            if (contentType.Contains("text/xml") || contentType.Contains("application/json"))
            {

                return WebContentFormat.Raw;

            }

            else
            {

                return WebContentFormat.Raw;

            }
        }
    }
}