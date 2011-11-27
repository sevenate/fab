using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Client.Shell
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

		public void Report()
		{
			throw new NotImplementedException();
		}

		public void Close()
		{
			TryClose();
		}
	}
}