//------------------------------------------------------------
// <copyright file="AccountsUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Collections.Generic;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Send by <see cref="AccountsRepository"/> after <see cref="AccountsRepository.Download()"/> has been updated.
	/// </summary>
	public class AccountsUpdatedMessage
	{
		/// <summary>
		/// Gets or sets all current user accounts.
		/// </summary>
		public IEnumerable<AccountDTO> Accounts { get; set; }
	}
}