using System;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ProgParty.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ProgParty.Core.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shop : Page
    {
        public Shop()
        {
            this.InitializeComponent();
            SetRemoveAdvertisements();
            SetDonations();
        }
        
        private async void BuyRemoveAds_Click(object sender, RoutedEventArgs e)
        {
            if (!Config.Instance.LicenseInformation.ProductLicenses[InAppPurchase.TokenRemoveAdvertisement].IsActive)
            {
                try
                {
                // show the purchase dialog.
#if DEBUG
                await CurrentAppSimulator.RequestProductPurchaseAsync(InAppPurchase.TokenRemoveAdvertisement);
#else
                await CurrentApp.RequestProductPurchaseAsync(InAppPurchase.TokenRemoveAdvertisement);
#endif
                SetRemoveAdvertisements();
            }
            catch (Exception)
                {
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
            else
            {
                SetRemoveAdvertisements();
            }
        }

        private void SetRemoveAdvertisements()
        {
            if (Config.Instance.LicenseInformation.ProductLicenses[InAppPurchase.TokenRemoveAdvertisement].IsActive)
            {
                RemoveAdsButton.Visibility = Visibility.Collapsed;
                RemoveAdsPrice.Visibility = Visibility.Collapsed;
                RemoveAdsBought.Visibility = Visibility.Visible;
            }
            else
            {
                RemoveAdsBought.Visibility = Visibility.Collapsed;
            }
        }
                
        private async void Donation_Click(object sender, RoutedEventArgs e)
        {
            if (!Config.Instance.LicenseInformation.ProductLicenses[InAppPurchase.TokenDonation].IsActive)
            {
                try
                {
                    // show the purchase dialog.
#if DEBUG
                    await CurrentAppSimulator.RequestProductPurchaseAsync(InAppPurchase.TokenDonation);
#else
                    await CurrentApp.RequestProductPurchaseAsync(InAppPurchase.TokenDonation);
#endif
                    SetDonations();
                }
                catch (Exception)
                {
                    // The in-app purchase was not completed because 
                    // an error occurred.
                }
            }
            else
            {
                SetDonations();
            }
        }

        private void SetDonations()
        {
            if (Config.Instance.LicenseInformation.ProductLicenses[InAppPurchase.TokenDonation].IsActive)
            {
                DonationButton.Visibility = Visibility.Collapsed;
                DonationPrice.Visibility = Visibility.Collapsed;
                DonationBought.Visibility = Visibility.Visible;
            }
            else
            {
                DonationBought.Visibility = Visibility.Collapsed;
            }
        }
    }
}
