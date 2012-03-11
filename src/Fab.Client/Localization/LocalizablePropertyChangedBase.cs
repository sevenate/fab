using Caliburn.Micro;
using Fab.Client.Resources;

namespace Fab.Client.Localization
{
	public class LocalizablePropertyChangedBase : PropertyChangedBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizablePropertyChangedBase"/> class.
		/// </summary>
		public LocalizablePropertyChangedBase()
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