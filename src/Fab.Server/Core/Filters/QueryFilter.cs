// <copyright file="QueryFilter.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-02-05" />

using System;
using System.Diagnostics;

namespace Fab.Server.Core.Filters
{
	/// <summary>
	/// Common query filter.
	/// </summary>
	[DebuggerDisplay("{NotOlderThen} {Upto} {Skip} {Take}")]
	public class QueryFilter : IQueryFilter
	{
		#region Implementation of IQueryFilter

		/// <summary>
		/// Gets or sets strict lower boundary date for all items in result sequence.
		/// </summary>
		public DateTime? NotOlderThen { get; set; }

		/// <summary>
		/// Gets or sets upper boundary date for all items in result sequence.
		/// </summary>
		public DateTime? Upto { get; set; }

		/// <summary>
		/// Gets or sets how many items should be skipped from start of result sequence.
		/// </summary>
		public int? Skip { get; set; }

		/// <summary>
		/// Gets or sets how many items should be contained in result sequence.
		/// </summary>
		public int? Take { get; set; }

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current query filter.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current query filter.</returns>
		public override string ToString()
		{
			return string.Format(
				"NotOlderThen={0} Upto={1} Skip={2} Take={3}", NotOlderThen, Upto, Skip, Take);
		}

		#endregion
	}
}