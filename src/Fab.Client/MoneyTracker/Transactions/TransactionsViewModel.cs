// <copyright file="TransactionsViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-10" />
// <summary>Transactions view model.</summary>

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.TransactionDetails;
using Fab.Client.MoneyTracker.Transfers;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// Transactions view model.
	/// </summary>
	[Export(typeof(ITransactionsViewModel))]
	public class TransactionsViewModel : Screen, ITransactionsViewModel
	{
		#region Fields

		/// <summary>
		/// Transactions owner ID.
		/// </summary>
		private readonly Guid userId = new Guid("DC57BFF0-57A6-4BFC-9104-5F323ABBEDAB"); // 7F06BFA6-B675-483C-9BF3-F59B88230382

		/// <summary>
		/// Gets or sets <see cref="IAccountsViewModel"/>.
		/// </summary>
		private IAccountsViewModel accountsVM;

		/// <summary>
		/// Gets or sets <see cref="ITransactionDetailsViewModel"/>.
		/// </summary>
		private ITransactionDetailsViewModel transactionDetailsVM;

		/// <summary>
		/// Gets or sets <see cref="ITransferViewModel"/>.
		/// </summary>
		private ITransferViewModel transferVM;

		/// <summary>
		/// Corresponding account of transactions.
		/// </summary>
		private AccountDTO currentAccount;

		/// <summary>
		/// Gets or sets corresponding account of transactions.
		/// </summary>
		public AccountDTO CurrentAccount
		{
			get { return currentAccount; }
			private set
			{
				if (currentAccount != value)
				{
					currentAccount = value;
					NotifyOfPropertyChange(() => CurrentAccount);
					NotifyOfPropertyChange(() => CanDownloadAllTransactions);

					Coroutine.Execute(DownloadAllTransactions().GetEnumerator());
				}
			}
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionsViewModel"/> class.
		/// </summary>
		/// <param name="accountsVM">Accounts view model.</param>
		[ImportingConstructor]
		public TransactionsViewModel(IAccountsViewModel accountsVM, ITransactionDetailsViewModel transactionDetailsVM, ITransferViewModel transferVM)
		{
			TransactionRecords = new BindableCollection<TransactionRecord>();
			this.accountsVM = accountsVM;
			this.transactionDetailsVM = transactionDetailsVM;
			this.transferVM = transferVM;

			this.accountsVM.Accounts.CurrentChanged += (o, eventArgs) =>
			{
				if (!this.accountsVM.Accounts.IsEmpty)
				{
					CurrentAccount = this.accountsVM.Accounts.CurrentItem as AccountDTO;
				}
			};
		}

		#endregion

		#region Implementation of ITransactionsViewModel

		/// <summary>
		/// Gets transaction records.
		/// </summary>
		public IObservableCollection<TransactionRecord> TransactionRecords { get; private set; }

		/// <summary>
		/// Download all transactions for specific account of the specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public IEnumerable<IResult> DownloadAllTransactions()
		{
			yield return Loader.Show("Loading...");

			var request = new GetAllTransactionsResult(userId, CurrentAccount.Id);
			yield return request;

			TransactionRecords.Clear();
			TransactionRecords.AddRange(request.TransactionRecords);

			if (Reloaded != null)
			{
				Reloaded(this, EventArgs.Empty);
			}

			yield return Loader.Hide();
		}

		public bool CanDownloadAllTransactions
		{
			get
			{
				return CurrentAccount != null;
			}
		}

		/// <summary>
		/// Raised right after categories were reloaded from server.
		/// </summary>
		public event EventHandler<EventArgs> Reloaded;

		#endregion

		public IEnumerable<IResult> DeleteTransaction(TransactionRecord transactionRecord)
		{
			yield return Loader.Show("Deleting...");

			// Remove transaction on server
			var request = new DeleteTransactionResult(userId, CurrentAccount.Id, transactionRecord.TransactionId);
			yield return request;

			// Remove transaction locally
			var transactionToDelete = TransactionRecords.Where(record => record.TransactionId == transactionRecord.TransactionId).Single();
			var index = TransactionRecords.IndexOf(transactionToDelete);
			TransactionRecords.Remove(transactionToDelete);

			// Correct remained balance for following transactions
			if (TransactionRecords.Count > 0 && index < TransactionRecords.Count)
			{
				var deletedAmount = transactionToDelete.Income > 0
														? -transactionToDelete.Income
														: transactionToDelete.Expense;

				for (int i = index; i < TransactionRecords.Count; i++)
				{
					TransactionRecords[i].Balance += deletedAmount;
				}
			}

			yield return Loader.Hide();
		}

		public IEnumerable<IResult> EditTransaction(TransactionRecord transactionRecord)
		{
			yield return Loader.Show("Load transaction details...");

			// Remove transaction on server
			var request = new LoadTransactionResult(userId, CurrentAccount.Id, transactionRecord.TransactionId);
			yield return request;

			// Todo: use JournalType enumeration here instead of byte.
			switch (request.Transaction.JournalType)
			{
				case 1:
				case 2:
					transactionDetailsVM.Edit(request.Transaction);
					break;

				case 3:
					transferVM.Edit(request.Transaction);
					break;

				default:
					throw new NotSupportedException("Transaction with journal type " + request.Transaction.JournalType +
					                                " is not editable.");
			}

			yield return Loader.Hide();
		}
	}
}