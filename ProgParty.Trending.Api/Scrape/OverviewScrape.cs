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
                client.DefaultRequestHeaders.Host = "www.boredpanda.com";
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.125 Safari/537.36");
                //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                var response = client.GetAsync(url).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                
                return ConvertToResult(result);
            }
        }

        public string ConstructUrl() => $"http://www.boredpanda.com{GetBaseUrl()}/page/{Parameters.Paging}";

        private string GetBaseUrl()
        {
            switch (Parameters.Category)
            {
                case Parameter.ArticleCategory.Advertising: return "/category/advertising";
                case Parameter.ArticleCategory.All: return "";
                case Parameter.ArticleCategory.Animals: return "/category/animals";
                case Parameter.ArticleCategory.Architecture: return "/category/architecture";
                case Parameter.ArticleCategory.Art: return "/category/art";
                case Parameter.ArticleCategory.Automotive: return "/category/automotive";
                case Parameter.ArticleCategory.BodyArt: return "/category/body-art-art";
                case Parameter.ArticleCategory.Comics: return "/category/comics";
                case Parameter.ArticleCategory.DigitalArt: return "/category/digital-art-art";
                case Parameter.ArticleCategory.DIY: return "/category/diy";
                case Parameter.ArticleCategory.Drawing: return "/category/drawing";
                case Parameter.ArticleCategory.Entertainment: return "/category/entertainment";
                case Parameter.ArticleCategory.FoodArt: return "/category/food";
                case Parameter.ArticleCategory.Funny: return "/category/funny";
                case Parameter.ArticleCategory.FurnitureDesign: return "/category/furniture-design";
                case Parameter.ArticleCategory.GoodNews: return "/category/good-news";
                case Parameter.ArticleCategory.GraphicDesign: return "/category/graphic-design";
                case Parameter.ArticleCategory.History: return "/category/history";
                case Parameter.ArticleCategory.Home: return "/category/home";
                case Parameter.ArticleCategory.Illustration: return "/category/illustration";
                case Parameter.ArticleCategory.Installation: return "/category/installation-art";
                case Parameter.ArticleCategory.InteriorDesign: return "/category/interior-design";
                case Parameter.ArticleCategory.LandArt: return "/category/land-art";
                case Parameter.ArticleCategory.Nature: return "/category/nature";
                case Parameter.ArticleCategory.NeedleAndThread: return "/category/needle-thread";
                case Parameter.ArticleCategory.OpticalIllusions: return "/category/optical-illusions";
                case Parameter.ArticleCategory.Other: return "/category/other";
                case Parameter.ArticleCategory.Packaging: return "/category/packaging";
                case Parameter.ArticleCategory.Painting: return "/category/painting";
                case Parameter.ArticleCategory.PaperArt: return "/category/paper-art-art";
                case Parameter.ArticleCategory.Parenting: return "/category/parenting";
                case Parameter.ArticleCategory.Photography: return "/category/photography";
                case Parameter.ArticleCategory.Pics: return "/category/pics";
                case Parameter.ArticleCategory.ProductDesign: return "/category/industrial-design";
                case Parameter.ArticleCategory.Recycling: return "/category/recycling-art";
                case Parameter.ArticleCategory.Science: return "/category/science";
                case Parameter.ArticleCategory.Sculpting: return "/category/sculpting";
                case Parameter.ArticleCategory.SocialIssues: return "/category/social-issues";
                case Parameter.ArticleCategory.StreetArt: return "/category/street-art";
                case Parameter.ArticleCategory.Style: return "/category/style";
                case Parameter.ArticleCategory.Technology: return "/category/technology";
                case Parameter.ArticleCategory.Travel: return "/category/travel";
                case Parameter.ArticleCategory.Typography: return "/category/typography";
                case Parameter.ArticleCategory.Video: return "/category/video";
                case Parameter.ArticleCategory.Weird: return "/category/weird";
                    
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

            var sectionNode = node.Descendants("section").SingleOrDefault(c => c.Attributes["class"]?.Value?.Contains("posts") ?? false);
            if (sectionNode == null)
                return overviewResult;

            var posts = sectionNode.Descendants("article").Where(c => c.Attributes["class"]?.Value?.Contains("post") ?? false);

            foreach (var post in posts)
            {
                overviewResult.Add(ConvertSingleResult(post));
            }

            return overviewResult;
        }

        public OverviewResult ConvertSingleResult(HtmlNode node)
        {
            var aDiv = node.Descendants("div").FirstOrDefault(c => c.Attributes["class"]?.Value == "post-cover-container").Descendants("a").FirstOrDefault();

            var o = new OverviewResult()
            {
                Title = (node.Descendants("h2").FirstOrDefault()?.Descendants("a")?.FirstOrDefault()?.InnerText ?? "Title not found").Trim(),
                SubTitle = node.Descendants("p")?.FirstOrDefault(c => c.Attributes["class"]?.Value == "description")?.InnerText ?? "Sub title not found",
                ImageUrl = aDiv?.Descendants("img")?.FirstOrDefault()?.Attributes["src"]?.Value ?? string.Empty,
                Url = aDiv?.Attributes["href"]?.Value ?? string.Empty
            };

            //Uri temp;
            //if(Uri.TryCreate(aDiv?.Attributes["href"]?.Value ?? string.Empty, UriKind.Absolute, out temp))
            //    o.Url = temp;
            //else
            //    o.Url = new Uri("");
            o.Title = System.Net.WebUtility.HtmlDecode(o.Title);
            o.SubTitle = System.Net.WebUtility.HtmlDecode(o.SubTitle);
            return o;
        }
    }
}
