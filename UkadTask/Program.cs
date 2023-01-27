using UkadTask;
using UkadTask.Models;

var urlValidator = new UrlValidator();
var webPage = new WebPage();

Console.Write("Please enter url: ");
string inputedLine = Console.ReadLine();

while (!urlValidator.IsValid(inputedLine))
{
    Console.Write("Inputed line is not valid webadress. Please enter valid url: ");
    inputedLine = Console.ReadLine();
}

webPage.UrlPath = inputedLine;
webPage.WebParse();



if (urlValidator.IsValid($"{webPage.UrlPath}/sitemap.xml"))
{
    webPage.SiteMapPath = $"{webPage.UrlPath}/sitemap.xml";
    webPage.SiteMapParse();

    if (webPage.SiteMapUrls.Count() > 0)
    {
        var mergedUrls = new List<UrlModel>();
        var siteMapYesWebNo = new List<UrlModel>();
        var webYesSiteMapNo = new List<UrlModel>();

        foreach (UrlModel siteMapUrl in webPage.SiteMapUrls)
        {
            mergedUrls.Add(siteMapUrl);
            if (!webPage.WebUrls.Select(x => x.Url).Contains(siteMapUrl.Url))
            {
                siteMapYesWebNo.Add(siteMapUrl);
            }
        }
        foreach (UrlModel webUrl in webPage.WebUrls)
        {
            if (!webPage.SiteMapUrls.Select(x => x.Url).Contains(webUrl.Url))
            {
                mergedUrls.Add(webUrl);
                webYesSiteMapNo.Add(webUrl);
            }
        }

        Print(mergedUrls, "Merged List:");
        Print(siteMapYesWebNo, "Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site:");
        Print(webYesSiteMapNo, "Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml");

    }
}
else 
{
    if (webPage.WebUrls.Count() > 0)
    {
        Print(webPage.WebUrls, "No sitemap.xml found \n WebPage url list:");
    }
}

Console.ReadLine(); 

void Print(List<UrlModel> list, string title)
{
    Console.WriteLine(title);
    foreach (var item in list.OrderBy(x => x.ResponseTime))
    {
        Console.WriteLine($"{item.Url}, Response time(ms): {item.ResponseTime}");
    }
    Console.WriteLine("\n");
}