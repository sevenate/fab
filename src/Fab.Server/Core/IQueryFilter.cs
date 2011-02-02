// <copyright file="IQueryFilter.cs" company="HD">
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
// <summary>Common query filter interface.</summary>

using System;

namespace Fab.Server.Core
{
	/// <summary>
	/// Common query filter interface.
	/// </summary>
	public interface IQueryFilter
	{
		/// <summary>
		/// Gets or sets how many items should be skipped from start of result sequence.
		/// </summary>
		int? Skip { get; set; }

		/// <summary>
		/// Gets or sets how many items should be contained in result sequence.
		/// </summary>
		int? Take { get; set; }

		/// <summary>
		/// Gets or sets a "full text search" query on every string property of items in result sequence.
		/// </summary>
		string Contains { get; set; }

		/// <summary>
		/// Gets or sets strict lower boundary date for all items in result sequence.
		/// </summary>
		DateTime? NotOlderThen { get; set; }

		/// <summary>
		/// Gets or sets upper boundary date for all items in result sequence.
		/// </summary>
		DateTime? Upto { get; set; }
	}
}