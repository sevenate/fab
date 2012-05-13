using Caliburn.Micro;
using Fab.Client.Resources;

namespace Fab.Client.Localization
{
	public class LocalizableConductor<T> : Conductor<T>.Collection.OneActive
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizableConductor{T}"/> class.
		/// </summary>
		public LocalizableConductor()
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