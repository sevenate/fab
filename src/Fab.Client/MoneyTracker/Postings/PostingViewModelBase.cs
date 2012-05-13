//------------------------------------------------------------
// <copyright file="PostingViewModelBase.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.Framework.Results;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;
using Fab.Client.MoneyTracker.Postings.Transactions;
using Fab.Client.MoneyTracker.Postings.Transfers;
using Fab.Core.Framework;

namespace Fab.Client.MoneyTracker.Postings
{
	public class PostingViewModelBase : LocalizableConductor<IPostingPanel>,
										ICanBeBusy,
										IHandle<CategoryDeletedMessage>,
										IHandle<AccountUpdatedMessage>
	{
		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		protected readonly IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
		protected readonly ICategoriesRepository categoriesRepository = IoC.Get<ICategoriesRepository>();
		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();

		#region Properties

		private string searchStatus = Resources.Strings.PostingViewModelBase_SearchStatus_Search;

		public string SearchStatus
		{
			get { return searchStatus; }
			set
			{
				searchStatus = value;
				NotifyOfPropertyChange(() => SearchStatus);
			}
		}

		private string containsText;

		public string ContainsText
		{
			get { return containsText; }
			set
			{
				containsText = value;
				NotifyOfPropertyChange(() => ContainsText);
			}
		}

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		protected DateTime startDate;

		public DateTime StartDate
		{
			get { return startDate; }
			set
			{
				startDate = value;
				NotifyOfPropertyChange(() => StartDate);
			}
		}

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		protected DateTime endDate;

		public DateTime EndDate
		{
			get { return endDate; }
			set
			{
				endDate = value;
				NotifyOfPropertyChange(() => EndDate);
			}
		}

		private bool useEndDate;

		public bool UseEndDate
		{
			get { return useEndDate; }
			set
			{
				useEndDate = value;
				NotifyOfPropertyChange(() => UseEndDate);
			}
		}

		private bool useStartDate;

		public bool UseStartDate
		{
			get { return useStartDate; }

			set
			{
				useStartDate = value;
				NotifyOfPropertyChange(() => UseStartDate);
			}
		}


		/// <summary>
		/// Account ID of transactions.
		/// </summary>
		private int accountId;

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
				NotifyOfPropertyChange(() => BalanceDiff);
				NotifyOfPropertyChange(() => EndBalance);
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
				NotifyOfPropertyChange(() => BalanceDiff);
				NotifyOfPropertyChange(() => EndBalance);
			}
		}

		/// <summary>
		/// Indicate whether postings are outdated for current filter period and need to be downloaded from server.
		/// </summary>
		private bool isOutdated = true;

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

		/// <summary>
		/// Account balance at the <see cref="startDate"/> moment.
		/// </summary>
		private decimal startBalance;

		/// <summary>
		/// Gets or sets account balance at the <see cref="startDate"/> moment.
		/// </summary>
		public decimal StartBalance
		{
			get { return startBalance; }
			set
			{
				startBalance = value;
				NotifyOfPropertyChange(() => StartBalance);
				NotifyOfPropertyChange(() => EndBalance);
			}
		}

		/// <summary>
		/// Gets or sets account balance difference for the filtered period.
		/// </summary>
		public decimal BalanceDiff
		{
			get { return TotalIncome + TotalExpense; }
		}

		/// <summary>
		/// Gets or sets account balance at the <see cref="endDate"/> moment.
		/// </summary>
		public decimal EndBalance
		{
			get { return StartBalance + BalanceDiff; }
		}

		/// <summary>
		/// Gets transaction records.
		/// </summary>
		public IObservableCollection<PostingRecordBase> TransactionRecords { get; private set; }

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

		public TransactionViewModel TransactionDetails { get; set; }
		public TransferViewModel TransferDetails { get; set; }

		[Import]
		public IDialogManager Dialogs { get; set; }

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Caliburn.Micro.Conductor`1.Collection.OneActive"/> class.
		/// </summary>
		[ImportingConstructor]
		public PostingViewModelBase(TransactionViewModel transactionDetails, TransferViewModel transferDetails)
		{
			TransactionDetails = transactionDetails;
			TransferDetails = transferDetails;
			Init();
			eventAggregator.Subscribe(this);
			Translator.CultureChanged += (sender, args) => NotifyOfPropertyChange(() => SearchStatus);
		}

		protected void Init()
		{
			if (TransactionRecords == null)
			{
				TransactionRecords = new BindableCollection<PostingRecordBase>();
			}
			else
			{
				TransactionRecords.Clear();
			}

			StartDate = DateTime.Now.Date;
			EndDate = StartDate;
			UseStartDate = false;
			UseEndDate = false;
			ContainsText = string.Empty;
			TotalIncome = 0;
			TotalExpense = 0;
			TotalPostingsCount = 0;
		}

		protected virtual IEnumerable<IResult> PreAction()
		{
			yield break;
		}

		protected virtual AddTransactionRecordBaseResult CreateTransactionRecordResult(JournalDTO r)
		{
			return new AddTransactionRecordBaseResult(r, categoriesRepository);
		}

		/// <summary>
		/// Download all transactions for specific account of the specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		/// TODO: Sometimes "from-till" dates with 1 month interval doesn't equal, for example:
		/// "2 March - 2 April", but in most cases correct should be "2 March - 1 April"
		public IEnumerable<IResult> DownloadAllTransactions()
		{
			if (IsBusy)
			{
				yield break;
			}

			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					SearchStatus = Resources.Strings.PostingViewModelBase_SearchStatus_Searching;
				}
			};

			yield return Loader.Show(Resources.Strings.Loader_Loading);

			yield return new SequentialResult(PreAction().GetEnumerator());

			yield return new SequentialResult(DeterminePagesCount().GetEnumerator());

			yield return new SequentialResult(LoadPostings().GetEnumerator());

			SearchStatus = Resources.Strings.PostingViewModelBase_SearchStatus_Search;
			IsBusy = false;
			IsOutdated = false;

			yield return Loader.Hide();
		}

		private IEnumerable<IResult> DeterminePagesCount()
		{
			var countResult = new CountPostingsResult(UserCredentials.Current.UserId, AccountId, CreateFilter(), eventAggregator);
			yield return countResult;

			yield return new SingleResult
				        {
				            Action = () =>
				             		    {

											if (countResult.Count >= 1)
				             		        {
				             		         	TotalPostingsCount = countResult.Count;
				             		         	PagesCount = PageSize.HasValue
				             		         			        ? countResult.Count/PageSize.Value +
				             		         			            (countResult.Count%PageSize.Value > 0 ? 1 : 0)
				             		         			        : 1;
				             		         	CurrentPageIndex = PagesCount > 0 ? 1 : 0;
				             		        }
											else
											{
												TotalPostingsCount = 0;
												PagesCount = 0;
												CurrentPageIndex = 0;
											}
				             		    }
				        };
		}

		private IEnumerable<IResult> LoadPostings()
		{
			// Get filtered transactions during specified time frame
			var transactionsResult = new GetPostingsResult(UserCredentials.Current.UserId, AccountId, CreateFilter(PageSize), eventAggregator);
			yield return transactionsResult;

			yield return new SingleResult
			{
				Action = () =>
				{
					TotalIncome = 0;
					TotalExpense = 0;
					TransactionRecords.Clear();
				}
			};

			foreach (var r in transactionsResult.TransactionRecords)
			{
				var result = CreateTransactionRecordResult(r);
				yield return result;

				PostAction(result);
			}
		}

		protected virtual void PostAction(AddTransactionRecordBaseResult result)
		{
			TotalIncome += result.Income;
			TotalExpense -= result.Expense;
			TransactionRecords.Add(result.TransactionRecord);
		}

		private TextSearchFilter CreateFilter(int? itemsPerPage = null)
		{
			var filter = new TextSearchFilter
			             	{
			             		NotOlderThen = UseStartDate
												? StartDate.Date.ToUniversalTime()
			             		               	: (DateTime?)null,
			             		Upto = UseEndDate
										? EndDate.Date.AddDays(1).ToUniversalTime()
			             		       	: (DateTime?)null,
			             		Contains = !string.IsNullOrEmpty(ContainsText)
			             		           	? ContainsText
			             		           	: null,
			             		Take = itemsPerPage,
			             		Skip = itemsPerPage != null ? (CurrentPageIndex - 1) * itemsPerPage : null,
			             	};
			return filter;
		}

		#region Paging

		private int currentPageIndex;

		public int CurrentPageIndex
		{
			get { return currentPageIndex; }
			set
			{
				currentPageIndex = value;
				NotifyOfPropertyChange(() => CurrentPageIndex);
				NotifyOfPropertyChange(() => CanNextPage);
				NotifyOfPropertyChange(() => CanPrevPage);
			}
		}

		private int? pageSize;
		public int? PageSize
		{
			get { return pageSize; }
			set
			{
				pageSize = value;
				NotifyOfPropertyChange(() => PageSize);
			}
		}

		private int pagesCount;
		public int PagesCount
		{
			get { return pagesCount; }
			set
			{
				pagesCount = value;
				NotifyOfPropertyChange(() => PagesCount);
			}
		}

		private int totalPostingsCount;

		public int TotalPostingsCount
		{
			get { return totalPostingsCount; }
			set
			{
				totalPostingsCount = value;
				NotifyOfPropertyChange(() => TotalPostingsCount);
			}
		}

		public IEnumerable<IResult> NextPage()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					CurrentPageIndex++;
				}
			};

			yield return new SequentialResult(DownloadAllTransactions().GetEnumerator());

			IsBusy = false;
		}

		public bool CanNextPage
		{
			get { return CurrentPageIndex < PagesCount; }
		}

		public IEnumerable<IResult> PrevPage()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					CurrentPageIndex--;
				}
			};

			yield return new SequentialResult(DownloadAllTransactions().GetEnumerator());

			IsBusy = false;
		}

		public bool CanPrevPage
		{
			get { return 1 < CurrentPageIndex; }
		}

		#endregion

		#region Implementation of ICanBeBusy

		/// <summary>
		/// Indicate whether view-model is busy by some background operation.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether view-model is busy by some background operation.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
			}
		}

		#endregion

		#region Implementation of IHandle<AccountUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountUpdatedMessage message)
		{
			if (message.Account.Id == AccountId)
			{
				IsOutdated = true;
				Update();
			}
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
				if (transactionRecord.Category != null && transactionRecord.Category.Id == message.Category.Id)
				{
					transactionRecord.Category = null;
				}
			}
		}

		#endregion

		public override void CanClose(Action<bool> callback)
		{
			callback(IsDirty);
		}

		public virtual void CancelEdit()
		{
			ActivateItem(null);
		}

		public IEnumerable<IResult> EditPosting(PostingRecord transactionRecord)
		{
			if (transactionRecord.Journal is TransactionDTO)
			{
				TransactionDetails.Edit(transactionRecord.Journal as TransactionDTO, AccountId);
				ActivateItem(TransactionDetails);
			}
			else if (transactionRecord.Journal is TransferDTO)
			{
				var transferResult = new GetPostingResult(UserCredentials.Current.UserId, AccountId, transactionRecord.TransactionId, eventAggregator);
				yield return transferResult;

				var transfer = transferResult.Transaction as TransferDTO;
				TransferDetails.Edit(transfer, AccountId);
				ActivateItem(TransferDetails);
			}
			else
			{
				throw new NotSupportedException(string.Format("Transaction of type {0} is not editable.", transactionRecord.Journal.GetType()));
			}
		}

		public IEnumerable<IResult> DeleteTransaction(PostingRecord transactionRecord)
		{
			var openConfirmationResult = new OpenConfirmationResult(eventAggregator)
			                             	{
			                             		Message =
													string.Format(Resources.Strings.PostingViewModelBase_DeleteConfirmation_Message, transactionRecord.TransactionId),
												Title = Resources.Strings.PostingViewModelBase_DeleteConfirmation_Title,
			                             		Options = MessageBoxOptions.Yes | MessageBoxOptions.Cancel,
			                             	};

			yield return openConfirmationResult;

			if (openConfirmationResult.Selected == MessageBoxOptions.Yes)
			{
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
						((PostingRecord) TransactionRecords[i]).Balance += deletedAmount;
					}
				}
			}
		}


		/// <summary>
		/// Download postings if they are outdated for current filter period.
		/// </summary>
		public void Update()
		{
			if (IsOutdated)
			{
				Coroutine.BeginExecute(DownloadAllTransactions().GetEnumerator());
			}
		}
	}
}