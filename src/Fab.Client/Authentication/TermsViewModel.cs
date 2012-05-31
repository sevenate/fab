using System.ComponentModel.Composition;
using Fab.Client.Localization;

namespace Fab.Client.Authentication
{
	[Export(typeof(TermsViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class TermsViewModel : LocalizableScreen
	{
		[ImportingConstructor]
		public TermsViewModel()
		{
		}

		public string Text { get; set; }

		public void Close()
		{
			TryClose();
		}
	}
}