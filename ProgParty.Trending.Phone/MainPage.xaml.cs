using ProgParty.Core;
using ProgParty.Trending.Api.Parameter;
using ProgParty.Trending.Api.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace ProgParty.Trending
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public TrendingDataContext PageDataContext { get; set; }
        public Pivot Pivot => searchPivot;

        CancellationTokenSource source = new CancellationTokenSource();
        SynchronizationContext context = SynchronizationContext.Current;
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            Config.Instance = new Config(this)
            {
                Pivot = searchPivot,
                AppName = "Trending",
                Ad = new ConfigAd()
                {
                    AdHolder = AdHolder,
                    AdApplicationId = "5c6e8ec0-b7fe-44c1-a47e-2ec91587e338",
                    SmallAdUnitId = "11569959",
                    MediumAdUnitId = "11569958",
                    LargeAdUnitId = "11569957"
                }
            };
#if DEBUG
            Core.Config.Instance.LicenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            Core.Config.Instance.LicenseInformation = CurrentApp.LicenseInformation;
#endif
            Core.License.LicenseInfo.SetLicenseInformation();

            Register.Execute();

            PageDataContext = new TrendingDataContext();
            DataContext = PageDataContext;

            string gallery = GetSavedGalleryType();
            if (string.IsNullOrEmpty(gallery))
                gallery = "All";

            ComboBoxMenu.SelectedIndex = (int)GetByName(gallery);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Register.RegisterOnNavigatedTo(Config.Instance.LicenseInformation);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Register.RegisterOnLoaded();
        }

        private void ContactButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Core.Pages.Contact), null);
        }

        private void BuyBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Core.Pages.Shop));
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private async void ShowGallery(ArticleCategory category)
        {
            PageDataContext.CurrentArticleGallery = category;
            var parameters = new OverviewParameter() { StartOver = true, Category = category, Paging = -1 };
            await Task.Factory.StartNew(() => Searcher.ExecuteGaleryScrape(this, context, parameters), source.Token);
        }

        private async void GalleryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems.FirstOrDefault() as OverviewResult;
            if (selectedItem == null)
                return;

            PageDataContext.SelectedGallery = selectedItem.Index;

            if (PageDataContext.NeedGalleryScrape())
                await Task.Factory.StartNew(() => Searcher.ExecuteGaleryScrape(this, context), source.Token);

            await Task.Factory.StartNew(() => Searcher.ExecuteSingleArticleScrape(this, context, selectedItem), source.Token);
            
        }

        private async void ComboBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = e.AddedItems.First() as ComboBoxItem;
            string name = item.Name;
            SaveGalleryType(name);
            ShowArticleGallery(name);
        }

        private void ShowArticleGallery(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            ArticleCategory articleCategory = GetByName(name);
            ShowGallery(articleCategory);
        }

        private void SaveGalleryType(string name) => new Core.Storage.Storage().StoreInLocal("gallerytype", name);

        private string GetSavedGalleryType() => new Core.Storage.Storage().LoadFromLocal("gallerytype")?.ToString() ?? "";

        private ArticleCategory GetByName(string name)
        {
            switch (name)
            {
                case "All": return ArticleCategory.All;
                case "Celebs": return ArticleCategory.Celebs;
                case "Food": return ArticleCategory.Food;
                case "Fun": return ArticleCategory.Fun;
                case "FYI": return ArticleCategory.FYI;
                case "Internet": return ArticleCategory.Internet;
                case "Lifestyle": return ArticleCategory.Lifestyle;
                case "Media": return ArticleCategory.Media;
                case "Muziek": return ArticleCategory.Muziek;
                case "Nieuws": return ArticleCategory.Nieuws;
                case "Opmerkelijk": return ArticleCategory.Opmerkelijk;
                case "Quiz": return ArticleCategory.Quiz;
                case "Sport": return ArticleCategory.Sport;
                case "WTF": return ArticleCategory.WTF;
                default:
                    throw new System.NotImplementedException("This type is not implemented " + name);
            }
        }

        private async void LoadMoreGalleries_Click(object sender, RoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => Searcher.ExecuteGaleryScrape(this, context), source.Token);
        }

        private async void LoadPreviousGallery_Click(object sender, RoutedEventArgs e)
        {
            if (PageDataContext.NeedGalleryScrape())
                await Task.Factory.StartNew(() => Searcher.ExecuteGaleryScrape(this, context), source.Token);

            var gallery = PageDataContext.GetNextGallery();

            if (gallery == null)
            {
                //should not be possible, because gallery's are loaded async before the last gallery is reached.
                new MessageDialog("Geen gallerij aanwezig, laad er meer.").ShowAsync();
                Pivot.SelectedIndex = 0;
            }
            else
            {
                await Task.Factory.StartNew(() => Searcher.ExecuteSingleArticleScrape(this, context, gallery), source.Token);
            }
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            var share = new Core.Share.ShareUrl();

            share.RegisterForShare(sender as MenuFlyoutItem, "http://www.trending.nl" + ((sender as MenuFlyoutItem).DataContext as OverviewResult).Url);
        }
    }
}
