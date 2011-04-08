// <copyright file="ICategoriesRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-05" />

using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Specify interface of common operation with user categories.
	/// </summary>
	public interface ICategoriesRepository : IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
	{
		/// <summary>
		/// Gets categories for specific user.
		/// </summary>
		IEnumerable<CategoryDTO> Categories { get; }

		/// <summary>
		/// Download all categories for specific user.
		/// </summary>
		void DownloadAll();
	}
}