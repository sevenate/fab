// <copyright file="IQueryFilter.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-21" />

using System;

namespace Fab.Server.Core.Filters
{
	/// <summary>
	/// Common query filter interface.
	/// </summary>
	public interface IQueryFilter
	{
		/// <summary>
		/// Gets or sets strict lower boundary date for all items in result sequence.
		/// </summary>
		DateTime? NotOlderThen { get; set; }

		/// <summary>
		/// Gets or sets upper boundary date for all items in result sequence.
		/// </summary>
		DateTime? Upto { get; set; }

		/// <summary>
		/// Gets or sets how many items should be skipped from start of result sequence.
		/// </summary>
		int? Skip { get; set; }

		/// <summary>
		/// Gets or sets how many items should be contained in result sequence.
		/// </summary>
		int? Take { get; set; }
	}
}