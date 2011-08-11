// <copyright file="CurrentAccountChangedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

namespace Fab.Client.MoneyTracker.Accounts.Single
{
	/// <summary>
	/// Send by <see cref="AccountsViewModel"/> after selected account has been changed.
	/// </summary>
	public class CurrentAccountChangedMessage
	{
		/// <summary>
		/// Gets or sets current selected account.
		/// </summary>
		public AccountViewModel CurrentAccount { get; set; }
	}
}