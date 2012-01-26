using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Web.Security;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Core;

namespace Fab.Managment
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private AdminServiceClient adminService;
		private int currentPageIndex = 0;
		private int pagesCount;

		public MainWindow()
		{
			InitializeComponent();

			Upto.SelectedDate = DateTime.Now;
			NotOlderThen.SelectedDate = DateTime.Now.AddMonths(-1);

			var times = new List<DateTime>();

			var time = DateTime.Now.Date;
			var maxTime = time.AddDays(1);

			while (time < maxTime)
			{
				times.Add(time);
				time = time.AddMinutes(30);
			}

			var currentTime = DateTime.Now;

			var currentTimeIndex = 0;

			for (int i = 0; i < times.Count; i++)
			{
				if (times[i] > currentTime)
				{
					currentTimeIndex = i;
					break;
				}
			}

			UptoTime.ItemsSource = times;
			NotOlderThenTime.ItemsSource = times;

			UptoTime.SelectedIndex = currentTimeIndex;
			NotOlderThenTime.SelectedIndex = currentTimeIndex;

			// Important to accept SSL certificate from server before making any call to WCF services
			SslValidation.SetCertificatePolicy();

			Helpers.EndPointAddress = endPointAddress.Text;
			Helpers.Username = Username.Text;
			Helpers.Password = Password.Password;
		}

		private TextSearchFilter CreateFilter(int? pageSize = null)
		{
			var filter = new TextSearchFilter
			             	{
			             		NotOlderThen = IsNotOlderThen.IsChecked.HasValue && IsNotOlderThen.IsChecked.Value
			             		               	? (NotOlderThen.SelectedDate.Value +
			             		               	   ((DateTime) NotOlderThenTime.SelectedValue).TimeOfDay).ToUniversalTime()
			             		               	: (DateTime?) null,
			             		Upto = IsUpto.IsChecked.HasValue && IsUpto.IsChecked.Value
			             		       	? (Upto.SelectedDate.Value + ((DateTime) UptoTime.SelectedValue).TimeOfDay).ToUniversalTime()
			             		       	: (DateTime?) null,
			             		Contains = !string.IsNullOrEmpty(SearchText.Text)
			             		           	? SearchText.Text
			             		           	: null,
			             		Take = pageSize,
								Skip = pageSize != null ? (currentPageIndex - 1) * pageSize : null,
			             	};
			return filter;
		}

		private int PageSize
		{
			get { return int.Parse(take.SelectedValue.ToString()); }
		}

		private void OnLoadClick(object sender, RoutedEventArgs e)
		{
			search.IsEnabled = false;
			search.Content = "Loadig...";

			// Reset curent page index
			currentPageIndex = 1;

			adminService = Helpers.CreateClientProxy(endPointAddress.Text, Username.Text, Password.Password);

			adminService.GetUsersCountCompleted += OnGetUsersCountCompleted;
			adminService.GetUsersCountAsync(CreateFilter());

			adminService.GetUsersCompleted += GetLoadUserCompleted;
			adminService.GetUsersAsync(CreateFilter(PageSize));
		}

		private void OnGetUsersCountCompleted(object sender, GetUsersCountCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				totalUsersText.Text = args.Result.ToString(CultureInfo.InvariantCulture);
				pagesCount = args.Result / PageSize + args.Result % PageSize > 0 ? 1 : 0;
				pagesCountText.Text = pagesCount.ToString(CultureInfo.InvariantCulture);
				currentPageIndex = pagesCount > 0 ? 1 : 0;
				currentPageText.Text = currentPageIndex.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				Helpers.ErrorProcessing(args);
			}
		}

		private void GetLoadUserCompleted(object sender, GetUsersCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				adminService.Close();
				usersList.ItemsSource = args.Result;
			}
			else
			{
				Helpers.ErrorProcessing(args);
			}

			search.Content = "Load";
			search.IsEnabled = true;
		}

		private void OnClearClick(object sender, RoutedEventArgs e)
		{
			SearchText.Text = string.Empty;
			usersList.ItemsSource = null;
		}

		private void OnNextPageClick(object sender, RoutedEventArgs e)
		{
			currentPageIndex++;
			currentPageText.Text = currentPageIndex.ToString(CultureInfo.InvariantCulture);

			search.IsEnabled = false;
			search.Content = "Loadig...";

			var filter = CreateFilter(PageSize);

			adminService = Helpers.CreateClientProxy(endPointAddress.Text, Username.Text, Password.Password);

			adminService.GetUsersCompleted += GetLoadUserCompleted;
			adminService.GetUsersAsync(filter);
		}

		private void OnPrevPageClick(object sender, RoutedEventArgs e)
		{
			currentPageIndex--;
			currentPageText.Text = currentPageIndex.ToString(CultureInfo.InvariantCulture);

			search.IsEnabled = false;
			search.Content = "Loadig...";

			var filter = CreateFilter(PageSize);

			adminService = Helpers.CreateClientProxy(endPointAddress.Text, Username.Text, Password.Password);

			adminService.GetUsersCompleted += GetLoadUserCompleted;
			adminService.GetUsersAsync(filter);
		}

		#region Misc

		private void OnGenerateClick(object sender, RoutedEventArgs e)
		{
			PasswordHash.Text =
				FormsAuthentication.HashPasswordForStoringInConfigFile(NewPassword.Text, FormsAuthPasswordFormat.SHA1.ToString());
		}

		private void OnServiceAddressChanged(object sender, SelectionChangedEventArgs e)
		{
			Helpers.EndPointAddress = ((ComboBoxItem)endPointAddress.SelectedValue).Content.ToString();
		}

		private void OnUsernameTextChanged(object sender, TextChangedEventArgs e)
		{
			Helpers.Username = Username.Text;
		}

		private void OnPasswordPasswordChanged(object sender, RoutedEventArgs e)
		{
			Helpers.Password = Password.Password;
		}

		#endregion
	}
}
