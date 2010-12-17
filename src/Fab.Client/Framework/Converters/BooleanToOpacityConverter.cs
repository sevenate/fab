// <copyright file="BooleanToOpacityConverter.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-12-10" />
// <summary>Convert <see cref="bool"/> <c>True</c> to <c>100</c> opacity and <c>False</c> to 0 opacity.</summary>

using System;
using System.Globalization;
using System.Windows.Data;

namespace Fab.Client.Framework.Converters
{
	/// <summary>
	/// Convert <see cref="bool"/> value <c>True</c> to <c>100.0</c> opacity and <c>False</c> to <c>0.0</c> opacity.
	/// </summary>
	public class BooleanToOpacityConverter : IValueConverter
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
			if (value is bool)
			{
				return (bool)value ? 100.0 : 0.0;
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
			if (value is double)
			{
				return (double)value == 100.0;
			}

			return value;
		}

		#endregion
	}
}