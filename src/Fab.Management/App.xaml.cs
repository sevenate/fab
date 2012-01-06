using System.Windows;
using System.Windows.Threading;

namespace Fab.Managment
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
#if !DEBUG
			e.Handled =
				MessageBox.Show(
					e.Exception.ToString(), "Unhandled exception occur. Continue?", MessageBoxButton.YesNo, MessageBoxImage.Error)
				== MessageBoxResult.Yes;
#endif
		}
	}
}
