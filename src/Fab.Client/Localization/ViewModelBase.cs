using System.ComponentModel;
using Fab.Client.Resources;

namespace Fab.Client.Localization
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		#region Implementation of INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged = delegate {};

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public ViewModelBase()
		{
			Translator.CultureChanged += (sender, args) =>
			                             	{
			                             		OnPropertyChanged("LocalizedStrings");
			                             	};
		}

		private static readonly Strings Strings = new Strings();
		public Strings LocalizedStrings
		{
			get { return Strings; }
		}
	}
}