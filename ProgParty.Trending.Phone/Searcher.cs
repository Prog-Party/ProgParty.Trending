using ProgParty.Trending.Api.Execute;
using ProgParty.Trending.Api.Result;
using System.Threading;

namespace ProgParty.Trending
{
    public class Searcher
    {
        public static void ExecuteSingleArticleScrape(MainPage mainpage, SynchronizationContext theContext, OverviewResult search)
        {
            theContext.Post((_) =>
            {
                mainpage.Pivot.SelectedIndex = 1;
                mainpage.PageDataContext.ArticleLoading = true;
                mainpage.PageDataContext.Articles.Clear();
            }, null);

            ArticleExecute execute = new ArticleExecute();
            execute.Parameters.Url = "http://www.trending.nl/" + search.Url;
            execute.Parameters.Type = mainpage.PageDataContext.CurrentArticleGallery;
            execute.Execute();
            var article = execute.Result;

            theContext.Post((_) =>
            {
                mainpage.PageDataContext.InitializeNewArticle(article);
            }, null);
        }

        public static void ExecuteGaleryScrape(MainPage mainpage, SynchronizationContext context, Api.Parameter.OverviewParameter parameters = null)
        {
            if (parameters == null)
                parameters = new Api.Parameter.OverviewParameter();

            context.Post((_) =>
            {
                mainpage.PageDataContext.GalleryItemsLoading = true;

                if (parameters.StartOver)
                    mainpage.PageDataContext.Gallery.Clear();
            }, null);

            parameters.Category = mainpage.PageDataContext.CurrentArticleGallery;

            if (parameters.StartOver)
            {
                mainpage.PageDataContext.GalleryPaging = -1;
                mainpage.PageDataContext.GalleryItemIndex = 0;
                mainpage.PageDataContext.SelectedGallery = 0;
            }

            mainpage.PageDataContext.GalleryPaging += 1;
            parameters.Paging = mainpage.PageDataContext.GalleryPaging;

            OverviewExecute oe = new OverviewExecute();
            oe.Parameters = parameters;
            oe.Execute();
            var result = oe.Result;


            context.Post((_) =>
            {
                if (parameters.StartOver)
                {
                    mainpage.PageDataContext.Gallery.Clear();
                }

                foreach (var item in result)
                {
                    item.Index = mainpage.PageDataContext.GalleryItemIndex;
                    mainpage.PageDataContext.Gallery.Add(item);
                    mainpage.PageDataContext.GalleryItemIndex++;
                }

                mainpage.PageDataContext.GalleryItemsLoading = false;
            }, null);
        }
    }
}
