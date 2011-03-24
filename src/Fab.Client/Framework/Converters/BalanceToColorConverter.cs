// <copyright file="BalanceToColorConverter.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" />
// <summary>Convert negative <see cref="decimal"/> value to <see cref="Colors.Red"/>.</summary>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Fab.Client.Framework.Converters
{
	/// <summary>
	/// Convert negative (or zero) <see cref="decimal"/> value to <see cref="Colors.Red"/>
	/// and positive - to <see cref="Colors.Green"/>.
	/// </summary>
	public class BalanceToColorConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <returns>The value to be passed to the target dependency property.</returns>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is decimal)
			{
				var balance = (decimal) value;

				//TODO: move balance colors values to resources.
				return balance < 0 ? new SolidColorBrush(Colors.Red)
								   : balance == 0 ? new SolidColorBrush(Colors.Yellow)
												  : new SolidColorBrush(new Color { A = 0xFF, R = 0, G = 0xFF, B = 0});
			}

			return value;
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.
		/// This method is called only in <see cref="BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <returns>The value to be passed to the source object.</returns>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}