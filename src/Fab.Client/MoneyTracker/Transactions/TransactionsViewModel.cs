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
using Fab.Client.MoneyTracker.Accounts.Single;
using Fab.Client.MoneyTracker.Categories;
using Fab.Client.MoneyTracker.Filters;
using Fab.Client.MoneyTracker.TransactionDetails;
using Fab.Client.MoneyTracker.Transfers;
using Fab.Client.Shell;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// Transactions view model.
	/// </summary>
	[Export(typeof(TransactionsViewModel))]
	public class TransactionsViewModel : Conductor<TransactionDetailsViewModel>.Collection.OneActive, IHandle<LoggedOutMessage>, IHandle<CurrentAccountChangedMessage>, IHandle<PostingsFilterUpdatedMessage>
	{
		#region Fields

		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();
		private readonly ICategoriesRepository categoriesRepository = IoC.Get<ICategoriesRepository>();

		/// <summary>
		/// Corresponding account of transactions.
		/// </summary>
		private AccountViewModel currentAccount;

		/// <summary>
		/// Account balance at the <see cref="fromDate"/> moment.
		/// </summary>
		private decimal startBalance;

		/// <summary>
		/// Account balance at the <see cref="tillDate"/> moment.
		/// </summary>
		private decimal endBalance;

		/// <summary>
		/// Total income for the filtered period.
		/// </summary>
		private decimal totalIncome;

		/// <summary>
		/// Total expense for the filtered period.
		/// </summary>
		private decimal totalExpense;

		/// <summary>
		/// Account balance difference for the filtered period.
		/// </summary>
		private decimal balanceDiff;

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		private DateTime fromDate;

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		private DateTime tillDate;

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionsViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public TransactionsViewModel()
		{
			TransactionRecords = new BindableCollection<TransactionRecord>();

			fromDate = DateTime.Now.Date;
			tillDate = DateTime.Now.Date;

			eventAggregator.Subscribe(this);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets corresponding account of transactions.
		/// </summary>
		public AccountViewModel CurrentAccount
		{
			get { return currentAccount; }
			set
			{
				if (currentAccount != value)
				{
					currentAccount = value;
					NotifyOfPropertyChange(() => CurrentAccount);

					Coroutine.BeginExecute(DownloadAllTransactions().GetEnumerator());
				}
			}
		}

		/// <summary>
		/// Gets transaction records.
		/// </summary>
		public IObservableCollection<TransactionRecord> TransactionRecords { get; private set; }

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
		/// Gets filter date period text ("FromDate - TillDate" or "FromDate" only).
		/// </summary>
		public string Period
		{
			get
			{
				return fromDate.Date == tillDate.Date
						? fromDate.Date.ToLongDateString()
						: fromDate.Date.ToLongDateString() + " - " + tillDate.Date.ToLongDateString();
			}
		}

		[Import]
		public IDialogManager Dialogs { get; set; }

		[Import]
		public TransactionDetailsViewModel TransactionDetails { get; set; }

		#endregion

		#region Methods

		public void NewTransaction()
		{
			TransactionDetails.Create(CurrentAccount);
			TransactionDetails.Parent = this;
			ActivateItem(TransactionDetails);
			//Dialogs.ShowDialog(TransactionDetails);
		}

		public void NewTransfer()
		{
			var transferVM = IoC.Get<TransferViewModel>();
			transferVM.Create(CurrentAccount);
			/*eventAggregator.Publish(new OpenDialogMessage
			                        {
			                        	Dialog = transferVM
			                        });*/

			Dialogs.ShowDialog(transferVM);
		}

		public void EditTransaction(TransactionRecord transactionRecord)
		{
			if (transactionRecord.Journal is TransactionDTO)
			{
				var transactionDetailsVM = IoC.Get<TransactionDetailsViewModel>();
				transactionDetailsVM.Edit(transactionRecord.Journal as TransactionDTO, CurrentAccount);
				eventAggregator.Publish(new OpenDialogMessage
				{
					Dialog = transactionDetailsVM
				});
			}
			else if (transactionRecord.Journal is TransferDTO)
			{
				var transferVM = IoC.Get<TransferViewModel>();
				transferVM.Edit(transactionRecord.Journal as TransferDTO, CurrentAccount);
				eventAggregator.Publish(new OpenDialogMessage
				{
					Dialog = transferVM
				});
			}
			else
			{
				throw new NotSupportedException("Transaction of type " + transactionRecord.Journal.GetType() + " is not editable.");
			}
		}

		public IEnumerable<IResult> DeleteTransaction(TransactionRecord transactionRecord)
		{
			var openConfirmationResult = new OpenConfirmationResult(eventAggregator)
			                             {
			                             	Message = "Do you really want to delete the selected posting #" + transactionRecord.TransactionId + " ?",
			                             	Title = "Confirmation",
			                             	Options = MessageBoxOptions.Yes | MessageBoxOptions.Cancel,
			                             };

			yield return openConfirmationResult;

			if (openConfirmationResult.Selected == MessageBoxOptions.Yes)
			{
				yield return Loader.Show("Deleting...");

				// Load transaction from server (used below to determine if the deleted posting was transfer)
				var request = new LoadTransactionResult(UserCredentials.Current.UserId, CurrentAccount.Id, transactionRecord.TransactionId);
				yield return request;

				// Remove transaction on server
				var request2 = new DeleteTransactionResult(UserCredentials.Current.UserId, CurrentAccount.Id, transactionRecord.TransactionId);
				yield return request2;

				// Update accounts balance
				accountsRepository.Download(CurrentAccount.Id);

				// For transfer the 2-nd account should also be updated
				if (request.Transaction is TransferDTO)
				{
					int secondAccountId = ((TransferDTO) request.Transaction).SecondAccountId.Value;
					accountsRepository.Download(secondAccountId);
				}

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
		}

//		private void UpdateTillDate(bool reloadTransactions)
//		{
//			switch (SelectedDateRange)
//			{
//				case DateRange.Day:
//					TillDate = FromDate.AddDays(1).AddMilliseconds(-1);
//					break;
//
//				case DateRange.FourDays:
//					TillDate = FromDate.AddDays(4).AddMilliseconds(-1);
//					break;
//
//				case DateRange.Week:
//					TillDate = FromDate.AddDays(7).AddMilliseconds(-1);
//					break;
//
//				case DateRange.Month:
//					TillDate = FromDate.AddMonths(1).AddMilliseconds(-1);
//					break;
//			}
//
//			if (reloadTransactions)
//			{
//				Coroutine.Execute(DownloadAllTransactions().GetEnumerator());
//			}
//		}

		/// <summary>
		/// Download all transactions for specific account of the specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public IEnumerable<IResult> DownloadAllTransactions()
		{
			yield return Loader.Show("Loading...");

			// Determine previous account balance
			var balanceResult = new GetBalanceResult(UserCredentials.Current.UserId, CurrentAccount.Id, fromDate.ToUniversalTime());
			yield return balanceResult;

			StartBalance = balanceResult.Balance;

			//TODO: ###########################
			// Выяснить, почему иногда from-till даты с интервалом в месяц не совпадают, например:
			// 2 марта - 2 апреля, хотя в большинстве случаях правильно должно быть 2 марта - 1 апреля

			// И удалить все старое, что осталось от интервалов (оставляем только календарик)
			// ################################

			// Get filtered transactions during specified time frame
			var queryFilterDTO = new QueryFilter
			                     {
			                     	NotOlderThen = fromDate.ToUniversalTime(),
			                     	Upto = tillDate.AddDays(1) /*.AddMilliseconds(1)*/.ToUniversalTime(),
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
				                       	Comment = r.Comment,
										Journal = r
				                       });
			}

			EndBalance = balance;
			TotalIncome = incomeForPeriod;
			TotalExpense = expenseForPeriod;
			BalanceDiff = incomeForPeriod + expenseForPeriod;

			yield return Loader.Hide();
		}

		#endregion

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
			fromDate = DateTime.UtcNow;
			tillDate = DateTime.UtcNow;
		}

		#endregion

		#region Implementation of IHandle<in CurrentAccountChangedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CurrentAccountChangedMessage message)
		{
			CurrentAccount = message.CurrentAccount;
		}

		#endregion

		#region Implementation of IHandle<in PostingsFilterUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(PostingsFilterUpdatedMessage message)
		{
			fromDate = message.Start;
			tillDate = message.End;
			NotifyOfPropertyChange(() => Period);
			Coroutine.BeginExecute(DownloadAllTransactions().GetEnumerator());
		}

		#endregion
	}
}