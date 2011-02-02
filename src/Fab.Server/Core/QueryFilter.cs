// <copyright file="QueryFilter.cs" company="HD">
// 	Copyright (c) 2009 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2009-07-29</date>
// </author>
// <editor name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2009-07-29</date>
// </editor>
// <summary>Common query filter.</summary>

using System;
using System.Diagnostics;

namespace Fab.Server.Core
{
	/// <summary>
	/// Common query filter.
	/// </summary>
	[DebuggerDisplay("{NotOlderThen} {Upto} {Contains} {Skip} {Take}")]
	public class QueryFilter : IQueryFilter
	{
		#region Implementation of IQueryFilter

		/// <summary>
		/// Gets or sets how many items should be skipped from start of result sequence.
		/// </summary>
		public int? Skip { get; set; }

		/// <summary>
		/// Gets or sets how many items should be contained in result sequence.
		/// </summary>
		public int? Take { get; set; }

		/// <summary>
		/// Gets or sets a "full text search" query on every string property of items in result sequence.
		/// </summary>
		public string Contains { get; set; }

		/// <summary>
		/// Gets or sets strict lower boundary date for all items in result sequence.
		/// </summary>
		public DateTime? NotOlderThen { get; set; }

		/// <summary>
		/// Gets or sets upper boundary date for all items in result sequence.
		/// </summary>
		public DateTime? Upto { get; set; }

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns a System.String that represents the current query filter.
		/// </summary>
		/// <returns>A System.String that represents the current query filter.</returns>
		public override string ToString()
		{
			return string.Format(
				"NotOlderThen={3}\nUpto={4}\nContains={2}\nSkip={0}\nTake={1}", NotOlderThen, Upto, Contains, Skip, Take);
		}

		#endregion
	}
}