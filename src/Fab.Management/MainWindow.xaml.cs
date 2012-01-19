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

		#region WCF calls

		private void OnSearchClick(object sender, RoutedEventArgs e)
		{
			Search.IsEnabled = false;
			Search.Content = "Searching...";

			adminService = Helpers.CreateClientProxy(endPointAddress.Text, Username.Text, Password.Password);

			var filter = new TextSearchFilter
			{
				NotOlderThen = IsNotOlderThen.IsChecked.HasValue && IsNotOlderThen.IsChecked.Value
								? (NotOlderThen.SelectedDate.Value + ((DateTime)NotOlderThenTime.SelectedValue).TimeOfDay).ToUniversalTime()
								: (DateTime?)null,
				Upto = IsUpto.IsChecked.HasValue && IsUpto.IsChecked.Value
						? (Upto.SelectedDate.Value + ((DateTime)UptoTime.SelectedValue).TimeOfDay).ToUniversalTime()
						: (DateTime?)null,
				Contains = !string.IsNullOrEmpty(SearchText.Text)
							? SearchText.Text
							: null,
				Skip = Skip.Text.Length > 0
						? (int?)int.Parse(Skip.Text)
						: null,
				Take = Take.Text.Length > 0
						? (int?)int.Parse(Take.Text)
						: null,
			};

			adminService.GetUsersCountCompleted += OnGetUsersCountCompleted;
			adminService.GetUsersCountAsync(filter);

			adminService.GetUsersCompleted += GetSearchCompleted;
			adminService.GetUsersAsync(filter);
		}

		private void OnGetUsersCountCompleted(object sender, GetUsersCountCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				TotalFound.Text = args.Result.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				Helpers.ErrorProcessing(args);
			}
		}

		#endregion

		#region Callback handlers

		private void GetSearchCompleted(object sender, GetUsersCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				adminService.Close();
				UsersList.ItemsSource = args.Result;
			}
			else
			{
				Helpers.ErrorProcessing(args);
			}

			Search.Content = "Search";
			Search.IsEnabled = true;
		}

//		private void GetFeedbacksCountCompleted(object sender, GetFeedbacksCountCompletedEventArgs args)
//		{
//			feedbackManagmentServiceClient.Close();
//
//			var feedbackCounts = "- indeterminate -";
//
//			if (args.Error == null)
//			{
//				feedbackCounts = string.Format("{0:N0}", (decimal)args.Result);
//			}
//			else
//			{
//				Helpers.ErrorProcessing(args);
//			}
//
//			FoundCount.Content = feedbackCounts;
//			FoundCount.IsEnabled = true;
//		}

		#endregion

		private void OnClearClick(object sender, RoutedEventArgs e)
		{
			SearchText.Text = string.Empty;
			UsersList.ItemsSource = null;
		}

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
	}
}
