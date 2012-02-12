//------------------------------------------------------------
// <copyright file="CultureSettings.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Threading;
using System.Windows.Markup;

namespace Fab.Managment.Framework
{
	/// <summary>
	/// Used to get <see cref="XmlLanguage"/> for current OS culture.
	/// </summary>
	public class CultureSettings
	{
		/// <summary>
		/// Gets <see cref="XmlLanguage"/> for current OS culture.
		/// Used for input data format binding and StringFormat in binding.
		/// </summary>
		public XmlLanguage Language
		{
			get { return XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name); }
		}
	}
}