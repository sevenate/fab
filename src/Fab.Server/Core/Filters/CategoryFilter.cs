// <copyright file="CategoryFilter.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-21" />

using System.Diagnostics;

namespace Fab.Server.Core.Filters
{
	/// <summary>
	/// Category query filter.
	/// </summary>
	[DebuggerDisplay("{CategoryId}")]
	public class CategoryFilter : QueryFilter
	{
		/// <summary>
		/// Gets or sets a <see cref="Category"/> ID that should be associated to every item in result sequence.
		/// </summary>
		public int? CategoryId { get; set; }

		#region Overrides of Object

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current query filter.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current query filter.</returns>
		public override string ToString()
		{
			return string.Format("{0} CategoryID={1}", base.ToString(), CategoryId);
		}

		#endregion
	}
}