// <copyright file="ITransactionsViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-13" />
// <summary>General transactions view model interface.</summary>

using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// General transactions view model interface.
	/// </summary>
	public interface ITransactionsViewModel
	{
		/// <summary>
		/// Gets transaction records.
		/// </summary>
		IObservableCollection<TransactionRecord> TransactionRecords { get; }

		/// <summary>
		/// Download all transactions for specific account of the specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		IEnumerable<IResult> DownloadAllTransactions();

		/// <summary>
		/// Raised right after categories were reloaded from server.
		/// </summary>
		event EventHandler<EventArgs> Reloaded;
	}
}