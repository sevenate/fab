//------------------------------------------------------------
// <copyright file="Translator.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

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
		/// Default and fallback culture.
		/// </summary>
		public static readonly CultureInfo DefaultCulture = new CultureInfo("en");

		#endregion

		#region Properties

		/// <summary>
		/// Gets all supported cultures.
		/// </summary>
		/// <returns>Enumerates through all supported cultures.</returns>
		public static IEnumerable<CultureInfo> SupportedCultures
		{
			get
    		{
				yield return DefaultCulture;
				yield return new CultureInfo("ru");
				yield return new CultureInfo("uk");
    		}
		}

		/// <summary>
		/// Gets current UI culture.
		/// </summary>
		public static CultureInfo CurrentCulture
		{
			get { return Thread.CurrentThread.CurrentUICulture; }
			set
			{
				Thread.CurrentThread.CurrentUICulture = value ?? DefaultCulture;
				Thread.CurrentThread.CurrentCulture = value ?? DefaultCulture;
				CultureChanged(null, EventArgs.Empty);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when current application culture was changed.
		/// </summary>
		public static event EventHandler CultureChanged = delegate {};

		#endregion

		/// <summary>
		/// Initializes static members of the <see cref="Translator"/> class.
		/// </summary>
		static Translator()
		{
			// Determine user's current culture setting and if it is in the supported list - use it, or fallback to default culture
			var culture = SupportedCultures.FirstOrDefault(info => info.TwoLetterISOLanguageName == CurrentCulture.TwoLetterISOLanguageName);
			CurrentCulture = culture ?? DefaultCulture;
		}
	}
}