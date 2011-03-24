// <copyright file="DateRange.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-02-09" />
// <summary>Common date ranges.</summary>

using System.ComponentModel;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// Common date ranges.
	/// </summary>
	public enum DateRange
	{
		/// <summary>
		/// The "1 day" range.
		/// </summary>
		[Description("Day")]
		Day,

		/// <summary>
		/// The "4 day" range.
		/// </summary>
		[Description("4 Days")]
		FourDays,

		/// <summary>
		/// The "1 week" range.
		/// </summary>
		[Description("Week")]
		Week,

		/// <summary>
		/// The "1 month" range.
		/// </summary>
		[Description("Month")]
		Month,
	}
}