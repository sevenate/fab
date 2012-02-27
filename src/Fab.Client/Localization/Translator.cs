//------------------------------------------------------------
// <copyright file="Translator.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Fab.Client.Resources;

namespace Fab.Client.Localization
{
	/// <summary>
	/// Helper translation changer.
	/// <code>
	/// Translator.Culture = (CultureInfo)languages.SelectedItem
	/// </code>
	/// /// <remarks>
	/// Inspired by Christian Moser's http://www.wpftutorial.net/LocalizeMarkupExtension.html.
	/// </remarks>
	/// </summary>
	public static class Translator
	{
		#region Fields

		/// <summary>
		/// Fallback culture.
		/// </summary>
		public static readonly CultureInfo DefaultCulture;

		/// <summary>
		/// Gets or sets current localization.
		/// </summary>
		private static LocalizationInfo currentLocalization;

		#endregion

		#region .Ctors

		/// <summary>
		/// Initializes static members of the <see cref="Translator"/> class.
		/// </summary>
		static Translator()
		{
			DefaultCulture = new CultureInfo("en-US");
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets all available localizations.
		/// </summary>
		/// <returns>Enumerates through all available localizations.</returns>
		public static IEnumerable<LocalizationInfo> AvailableLocalizations
		{
			get
    		{
				yield return new LocalizationInfo("English", new CultureInfo("en"));
				yield return new LocalizationInfo("Русский", new CultureInfo("ru"));
    		}
		}

		/// <summary>
		/// Gets or sets current localization to new value.
		/// </summary>
		public static LocalizationInfo CurrentLocalization
		{
			get { return currentLocalization; }
			set
			{
				currentLocalization = value;

				if (value != null && value.Culture != null)
				{
					Thread.CurrentThread.CurrentUICulture = value.Culture;
				}
				else
				{
					// fallback to default culture available in resources
					Thread.CurrentThread.CurrentUICulture = DefaultCulture;
				}

				if (CultureChanged != null)
				{
					CultureChanged(null, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets current UI culture.
		/// </summary>
		public static CultureInfo CurrentUICulture
		{
			get { return Thread.CurrentThread.CurrentUICulture; }
		}

		/// <summary>
		/// Gets current culture.
		/// </summary>
		public static CultureInfo CurrentCulture
		{
			get { return Thread.CurrentThread.CurrentCulture; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets a localized value for the specified resource key from assembly resources.
		/// </summary>
		/// <param name="key">The localized resource key.</param>
		/// <returns>Localized resource value.</returns>
		public static object GetValue(string key)
		{
			return null; // Strings.ResourceManager.GetString(key);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when current application culture was changed.
		/// </summary>
		public static event EventHandler CultureChanged;

		#endregion
	}
}