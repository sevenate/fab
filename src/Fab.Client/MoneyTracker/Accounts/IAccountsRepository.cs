// <copyright file="IAccountsRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-05" />

using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Specify interface of common operation with user accounts.
	/// </summary>
	public interface IAccountsRepository : IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
	{
		/// <summary>
		/// Gets accounts for specific user.
		/// </summary>
		IEnumerable<AccountDTO> Accounts { get; }

		/// <summary>
		/// Download all accounts for specific user.
		/// </summary>
		void DownloadAll();

		/// <summary>
		/// Create new account for specific user.
		/// </summary>
		/// <param name="name">New account name.</param>
		/// <param name="assetTypeId">New account asset type.</param>
		/// <returns>Created account.</returns>
		AccountDTO Create(string name, int assetTypeId);
	}
}