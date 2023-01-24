using System.Diagnostics;
using System.Xml;
using System;
using UkadTask;
using HtmlAgilityPack;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Reflection;

//Console.Write("Please enter url address: ");
//string urlName = Console.ReadLine();
//string urlName = "https://www.ambebi.ge/";
string urlName = "https://www.github.com/";


Stopwatch stopwatch = new Stopwatch();
List<Url> webList = new List<Url>();

XmlDocument xmlDoc = new XmlDocument();
List<Url> xmlList = new List<Url>();

//Load relative path
var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"Sitemap.xml");
xmlDoc.Load(fileName);

XmlNodeList xmlNodes = xmlDoc.SelectNodes("/root/url");

foreach (XmlNode node in xmlNodes)
{
    stopwatch.Start();
    if (node.InnerText.Contains("/"))
    {
        stopwatch.Start();
        xmlList.Add(new Url { UrlName = node.InnerText.Replace("\n", "").Trim(), ElapsedTime = stopwatch.ElapsedTicks });
    }    
}

HtmlWeb web = new HtmlWeb();
HtmlDocument htmlDoc = web.Load(urlName);

//Use the SelectNodes method to find all the "a" elements in the document:
HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

//Iterate through the nodes and check the "href" attribute of each node to see if it starts with "http"
foreach (HtmlNode node in htmlNodes)
{
    stopwatch.Start();
    //if (node.Attributes["href"].Value.Contains("http"))
    if (node.OuterHtml.Contains("/"))
    {
        stopwatch.Stop();
        webList.Add(new Url { UrlName = node.Attributes["href"].Value.TrimEnd('/'), ElapsedTime = stopwatch.ElapsedTicks });
    }
}

var mergedUrls = new List<Url>();
var xmlExistWebNot = new List<Url>();
var webExistXmlNot = new List<Url>();

foreach (Url xmlUrl in xmlList)
{
    mergedUrls.Add(xmlUrl);
    if (!webList.Select(x => x.UrlName).Contains(xmlUrl.UrlName))
    {
        xmlExistWebNot.Add(xmlUrl);
    }
}
foreach (Url webUrl in webList)
{
    if (!xmlList.Select(x => x.UrlName).Contains(webUrl.UrlName))
    {
        mergedUrls.Add(webUrl);
    }
    if (!xmlList.Select(x => x.UrlName).Contains(webUrl.UrlName))
    {
        webExistXmlNot.Add(webUrl);
    }
}

Console.WriteLine("Merged List:");
foreach (Url url in mergedUrls.OrderBy(x => x.ElapsedTime))
{
    Console.WriteLine($"{url.UrlName} {url.ElapsedTime}");
}
Console.WriteLine("\n");

Console.WriteLine("Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site:");
foreach (var url in xmlExistWebNot.OrderBy(x => x.ElapsedTime))
{
    Console.WriteLine($"{url.UrlName} {url.ElapsedTime}");
}
Console.WriteLine("\n");

Console.WriteLine("Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");
foreach (var url in webExistXmlNot.OrderBy(x => x.ElapsedTime))
{
    Console.WriteLine($"{url.UrlName} {url.ElapsedTime}");
}
Console.WriteLine("\n");

Console.WriteLine($"Urls(html documents) found after crawling a website: {webList.Count} \n");
Console.WriteLine($"Urls found in sitemap: {webList.Count} \n");
Console.ReadLine();