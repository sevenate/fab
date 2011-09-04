// <copyright file="BalanceToColorConverter.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" />
// <summary>Convert negative <see cref="decimal"/> value to <see cref="Colors.Red"/>.</summary>

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Convert <see cref="CategoryType"/> enumeration into <see cref="Color"/>.
	/// </summary>
	public class CategoryTypeToColorConverter : IValueConverter
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
			if (value is CategoryType)
			{
				var categoryType = (CategoryType)value;

				//TODO: move colors values to resources.
				switch (categoryType)
				{
					case CategoryType.Common:
						return new SolidColorBrush(Colors.Yellow);
					case CategoryType.Deposit:
						return new SolidColorBrush(new Color { A = 0xFF, R = 0, G = 0xFF, B = 0 });
					case CategoryType.Withdrawal:
						return new SolidColorBrush(Colors.Red);
				}
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