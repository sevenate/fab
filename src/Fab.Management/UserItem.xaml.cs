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
		public UserItem()
		{
			InitializeComponent();
		}

		private void DeleteClick(object sender, RoutedEventArgs e)
		{
			var button = (FrameworkElement)sender;
			button.IsEnabled = false;

			var userDTO = (AdminUserDTO) button.DataContext;

			var adminServiceClient = Helpers.CreateClientProxy(null, null, null);

			adminServiceClient.DeleteUserCompleted += (o, args) =>
			                                          {
														  if (args.Error != null)
														  {
															  Helpers.ErrorProcessing(args);
														  }
														
														  button.IsEnabled = true;
													  };
			adminServiceClient.DeleteUserAsync(userDTO.Id);
		}

		private void DatabaseSize_OnClick(object sender, RoutedEventArgs e)
		{
			var button = (FrameworkElement)sender;
			button.IsEnabled = false;

			var userDTO = (AdminUserDTO)button.DataContext;

			var adminServiceClient = Helpers.CreateClientProxy(null, null, null);

			adminServiceClient.OptimizeUserDatabaseCompleted += (o, args) =>
			                                                    {
																	if (args.Error != null)
																	{
																		Helpers.ErrorProcessing(args);
																	}
																	else
																	{
																		userDTO.DatabaseSize = args.Result;
																	}

																	button.IsEnabled = true;
																};
			adminServiceClient.OptimizeUserDatabaseAsync(userDTO.Id);
		}
	}
}