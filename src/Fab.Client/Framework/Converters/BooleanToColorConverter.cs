// <copyright file="BooleanToVisibilityInvertConverter.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-12-10" />
// <summary>An <see cref="IValueConverter"/> which inversely converts <see cref="bool"/> to <see cref="Visibility"/>.</summary>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Fab.Client.Framework.Extensions;

namespace Fab.Client.Framework.Converters
{
    /// <summary>
    /// An <see cref="IValueConverter"/> which convert <see cref="bool"/> to border color.
    /// </summary>
	public class BooleanToColorConverter : IValueConverter
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
			return ((bool)value)
				? new SolidColorBrush(((string)parameter).ToColor())
				: new SolidColorBrush(Colors.Transparent);
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