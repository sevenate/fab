// <copyright file="ZeroToVisibilityConverter.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-02-04" />
// <summary>An <see cref="IValueConverter"/> which converts 0 (integer "zero") to <see cref="Visibility.Collapsed"/> and any other value to <see cref="Visibility.Visible"/>.</summary>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Fab.Client.Framework.Converters
{
    /// <summary>
	/// An <see cref="IValueConverter"/> which converts 0 (integer "zero") to <see cref="Visibility.Collapsed"/> and any other value to <see cref="Visibility.Visible"/>.
    /// </summary>
	public class ZeroToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="bool"/> value to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value == 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a value <see cref="Visibility"/> value to a <see cref="bool"/> value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
        	throw new NotSupportedException();
        }
    }
}