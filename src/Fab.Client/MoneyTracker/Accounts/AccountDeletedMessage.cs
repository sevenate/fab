// <copyright file="AccountDeletedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Send by <see cref="AccountDeletedMessage"/> after one of the account has been deleted.
	/// </summary>
	public class AccountDeletedMessage
	{
		/// <summary>
		/// Gets or sets updated user account.
		/// </summary>
		public AccountDTO Account { get; set; }
	}
}