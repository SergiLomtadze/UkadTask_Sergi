using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UkadTask
{
    public class UrlValidator
    {
        public bool IsValid(string url)
        {
            using (var client = new WebClient())
            {
                try
                {
                    using (client.OpenRead(url))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }            
        }
    }
}
