using System;
using System.Globalization;
using System.Windows.Data;

namespace Fab.Client.Framework.Converters
{
	public class NullToQuestionMarkConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value ?? "?";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}