//------------------------------------------------------------
// <copyright file="DateRange.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ComponentModel;

namespace Fab.Client.MoneyTracker.Postings
{
	/// <summary>
	/// Common date ranges.
	/// Todo: Remove as not used anymore.
	/// </summary>
	[Obsolete]
	public enum DateRange
	{
		/// <summary>
		/// The "1 day" range.
		/// </summary>
		[Description("Day")]
		Day = 1,

		/// <summary>
		/// The "4 day" range.
		/// </summary>
		[Description("4 Days")]
		FourDays = 4,

		/// <summary>
		/// The "1 week" range.
		/// </summary>
		[Description("Week")]
		Week = 7,

		/// <summary>
		/// The "1 month" range.
		/// </summary>
		[Description("Month")]
		Month = -1, // "30" could confuse, cause why not "31" or "28"?
	}
}