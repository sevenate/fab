﻿using System;
using System.Globalization;
using System.Windows.Data;
using Fab.Client.Framework.Controls;

namespace Fab.Client.Framework.Converters
{
	/// <summary>
	/// Convert <see cref="bool"/> input value to <see cref="ValidationStates"/>.
	/// </summary>
	public class BoolToValidationStateConverter : IValueConverter
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
			var input = (bool) value;

			return input ? ValidationStates.Ok : ValidationStates.Undefined;
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