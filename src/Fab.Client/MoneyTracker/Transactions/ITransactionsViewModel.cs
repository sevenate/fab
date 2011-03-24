// <copyright file="ITransactionsViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-04-13" />

using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// General transactions view model interface.
	/// </summary>
	public interface ITransactionsViewModel : IHandle<LoggedOutMessage>
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