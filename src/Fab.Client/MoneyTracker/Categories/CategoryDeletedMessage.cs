﻿// <copyright file="CategoryUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Send by <see cref="CategoryDeletedMessage"/> after one of the category has been deleted.
	/// </summary>
	public class CategoryDeletedMessage
	{
		/// <summary>
		/// Gets or sets updated user category.
		/// </summary>
		public CategoryDTO Category { get; set; }
	}
}