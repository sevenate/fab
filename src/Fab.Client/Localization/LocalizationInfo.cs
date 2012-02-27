//------------------------------------------------------------
// <copyright file="LocalizationInfo.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Globalization;

namespace Fab.Client.Localization
{
	/// <summary>
	/// Represent translation and culture information retrieved from language file.
	/// </summary>
	public class LocalizationInfo
	{
		#region Properties

		/// <summary>
		/// Gets custom localization friendly name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets localization culture information.
		/// </summary>
		public CultureInfo Culture { get; private set; }

		#endregion

		#region .Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizationInfo"/> class.
		/// </summary>
		/// <param name="name">Localization name.</param>
		/// <param name="culture">Culture information.</param>
		internal LocalizationInfo(string name, CultureInfo culture)
		{
			Name = name;
			Culture = culture;
		}

		#endregion
	}
}