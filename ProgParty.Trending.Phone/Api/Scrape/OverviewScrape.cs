using HtmlAgilityPack;
using ProgParty.Trending.Api.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ProgParty.Trending.Api.Scrape
{
    internal class OverviewScrape
    {
        private Parameter.OverviewParameter Parameters { get; set; }

        public OverviewScrape(Parameter.OverviewParameter parameters)
        {
            Parameters = parameters;
        }

        public List<OverviewResult> Execute()
        {
            string url = ConstructUrl();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Host = "www.trending.nl";
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.125 Safari/537.36");
                //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                
                return ConvertToResult(result);
            }
        }

        public string ConstructUrl()
        {
            var pageNr = Parameters.Paging * 12;

            if (Parameters.Category == Parameter.ArticleCategory.All)
            {
                if (pageNr == 0)
                    return $"http://www.trending.nl/artikelen";
                return $"http://www.trending.nl/artikelen/P{pageNr}";
            }

            if(pageNr == 0)
                return $"http://www.trending.nl/artikelen{GetBaseUrl()}";
            return $"http://www.trending.nl/artikelen{GetBaseUrl()}&offset=0/P{pageNr}";
        }

        private string GetBaseUrl()
        {
            switch (Parameters.Category)
            {
                case Parameter.ArticleCategory.All: return "";
                case Parameter.ArticleCategory.Celebs: return "/search&category=celebs";
                case Parameter.ArticleCategory.Food: return "/search&category=food";
                case Parameter.ArticleCategory.Fun: return "/search&category=fun";
                case Parameter.ArticleCategory.FYI: return "/search&category=fyi";
                case Parameter.ArticleCategory.Internet: return "/search&category=internet";
                case Parameter.ArticleCategory.Lifestyle: return "/search&category=lifestyle";
                case Parameter.ArticleCategory.Media: return "/search&category=media";
                case Parameter.ArticleCategory.Muziek: return "/search&category=muziek";
                case Parameter.ArticleCategory.Nieuws: return "/search&category=nieuws";
                case Parameter.ArticleCategory.Opmerkelijk: return "/search&category=opmerkelijk";
                case Parameter.ArticleCategory.Quiz: return "/search&category=quiz";
                case Parameter.ArticleCategory.Sport: return "/search&category=sport";
                case Parameter.ArticleCategory.WTF: return "/search&category=wtf";
                default:
                    throw new System.Exception("Not implemented the parameter: " + Parameters.Category);
            }
        }

        public List<OverviewResult> ConvertToResult(string result)
        {
            List<OverviewResult> overviewResult = new List<OverviewResult>();

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(result);
            var node = htmlDocument.DocumentNode;

            var sectionNode = node.Descendants("ul").SingleOrDefault(c => c.Attributes["id"]?.Value?.Contains("entriesBlocks") ?? false);
            if (sectionNode == null)
                return overviewResult;

            var posts = node.Descendants("div").Where(c => c.Attributes["class"]?.Value?.Contains("entryContainer") ?? false);

            foreach (var post in posts)
            {
                overviewResult.Add(ConvertSingleResult(post));
            }

            return overviewResult;
        }

        public OverviewResult ConvertSingleResult(HtmlNode node)
        {
            var o = new OverviewResult()
            {
                Title = node.Descendants("h5").FirstOrDefault()?.InnerText ?? "",
                Type = node.Descendants("h6").FirstOrDefault()?.InnerText ?? "",
                ImageUrl = node.Descendants("img").FirstOrDefault(c => c.Attributes["class"]?.Value?.Contains("hvr-grow") ?? false)?.Attributes["src"]?.Value ?? string.Empty,
                Url = node.Descendants("a").FirstOrDefault(c => c.Attributes["class"]?.Value == "entry")?.Attributes["href"]?.Value ?? string.Empty
            };

            o.Title = System.Net.WebUtility.HtmlDecode(o.Title);
            o.Type = System.Net.WebUtility.HtmlDecode(o.Type);
            return o;
        }
    }
}
