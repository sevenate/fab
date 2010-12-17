// <copyright file="ITransactionDetailsViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-13" />
// <summary>General transaction detail view model interface.</summary>

using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.TransactionDetails
{
	/// <summary>
	/// General transaction detail view model interface.
	/// </summary>
	public interface ITransactionDetailsViewModel
	{
		/// <summary>
		/// Open specific deposit or withdrawal transaction to edit.
		/// </summary>
		/// <param name="transaction">Transaction to edit.</param>
		void Edit(TransactionDTO transaction);
	}
}