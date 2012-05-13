using Caliburn.Micro;
using Fab.Client.Resources;

namespace Fab.Client.Localization
{
	public class LocalizableScreen : Screen
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public LocalizableScreen()
		{
			Translator.CultureChanged += (sender, args) => NotifyOfPropertyChange(() => LocalizedStrings);
		}

		private static readonly Strings strings = new Strings();
		public Strings LocalizedStrings
		{
			get { return strings; }
		}
	}
}