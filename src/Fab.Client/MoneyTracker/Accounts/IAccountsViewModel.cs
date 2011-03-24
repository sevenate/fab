// <copyright file="IAccountsViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-04-13" />

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// General accounts view model interface.
	/// </summary>
	public interface IAccountsViewModel : IHandle<LoggedOutMessage>
	{
		/// <summary>
		/// Gets accounts for specific user.
		/// </summary>
		ICollectionView Accounts { get; }

		/// <summary>
		/// Download all accounts for specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		IEnumerable<IResult> LoadAllAccounts();

		/// <summary>
		/// Raised right after accounts were reloaded from server.
		/// </summary>
		event EventHandler<EventArgs> Reloaded;
	}
}