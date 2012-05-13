// <copyright file="TextSearchFilter.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-21" />

using System.Diagnostics;

namespace Fab.Server.Core.Filters
{
	/// <summary>
	/// Text search query filter.
	/// </summary>
	[DebuggerDisplay("{Contains}")]
	public class TextSearchFilter : QueryFilter
	{
		/// <summary>
		/// Gets or sets a "full text search" query on every string property of items in result sequence.
		/// </summary>
		public string Contains { get; set; }

		#region Overrides of Object

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current query filter.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current query filter.</returns>
		public override string ToString()
		{
			return string.Format("{0} Contains={1}", base.ToString(), Contains);
		}

		#endregion
	}
}