/*
 For every output parameter, we make a single call to this class.
 This class encapsulates the job of creating an Xml as an output node
 */
using System;
using System.Xml;
using System.Reflection;
using System.Web.Services.Protocols;

namespace Microsoft.CTP.SDK.Client
{
    /// <summary>
    /// Summary description for Input.
    /// </summary>
    public class XmlOutput
    {
        #region State
        ParameterInfo mParameterInfo;
        Object mParameterValue;
        XmlDocument mXmlOutput;

        #endregion

        #region Constructor
        public XmlOutput(XmlDocument xmlOutput, ParameterInfo parameterInfo, Object parameterValue)
        {
            mXmlOutput = xmlOutput;
            CreateResultRoot(mXmlOutput);
            mParameterInfo = parameterInfo;
            mParameterValue = parameterValue;
        }
        public XmlOutput()
        {
        }
        #endregion constructor

        #region Public Properties
        public ParameterInfo Parameter
        {
            get
            {
                return mParameterInfo;
            }
        }


        #endregion

        #region PrivateMethods
        public XmlElement CreateResultRoot(XmlDocument xmlDocument)
        {
            XmlElement xmlElement = null;
            if (xmlDocument.DocumentElement == null)
            {
                xmlElement = xmlDocument.CreateElement("Result");
                xmlDocument.AppendChild(xmlElement);
            }
            return xmlElement;
        }


        #endregion

        #region public methods

        public XmlElement AddNode(XmlElement parentElement, string nodeName, string nodeValue)
        {
            XmlElement element = mXmlOutput.CreateElement(nodeName);
            element.InnerText = nodeValue;
            if (parentElement == null)
            {
                mXmlOutput.DocumentElement.AppendChild(element);
            }
            else
            {
                parentElement.AppendChild(element);
            }

            return element;
        }

        public XmlElement AddException(Exception exception)
        {
            string exceptionName = "Exception";
            string xPathException = "/" + exceptionName;
            XmlElement exceptionElement = null;
            XmlNode exceptionNode = mXmlOutput.DocumentElement.SelectSingleNode(xPathException);
            if (exceptionNode == null)
            {
                exceptionElement = mXmlOutput.CreateElement(exceptionName);
                mXmlOutput.DocumentElement.AppendChild(exceptionElement);
            }
            if (exception.InnerException != null)
                exceptionElement.InnerText = "<FaultMessage>" + exception.InnerException.Message + "</FaultMessage>";
            else
                exceptionElement.InnerText = "<FaultMessage>" + exception.Message + "</FaultMessage>";
            Type type = exception.GetType();
            if (exception.InnerException != null)
            {
                SoapException Se = exception.InnerException as SoapException;
                if (Se != null)
                    exceptionElement.InnerText += Se.Detail.InnerXml;
            }
            exceptionElement.InnerText = "<ExceptionInfo>" + exceptionElement.InnerText + "</ExceptionInfo>";
            return exceptionElement;
        }
        private string FindXml(string str)
        {
            int Start = str.IndexOf("<");
            int End = str.LastIndexOf(">");
            if (Start >= 0 && End >= 0 && End > Start)
                return (str.Substring(Start, End - Start + 1));
            else
                return (String.Empty);
        }
        public XmlElement AddParameterNode(string nodeName, string nodeValue)
        {
            XmlElement element = AddNode(null, nodeName, nodeValue);
            return element;
        }
        #endregion public methods

    }
}




