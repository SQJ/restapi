using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MintRestApi
{
    public class URLMessage
    {
        private HashSet<string> URLguidset;
        private Queue<string> URLguidqueue;
        //Recor max url
        private int maxsize;

        public URLMessage()
        {
            URLguidset = new HashSet<string>();
            URLguidqueue = new Queue<string>();
            maxsize = 10000;
        }

        private void maintainMessages()
        {
            if (URLguidset.Count() > maxsize * 0.9)
            {
                while (URLguidset.Count() > maxsize * 0.5)
                {
                    string guid = URLguidqueue.Dequeue();
                    URLguidset.Remove(guid);
                }
            }
        }

        public bool verifyguid(string guid)
        {
            bool flag = URLguidset.Contains("11");
            if (URLguidset.Contains(guid)==true) 
                return false;
            else
            {
                URLguidset.Add(guid);
                URLguidqueue.Enqueue(guid);
                maintainMessages();
                return true;
            }
        }

    }
}