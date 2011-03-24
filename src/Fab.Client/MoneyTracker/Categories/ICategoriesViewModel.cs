// <copyright file="ICategoriesViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-04-13" />

using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// General categories view model interface.
	/// </summary>
	public interface ICategoriesViewModel : IHandle<LoggedOutMessage>
	{
		/// <summary>
		/// Gets categories for specific user.
		/// </summary>
		IObservableCollection<CategoryDTO> Categories { get; }

		/// <summary>
		/// Download all categories for specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		IEnumerable<IResult> LoadAllCategories();

		/// <summary>
		/// Raised right after categories were reloaded from server.
		/// </summary>
		event EventHandler<EventArgs> Reloaded;
	}
}