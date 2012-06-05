//------------------------------------------------------------
// <copyright file="CultureSettings.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Threading;
using System.Windows.Markup;
using Caliburn.Micro;
using Fab.Client.Localization;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Used to get <see cref="XmlLanguage"/> for current OS culture.
	/// </summary>
	public class CultureSettings : PropertyChangedBase
	{
		/// <summary>
		/// Gets <see cref="XmlLanguage"/> for current OS culture.
		/// Used for input data format binding and StringFormat in binding.
		/// </summary>
		public XmlLanguage Language
		{
			get { return XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name); }
		}

		public CultureSettings()
		{
			Translator.CultureChanged += (sender, args) => NotifyOfPropertyChange(() => Language);
		}
	}
}