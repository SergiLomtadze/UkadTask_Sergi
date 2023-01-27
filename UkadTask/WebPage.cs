using HtmlAgilityPack;
using System.Diagnostics;
using System.Net;
using System.Xml;
using UkadTask.Models;

namespace UkadTask
{
    public class WebPage
    {
        public string UrlPath { get; set; }
        public string SiteMapPath { get; set; }
        public List<UrlModel> WebUrls { get; set; }
        public List<UrlModel> SiteMapUrls { get; set; }

        private UrlValidator _urlValidator;
        public WebPage()
        {
            SiteMapUrls = new List<UrlModel>();
            WebUrls = new List<UrlModel>();
            _urlValidator = new UrlValidator();
        }

        public void WebParse()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(UrlPath);

            HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

            foreach (HtmlNode node in htmlNodes)
            {
                if (node.OuterHtml.Contains("/"))
                {
                    var url = node.Attributes["href"].Value.TrimEnd('/');
                    WebUrls.Add(new UrlModel
                    {
                        Url = url,
                        ResponseTime = GetResponseTime(url)
                    });
                }
            }
        }

        public void SiteMapParse()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(SiteMapPath);
            foreach (XmlNode xmlNode in doc.DocumentElement)
            {
                var url = xmlNode.FirstChild.InnerXml;
                SiteMapUrls.Add(new UrlModel
                {
                    Url = url,
                    ResponseTime = GetResponseTime(url)
                });
            }
        }

        private int GetResponseTime(string url)
        {
            if (_urlValidator.IsValid(url))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                Stopwatch timer = new Stopwatch();

                timer.Start();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();

                timer.Stop();

                return timer.Elapsed.Milliseconds;
            }
            return 0;
        }

    }
}
