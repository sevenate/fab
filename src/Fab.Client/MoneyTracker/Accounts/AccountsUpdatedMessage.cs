// <copyright file="AccountsUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;
using System.Collections.Generic;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Send by <see cref="AccountsRepository"/> after <see cref="AccountsRepository.Accounts"/> has been updated.
	/// </summary>
	public class AccountsUpdatedMessage
	{
		/// <summary>
		/// Gets or sets the error if one occurred.
		/// </summary>
		public Exception Error { get; set; }

		/// <summary>
		/// Gets or sets all current user accounts.
		/// </summary>
		public IEnumerable<AccountDTO> Accounts { get; set; }
	}
}