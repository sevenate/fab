// <copyright file="ICategoriesRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-05" />

using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Specify interface of common operation with user categories.
	/// </summary>
	public interface ICategoriesRepository : IRepository<CategoryDTO, int>
	{
		/// <summary>
		/// Create new category for specific user.
		/// </summary>
		/// <param name="name">New category name.</param>
		/// <param name="categoryType">New category type.</param>
		/// <returns>Created category.</returns>
		CategoryDTO Create(string name, CategoryType categoryType);
	}
}