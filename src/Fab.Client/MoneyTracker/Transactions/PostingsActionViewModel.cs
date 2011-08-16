// <copyright file="PostingsActionViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-08-16" />

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// View model for <see cref="PostingsActionView"/>.
	/// </summary>
	[Export(typeof(PostingsActionViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class PostingsActionViewModel : Screen, IPostingPanel
	{
		/// <summary>
		/// Open dialog for creating new transaction.
		/// </summary>
		public void NewTransaction()
		{
			((TransactionsViewModel)Parent).NewTransaction();
		}

		/// <summary>
		/// Open dialog for creating new transfer.
		/// </summary>
		public void NewTransfer()
		{
			((TransactionsViewModel)Parent).NewTransfer();
		}

		/// <summary>
		/// Download all transactions for specific account of the specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public IEnumerable<IResult> DownloadAllTransactions()
		{
			return ((TransactionsViewModel)Parent).DownloadAllTransactions();
		}
	}
}