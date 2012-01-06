using System.Windows;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Core;

namespace Fab.Managment
{
	/// <summary>
	/// Interaction logic for FeedbackItem.xaml
	/// </summary>
	public partial class UserItem
	{
		private AdminServiceClient feedbackManagmentServiceClient;

		public UserItem()
		{
			InitializeComponent();
		}

		private void DeleteClick(object sender, RoutedEventArgs e)
		{
			var button = (FrameworkElement)sender;
			button.IsEnabled = false;

			var userDTO = (AdminUserDTO) button.DataContext;

			feedbackManagmentServiceClient = Helpers.CreateClientProxy(null, null, null);

			feedbackManagmentServiceClient.DeleteUserCompleted += DeleteFeedbackCompleted;
			feedbackManagmentServiceClient.DeleteUser(userDTO.Id);
		}

		private void DeleteFeedbackCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs args)
		{
			feedbackManagmentServiceClient.Close();

			if (args.Error != null)
			{
				Helpers.ErrorProcessing(args);
			}

			((FrameworkElement) args.UserState).IsEnabled = true;
		}
	}
}