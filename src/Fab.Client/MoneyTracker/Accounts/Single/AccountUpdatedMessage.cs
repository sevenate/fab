// <copyright file="AccountUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts.Single
{
	/// <summary>
	/// Send by <see cref="AccountsRepository"/> after one of the accounts has been updated.
	/// </summary>
	public class AccountUpdatedMessage
	{
		/// <summary>
		/// Gets or sets updated user account.
		/// </summary>
		public AccountDTO Account { get; set; }
	}
}