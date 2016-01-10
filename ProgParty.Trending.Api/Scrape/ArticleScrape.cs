using HtmlAgilityPack;
using ProgParty.Trending.Api.Result;
using ProgParty.Core.Extension;
using System.Linq;
using System.Net.Http;

namespace ProgParty.Trending.Api.Scrape
{
    internal class ArticleScrape
    {
        private Parameter.ArticleParameter Parameters { get; set; }

        public ArticleScrape(Parameter.ArticleParameter parameters)
        {
            Parameters = parameters;
        }

        public ArticleResult Execute()
        {
            string url = Parameters.Url;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Host = "www.boredpanda.com";
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.125 Safari/537.36");
                //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadAsStringAsync().Result;

                return ConvertToResult(result);
            }
        }

        public ArticleResult ConvertToResult(string result)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(result);
            var node = htmlDocument.DocumentNode;

            ArticleResult a = new ArticleResult()
            {
                Title = (node.Descendants("h1").FirstOrDefault()?.InnerText ?? string.Empty).Trim(),
                ViewsCount = (node.Descendants("p").FirstOrDefault(c => c.Attributes["class"]?.Value == "views-count")?.InnerText ?? string.Empty).Trim(),
                AuthorTime = (node.Descendants("p").FirstOrDefault(c => c.Attributes["class"]?.Value == "author-time")?.InnerText ?? string.Empty).Trim(),
                Content = (node.Descendants("div").FirstOrDefault(c => c.Attributes["class"]?.Value == "post-content")?.InnerHtml ?? string.Empty).Trim()
            };

            a.Title = System.Net.WebUtility.HtmlDecode(a.Title);
            a.AuthorTime = a.AuthorTime.Replace("\n", string.Empty).RemoveDoubleSpaces();

            return a;
        }
    }
}
