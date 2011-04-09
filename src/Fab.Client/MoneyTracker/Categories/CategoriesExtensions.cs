// <copyright file="CategoriesExtensions.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-09" />

using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Extension methods that's simplify working with categories.
	/// </summary>
	public static class CategoriesExtensions
	{
		/// <summary>
		/// Look up in <paramref name="repository"/> for the <see cref="CategoryDTO"/> by the <paramref name="id"/> key.
		/// If nothing found, null will be returned.
		/// </summary>
		/// <param name="id">Category unique ID.</param>
		/// <param name="repository">Repository to lookup in.</param>
		/// <returns>Found category instance or null otherwise.</returns>
		public static CategoryDTO LookupIn(this int? id, ICategoriesRepository repository)
		{
			return id.HasValue
			       	? repository.ByKey(id.Value)
			       	: null;
		}
	}
}