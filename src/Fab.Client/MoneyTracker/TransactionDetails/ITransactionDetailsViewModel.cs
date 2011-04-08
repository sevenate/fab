// <copyright file="ITransactionDetailsViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-13" />

using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;

namespace Fab.Client.MoneyTracker.TransactionDetails
{
	/// <summary>
	/// General transaction detail view model interface.
	/// </summary>
	public interface ITransactionDetailsViewModel : IHandle<AccountsUpdatedMessage>, IHandle<CategoriesUpdatedMessage>
	{
		/// <summary>
		/// Open specific deposit or withdrawal transaction to edit.
		/// </summary>
		/// <param name="transaction">Transaction to edit.</param>
		/// <param name="accountId">Current selected account.</param>
		void Edit(TransactionDTO transaction, int accountId);
	}
}