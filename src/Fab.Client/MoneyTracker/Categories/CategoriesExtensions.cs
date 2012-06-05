// <copyright file="CategoriesExtensions.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-09" />

using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories.Single;

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

		/// <summary>
		/// Convert <see cref="CategoryDTO" /> into <see cref="CategoryViewModel" />.
		/// </summary>
		/// <param name="categoryDTO">Source object with data.</param>
		/// <returns>Destination object with mapped data.</returns>
		/// <remarks>
		/// TODO: find a way to use "auto mapper" for SL4 instead of this method
		/// </remarks>
		public static CategoryViewModel Map(this CategoryDTO categoryDTO)
		{
			var accountViewModel = IoC.Get<CategoryViewModel>();

			accountViewModel.Id = categoryDTO.Id;
			accountViewModel.Name = categoryDTO.Name;
			accountViewModel.Popularity = categoryDTO.Popularity;
			accountViewModel.CategoryTypeWrapper = new EnumWrapper<CategoryType>(categoryDTO.CategoryType);

			return accountViewModel;
		}
	}
}