// <copyright file="TransactionsViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2010-04-10" />

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
using Fab.Client.MoneyTracker.Postings.Actions;
using Fab.Client.MoneyTracker.Postings.Transactions;
using Fab.Client.MoneyTracker.Postings.Transfers;

namespace Fab.Client.MoneyTracker.Postings
{
	/// <summary>
	/// Postings view model.
	/// </summary>
	[Export(typeof (PostingsViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class PostingsViewModel : Conductor<IPostingPanel>.Collection.OneActive, IHandle<CategoryDeletedMessage>
	{
		#region Fields

		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();
		private readonly ICategoriesRepository categoriesRepository = IoC.Get<ICategoriesRepository>();

		/// <summary>
		/// Account ID of transactions.
		/// </summary>
		private int accountId;

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

		/// <summary>
		/// Indicate whether postings are outdated for current filter period and need to be downloaded from server.
		/// </summary>
		private bool isOutdated = true;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets account ID of transactions.
		/// </summary>
		public int AccountId
		{
			get { return accountId; }
			set
			{
				if (accountId != value)
				{
					accountId = value;
					IsOutdated = true;
				}
			}
		}

		/// <summary>
		/// Gets transaction records.
		/// </summary>
		public IObservableCollection<PostingRecord> TransactionRecords { get; private set; }

		/// <summary>
		/// Gets or sets account balance at the <see cref="fromDate"/> moment.
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
		/// Gets or sets account balance at the <see cref="tillDate"/> moment.
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

		public TransactionViewModel TransactionDetails { get; set; }

		public TransferViewModel TransferDetails { get; set; }

		public PostingActionsViewModel PostingsActions { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether postings are outdated for current filter period.
		/// </summary>
		public bool IsOutdated
		{
			get { return isOutdated; }
			set
			{
				if (isOutdated != value)
				{
					isOutdated = value;
					NotifyOfPropertyChange(() => IsOutdated);
				}
			}
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="PostingsViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public PostingsViewModel(PostingActionsViewModel postingsActions, TransactionViewModel transactionDetails,
		                         TransferViewModel transferDetails)
		{
			TransactionRecords = new BindableCollection<PostingRecord>();

			PostingsActions = postingsActions;
			TransactionDetails = transactionDetails;
			TransferDetails = transferDetails;

			ActivationProcessed += (sender, args) => { IsDirty = (args.Item != PostingsActions); };

			fromDate = DateTime.Now.Date;
			tillDate = DateTime.Now.Date;

			eventAggregator.Subscribe(this);
		}

		#endregion

		#region DocumentBase

		private bool isDirty;

		public bool IsDirty
		{
			get { return isDirty; }
			set
			{
				isDirty = value;
				NotifyOfPropertyChange(() => IsDirty);
			}
		}

		[Import]
		public IDialogManager Dialogs { get; set; }

		public override void CanClose(Action<bool> callback)
		{
			callback(IsDirty);
		}

		public void CancelEdit()
		{
			ActivateItem(PostingsActions);
		}

		#endregion

		#region Overrides of ViewAware

		/// <summary>
		/// Called when an attached view's Loaded event fires.
		/// </summary>
		/// <param name="view"/>
		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);

			if (ActiveItem == null)
			{
				ActivateItem(PostingsActions);
			}
		}

		#endregion

		#region Overrides of OneActive

		/// <summary>
		/// Called when activating.
		/// </summary>
		protected override void OnActivate()
		{
			base.OnActivate();
			Update();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Open dialog for creating new transaction.
		/// </summary>
		public void NewTransaction()
		{
			TransactionDetails.CategoriesSource = IoC.Get<ICategoriesRepository>().Entities;
			TransactionDetails.Create(AccountId);
			ActivateItem(TransactionDetails);
		}

		/// <summary>
		/// Open dialog for creating new transfer.
		/// </summary>
		public void NewTransfer()
		{
			TransferDetails.Create(AccountId);
			ActivateItem(TransferDetails);
		}

		public void EditPosting(PostingRecord transactionRecord)
		{
			if (transactionRecord.Journal is TransactionDTO)
			{
				TransactionDetails.CategoriesSource = IoC.Get<ICategoriesRepository>().Entities;
				TransactionDetails.Edit(transactionRecord.Journal as TransactionDTO, AccountId);
				ActivateItem(TransactionDetails);
			}
			else if (transactionRecord.Journal is TransferDTO)
			{
				TransferDetails.Edit(transactionRecord.Journal as TransferDTO, AccountId);
				ActivateItem(TransferDetails);
			}
			else
			{
				throw new NotSupportedException("Transaction of type " + transactionRecord.Journal.GetType() + " is not editable.");
			}
		}

		public IEnumerable<IResult> DeleteTransaction(PostingRecord transactionRecord)
		{
			var openConfirmationResult = new OpenConfirmationResult(eventAggregator)
			                             {
			                             	Message =
			                             		"Do you really want to delete the selected posting #" +
			                             		transactionRecord.TransactionId + " ?",
			                             	Title = "Confirmation",
			                             	Options = MessageBoxOptions.Yes | MessageBoxOptions.Cancel,
			                             };

			yield return openConfirmationResult;

			if (openConfirmationResult.Selected == MessageBoxOptions.Yes)
			{
				yield return Loader.Show("Deleting...");

				// Load transaction from server (used below to determine if the deleted posting was transfer)
				var request = new GetPostingResult(UserCredentials.Current.UserId, AccountId, transactionRecord.TransactionId, eventAggregator);
				yield return request;

				// Remove transaction on server
				var request2 = new DeleteTransactionResult(UserCredentials.Current.UserId, AccountId,
				                                           transactionRecord.TransactionId);
				yield return request2;

				// Update accounts balance
				accountsRepository.Download(AccountId);

				// Update category usage
				if (transactionRecord.Category != null)
				{
					categoriesRepository.Download(transactionRecord.Category.Id);
				}

				// For transfer the 2-nd account should also be updated
				if (request.Transaction is TransferDTO)
				{
					int secondAccountId = ((TransferDTO) request.Transaction).SecondAccountId.Value;
					accountsRepository.Download(secondAccountId);
				}

				// Remove transaction locally
				var transactionToDelete =
					TransactionRecords.Where(record => record.TransactionId == transactionRecord.TransactionId).Single();
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

		public void SetFilterPeriod(DateTime startDate, DateTime endDate)
		{
			fromDate = startDate;
			tillDate = endDate;
			IsOutdated = true;
			NotifyOfPropertyChange(() => Period);
		}

		/// <summary>
		/// Download postings if they are outdated for current filter period.
		/// </summary>
		public void Update()
		{
			if (isOutdated)
			{
				Coroutine.BeginExecute(DownloadAllTransactions().GetEnumerator());
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
			var balanceResult = new GetBalanceResult(UserCredentials.Current.UserId, AccountId, fromDate.ToUniversalTime(), eventAggregator);
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
			var transactionsResult = new GetPostingsResult(UserCredentials.Current.UserId, AccountId, queryFilterDTO, eventAggregator);
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
					income = r.Amount; // positive is "TO this account"
					incomeForPeriod += r.Amount;
					expense = 0;
				}
				else if (r is OutgoingTransferDTO)
				{
					income = 0;
					expense = -r.Amount; // negative is "FROM this account"
					expenseForPeriod += r.Amount;
				}

				TransactionRecords.Add(new PostingRecord
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

			IsOutdated = false;

			yield return Loader.Hide();
		}

		#endregion

		#region Implementation of IHandle<CategoryDeletedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoryDeletedMessage message)
		{
			foreach (var transactionRecord in TransactionRecords)
			{
				if(transactionRecord.Category != null && transactionRecord.Category.Id == message.Category.Id)
				{
					transactionRecord.Category = null;
				}
			}
		}

		#endregion
	}
}