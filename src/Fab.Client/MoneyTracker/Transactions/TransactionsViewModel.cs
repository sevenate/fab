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
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;
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

		private ICategoriesRepository categoriesRepository;

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
		[ImportingConstructor]
		public TransactionsViewModel(IEventAggregator eventAggregator, ICategoriesRepository categoriesRepository, ITransactionDetailsViewModel transactionDetailsVM, ITransferViewModel transferVM)
		{
			TransactionRecords = new BindableCollection<TransactionRecord>();

			Intervals = new EnumWrapperList<DateRange>();
			fromDate = DateTime.Now.Date;
			selectedDateRange = DateRange.Day;
			UpdateTillDate(false);

			this.categoriesRepository = categoriesRepository;
			this.transactionDetailsVM = transactionDetailsVM;
			this.transferVM = transferVM;

			eventAggregator.Subscribe(this);
		}

		#endregion

		#region Implementation of ITransactionsViewModel

		/// <summary>
		/// Gets transaction records.
		/// </summary>
		public IObservableCollection<TransactionRecord> TransactionRecords { get; private set; }

		/// <summary>
		/// Account balance at the <see cref="FromDate"/> moment.
		/// </summary>
		private decimal startBalance;

		/// <summary>
		/// Gets or sets account balance at the <see cref="FromDate"/> moment.
		/// </summary>
		public decimal StartBalance
		{
			get { return startBalance; }
			set
			{
				startBalance = value;
				NotifyOfPropertyChange(() => StartBalance);
			}
		}

		/// <summary>
		/// Account balance at the <see cref="TillDate"/> moment.
		/// </summary>
		private decimal endBalance;

		/// <summary>
		/// Gets or sets account balance at the <see cref="TillDate"/> moment.
		/// </summary>
		public decimal EndBalance
		{
			get { return endBalance; }
			set
			{
				endBalance = value;
				NotifyOfPropertyChange(() => EndBalance);
			}
		}

		/// <summary>
		/// Total income for the filtered period.
		/// </summary>
		private decimal totalIncome;

		/// <summary>
		/// Gets or sets total income for the filtered period.
		/// </summary>
		public decimal TotalIncome
		{
			get { return totalIncome; }
			set
			{
				totalIncome = value;
				NotifyOfPropertyChange(() => TotalIncome);
			}
		}

		/// <summary>
		/// Total expense for the filtered period.
		/// </summary>
		private decimal totalExpense;

		/// <summary>
		/// Gets or sets total expense for the filtered period.
		/// </summary>
		public decimal TotalExpense
		{
			get { return totalExpense; }
			set
			{
				totalExpense = value;
				NotifyOfPropertyChange(() => TotalExpense);
			}
		}

		/// <summary>
		/// Account balance difference for the filtered period.
		/// </summary>
		private decimal balanceDiff;

		/// <summary>
		/// Gets or sets account balance difference for the filtered period.
		/// </summary>
		public decimal BalanceDiff
		{
			get { return balanceDiff; }
			set
			{
				balanceDiff = value;
				NotifyOfPropertyChange(() => BalanceDiff);
			}
		}

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		private DateTime fromDate;

		/// <summary>
		/// Gets or sets start date for filtering transactions.
		/// </summary>
		public DateTime FromDate
		{
			get { return fromDate; }
			set
			{
				fromDate = value;
				UpdateTillDate(true);
				NotifyOfPropertyChange(() => FromDate);
				NotifyOfPropertyChange(() => Period);
			}
		}

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		private DateTime tillDate;

		/// <summary>
		/// Gets or sets end date for filtering transactions.
		/// </summary>
		public DateTime TillDate
		{
			get { return tillDate; }
			set
			{
				tillDate = value;
				NotifyOfPropertyChange(() => TillDate);
				NotifyOfPropertyChange(() => Period);
			}
		}

		/// <summary>
		/// Gets or sets filter date period text ("FromDate - TillDate" or "FromDate" only).
		/// </summary>
		public string Period
		{
			get
			{
				return FromDate.Date == TillDate.Date
						? FromDate.Date.ToLongDateString()
						: FromDate.Date.ToLongDateString() + " - " + TillDate.Date.ToLongDateString();
			}
		}

		/// <summary>
		/// Gets available date range values to specify end date for filtering transactions.
		/// </summary>
		public EnumWrapperList<DateRange> Intervals { get; private set; }

		/// <summary>
		/// Selected date range for filtering transactions.
		/// </summary>
		private DateRange selectedDateRange;

		/// <summary>
		/// Gets or sets selected date range for filtering transactions.
		/// </summary>
		public DateRange SelectedDateRange
		{
			get { return selectedDateRange; }
			set
			{
				selectedDateRange = value;
				UpdateTillDate(true);
				NotifyOfPropertyChange(() => SelectedDateRange);
			}
		}

		private void UpdateTillDate(bool reloadTransactions)
		{
			switch (SelectedDateRange)
			{
				case DateRange.Day:
					TillDate = FromDate.AddDays(1).AddMilliseconds(-1);
					break;

				case DateRange.FourDays:
					TillDate = FromDate.AddDays(4).AddMilliseconds(-1);
					break;

				case DateRange.Week:
					TillDate = FromDate.AddDays(7).AddMilliseconds(-1);
					break;

				case DateRange.Month:
					TillDate = FromDate.AddMonths(1).AddMilliseconds(-1);
					break;
			}

			if (reloadTransactions)
			{
				Coroutine.Execute(DownloadAllTransactions().GetEnumerator());
			}
		}

		/// <summary>
		/// Download all transactions for specific account of the specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public IEnumerable<IResult> DownloadAllTransactions()
		{
			yield return Loader.Show("Loading...");

			// Determine previous account balance
			var balanceResult = new GetBalanceResult(UserCredentials.Current.UserId, CurrentAccount.Id, FromDate.ToUniversalTime());
			yield return balanceResult;

			StartBalance = balanceResult.Balance;

			// Get filtered transactions during specified time frame
			var queryFilterDTO = new QueryFilter
			                     	{
			                     		NotOlderThen = FromDate.ToUniversalTime(),
										Upto = TillDate.AddMilliseconds(1).ToUniversalTime(),
			                     	};
			var transactionsResult = new GetTransactionsResult(UserCredentials.Current.UserId, CurrentAccount.Id, queryFilterDTO);
			yield return transactionsResult;

			TransactionRecords.Clear();

			decimal income = 0;
			decimal expense = 0;
			decimal incomeForPeriod = 0;
			decimal expenseForPeriod = 0;
			decimal balance = balanceResult.Balance;
			CategoryDTO category = null;

			foreach (var r in transactionsResult.TransactionRecords)
			{
				balance += r.Amount;

				if (r is DepositDTO)
				{
					income = r.Amount;
					incomeForPeriod += r.Amount;
					expense = 0;
					category = (r as DepositDTO).CategoryId.LookupIn(categoriesRepository);
				}
				else if (r is WithdrawalDTO)
				{
					income = 0;
					expense = -r.Amount;
					expenseForPeriod += r.Amount;
					category = (r as WithdrawalDTO).CategoryId.LookupIn(categoriesRepository);
				}
				else if (r is IncomingTransferDTO)
				{
					income = r.Amount;		// positive is "TO this account"
					incomeForPeriod += r.Amount;
					expense = 0;
				}
				else if (r is OutgoingTransferDTO)
				{
					income = 0;
					expense = -r.Amount;	// negative is "FROM this account"
					expenseForPeriod += r.Amount;
				}

				TransactionRecords.Add(new TransactionRecord
				{
					TransactionId = r.Id,
					Date = DateTime.SpecifyKind(r.Date, DateTimeKind.Utc),
					Category = category,
					Income = income,
					Expense = expense,
					Balance = balance,
					Comment = r.Comment
				});
			}

			EndBalance = balance;
			TotalIncome = incomeForPeriod;
			TotalExpense = expenseForPeriod;
			BalanceDiff = incomeForPeriod + expenseForPeriod;

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
			var request = new DeleteTransactionResult(UserCredentials.Current.UserId, CurrentAccount.Id, transactionRecord.TransactionId);
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
			var request = new LoadTransactionResult(UserCredentials.Current.UserId, CurrentAccount.Id, transactionRecord.TransactionId);
			yield return request;

			if (request.Transaction is TransactionDTO)
			{
				transactionDetailsVM.Edit(request.Transaction as TransactionDTO, CurrentAccount.Id);
			}
			else if (request.Transaction is TransferDTO)
			{
				transferVM.Edit(request.Transaction as TransferDTO, currentAccount.Id);
			}
			else
			{
				throw new NotSupportedException("Transaction of type " + request.Transaction.GetType() +
												" is not editable.");
			}

			yield return Loader.Hide();
		}

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the <see cref="LoggedOutMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="LoggedOutMessage"/>.</param>
		public void Handle(LoggedOutMessage message)
		{
			TransactionRecords.Clear();
			CurrentAccount = null;
			StartBalance = 0;
			EndBalance = 0;
			TotalIncome = 0;
			TotalExpense = 0;
			BalanceDiff = 0;
			FromDate = DateTime.UtcNow;
			TillDate = DateTime.UtcNow;
			SelectedDateRange = DateRange.Day;
		}

		#endregion

		#region Implementation of IHandle<in CurrentAccountChangedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CurrentAccountChangedMessage message)
		{
			//TODO: consider remove condition here.
			if (message.CurrentAccount != null)
			{
				CurrentAccount = message.CurrentAccount;
			}
		}

		#endregion
	}
}