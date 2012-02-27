//------------------------------------------------------------
// <copyright file="TranslateExtension.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Fab.Client.Localization
{
	/// <summary>
	/// Translation markup extension.
	/// <remarks>
	/// Inspired by Christian Moser's http://www.wpftutorial.net/LocalizeMarkupExtension.html.
	/// </remarks>
	/// </summary>
	public class TranslateExtension : MarkupExtension
	{
		#region Fields

		/// <summary>
		/// Target dependence object.
		/// </summary>
		private DependencyObject targetObject;

		/// <summary>
		/// Target dependency property.
		/// </summary>
		private DependencyProperty targetProperty;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the resource key.
		/// </summary>
		/// <value>The resource key.</value>
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets a format string that is used to format the value
		/// </summary>
		/// <value>The format.</value>
		public string Format { get; set; }

		#endregion

		#region .Ctors / Finalize

		/// <summary>
		/// Initializes a new instance of the <see cref="TranslateExtension"/> class.
		/// </summary>
		public TranslateExtension()
		{
			Translator.CultureChanged += CultureChanged;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TranslateExtension"/> class.
		/// </summary>
		/// <param name="key">Localization value key.</param>
		public TranslateExtension(string key) : this()
		{
			Key = key;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="TranslateExtension"/> class,
		/// releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="TranslateExtension"/> is reclaimed by garbage collection.
		/// </summary>
		~TranslateExtension()
		{
			Translator.CultureChanged -= CultureChanged;
		}

		#endregion

		#region MarkupExtension Overrides

		/// <summary>
		/// When implemented in a derived class,
		/// returns an object that is set as the value of the target property for this markup extension.
		/// </summary>
		/// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
		/// <returns>The object value to set on the property where the extension is applied.</returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			// Ignore if in design mode
			if ((bool) DesignerProperties
			           	.IsInDesignModeProperty
			           	.GetMetadata(typeof (DependencyObject)).DefaultValue)
			{
				return null;
			}

			// Resolve the depending object and property
			if (targetObject == null)
			{
				var targetHelper = (IProvideValueTarget) serviceProvider.GetService(typeof (IProvideValueTarget));

				// Note: Important check for System.Windows.SharedDp,
				// when using markup extension inside of ControlTemplates and DataTemplates.
				// Details: http://social.msdn.microsoft.com/forums/en-US/wpf/thread/a9ead3d5-a4e4-4f9c-b507-b7a7d530c6a9/
				if (!(targetHelper.TargetObject is DependencyObject))
				{
					return this;
				}

				targetObject = (DependencyObject) targetHelper.TargetObject;
				targetProperty = (DependencyProperty) targetHelper.TargetProperty;
			}

			return ProvideValueInternal();
		}

		#endregion

		#region Private

		/// <summary>
		/// Handles the <see cref="Translator.CultureChanged"/> event.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void CultureChanged(object sender, EventArgs e)
		{
			if (targetObject != null && targetProperty != null)
			{
				targetObject.SetValue(targetProperty, ProvideValueInternal());
			}
		}

		/// <summary>
		/// Provides the value internal.
		/// </summary>
		/// <returns>Localized value.</returns>
		private object ProvideValueInternal()
		{
			// Get the localized value
			object value = Translator.GetValue(Key);

			// If no value is available, return the key
			if (value == null)
			{
				// Return the key surrounded by question marks for string type properties
				value = string.Concat("?", Key, "?");
			}

			// Format the value if a format string is provided and the type implements IFormattable
			if (value is IFormattable && Format != null)
			{
				((IFormattable) value).ToString(Format, CultureInfo.CurrentCulture);
			}

			return value;
		}

		#endregion
	}
}