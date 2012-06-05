// <copyright file="CategoriesUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System.Collections.Generic;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Send by <see cref="CategoriesRepository.Entities"/> after <see cref="CategoriesRepository"/> has been updated.
	/// </summary>
	public class CategoriesUpdatedMessage
	{
		/// <summary>
		/// Gets or sets all current user categories.
		/// </summary>
		public IEnumerable<CategoryDTO> Categories { get; set; }
	}
}