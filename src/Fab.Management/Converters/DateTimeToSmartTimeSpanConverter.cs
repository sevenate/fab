//------------------------------------------------------------
// <copyright file="DateTimeToSmartTimeSpanConverter.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using Fab.Core;

namespace Fab.Managment.Converters
{
	/// <summary>
	/// Convert nullable <see cref="DateTime"/> value to local nullable <see cref="DateTime"/> value.
	/// </summary>
	[ValueConversion(typeof(DateTime?), typeof(DateTime?))]
	public class DateTimeToSmartTimeSpanConverter : IValueConverter
	{
		#region Implementation of IValueConverter

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var date = (DateTime?) value;

			if (!date.HasValue)
			{
				return null;
			}

			return date.Value.ToSmartTimespanUtc();
		}

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Singleton

		/// <summary>
		/// Singleton instance.
		/// </summary>
		private static DateTimeToSmartTimeSpanConverter instance;

		/// <summary>
		/// Prevents a default instance of the <see cref="DateTimeToSmartTimeSpanConverter"/> class from being created.
		/// </summary>
		private DateTimeToSmartTimeSpanConverter()
		{
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static DateTimeToSmartTimeSpanConverter Inst
		{
			[DebuggerStepThrough]
			get
			{
				if (instance == null)
				{
					instance = new DateTimeToSmartTimeSpanConverter();
				}

				return instance;
			}
		}

		#endregion
	}
}