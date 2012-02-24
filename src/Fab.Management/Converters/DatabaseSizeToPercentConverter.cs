//------------------------------------------------------------
// <copyright file="DatabaseSizeToPercentConverter.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Fab.Managment.Converters
{
	/// <summary>
	/// Convert database size in long? into "% of 4 Gb size limit" string.
	/// </summary>
	[ValueConversion(typeof(long?), typeof(string))]
	public class DatabaseSizeToPercentConverter : IValueConverter
	{
		private const long Gigabyte = 4 * 1024 * 1024 * 1024L;

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
			var size = (long?)value;

			if (!size.HasValue)
			{
				return string.Empty;
			}

			return string.Format("{0:N4}%", (double)size.Value * 100 / Gigabyte);
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
		private static DatabaseSizeToPercentConverter instance;

		/// <summary>
		/// Prevents a default instance of the <see cref="DatabaseSizeToPercentConverter"/> class from being created.
		/// </summary>
		private DatabaseSizeToPercentConverter()
		{
		}

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static DatabaseSizeToPercentConverter Inst
		{
			[DebuggerStepThrough]
			get
			{
				if (instance == null)
				{
					instance = new DatabaseSizeToPercentConverter();
				}

				return instance;
			}
		}

		#endregion
	}
}