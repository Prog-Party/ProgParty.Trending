using ProgParty.Trending.Api.Result;
using ProgParty.Trending.Api.Scrape;
using System;

namespace ProgParty.Trending.Api.Execute
{
    public class ArticleExecute
    {
        public Parameter.ArticleParameter Parameters = new Parameter.ArticleParameter();

        public ArticleResult Result;

        public bool Execute()
        {
            try
            {
                Result = new ArticleScrape(Parameters).Execute();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}
