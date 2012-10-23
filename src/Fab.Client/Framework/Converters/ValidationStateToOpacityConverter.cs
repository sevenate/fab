//------------------------------------------------------------
// <copyright file="ValidationStateToOpacityConverter.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Data;
using Fab.Client.Framework.Controls;

namespace Fab.Client.Framework.Converters
{
	/// <summary>
	/// Convert <see cref="ValidationStates"/> to Opacity double value in range of [0.0..1.0].
	/// </summary>
	public class ValidationStateToOpacityConverter : IValueConverter
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
			var state = (ValidationStates) value;

			switch (state)
			{
				case ValidationStates.Undefined:
					return 0.0;

				case ValidationStates.Checking:
				case ValidationStates.Error:
					return 1.0;
			}

			// ValidationStates.Ok:
			return 0.0;
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