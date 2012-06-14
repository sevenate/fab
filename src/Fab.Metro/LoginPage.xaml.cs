using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Fab.Metro.Data;
using Fab.Metro.MoneyServiceReference;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Fab.Metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Change development service url to the relative to host from where the xap was loaded.
        /// </summary>
        /// <param name="serviceEndpoint">Original service endpoint whose address should be updated.</param>
        private static void SetupServiceUri(ServiceEndpoint serviceEndpoint, string url)
        {
#if DEBUG
			var serviceName = serviceEndpoint.Address.Uri.LocalPath.Split('/').Last();
// ReSharper disable AssignNullToNotNullAttribute
// ReSharper restore AssignNullToNotNullAttribute
            serviceEndpoint.Address = new EndpointAddress(url/*@"https://orion/StagingFab/MoneyService.svc"*/);
#endif
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var serviceUrl = ServiceUrl.Text;
            var service = new MoneyServiceClient();
            service.ClientCredentials.UserName.UserName = Username.Text;
            service.ClientCredentials.UserName.Password = Password.Text;

            SetupServiceUri(service.Endpoint, serviceUrl);

            var accountsResult = await service.GetAllAccountsAsync();

            Frame.Navigate(typeof (GroupedItemsPage), accountsResult);
/*
                new SampleDataGroup(
                "AllGroups",
                "Group Title",
                accountsResult.First().Name,
                string.Empty,
                accountsResult.First().Balance.ToString()));
*/
        }
    }
}
