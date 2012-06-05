using System.ComponentModel.Composition;
using Fab.Client.Localization;

namespace Fab.Client.Shell
{
	[Export(typeof (ErrorDialogViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class ErrorDialogViewModel : LocalizableScreen
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