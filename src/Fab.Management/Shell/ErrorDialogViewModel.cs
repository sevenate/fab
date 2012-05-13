using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Managment.Shell
{
	[Export(typeof (ErrorDialogViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class ErrorDialogViewModel : Screen
	{
		[ImportingConstructor]
		public ErrorDialogViewModel()
		{
		}

		public string Error { get; set; }

		public void Close()
		{
			TryClose();
		}
	}
}