using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories;
using Fab.Core;

namespace Fab.Client.MoneyTracker.Postings
{
	public class AddTransactionRecordBaseResult : IResult
	{
		protected readonly JournalDTO Journal;
		private readonly ICategoriesRepository categoriesRepository;

		public AddTransactionRecordBaseResult(JournalDTO journal, ICategoriesRepository repository)
		{
			Journal = journal;
			categoriesRepository = repository;
			TransactionRecord = new PostingRecord();
		}

		public PostingRecordBase TransactionRecord { get; private set; }
		public decimal Income { get; private set; }
		public decimal Expense { get; private set; }

		public void Execute(ActionExecutionContext context)
		{
			InitializeTransactionRecorcd();
			Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
		}

		protected virtual void InitializeTransactionRecorcd()
		{
			CategoryDTO category = null;

			if (Journal is DepositDTO)
			{
				Income = Journal.Amount;
				Expense = 0;
				category = (Journal as DepositDTO).CategoryId.LookupIn(categoriesRepository);
			}
			else if (Journal is WithdrawalDTO)
			{
				Income = 0;
				Expense = -Journal.Amount;
				category = (Journal as WithdrawalDTO).CategoryId.LookupIn(categoriesRepository);
			}
			else if (Journal is IncomingTransferDTO)
			{
				Income = Journal.Amount; // positive is "TO this account"
				Expense = 0;
			}
			else if (Journal is OutgoingTransferDTO)
			{
				Income = 0;
				Expense = -Journal.Amount; // negative is "FROM this account"
			}

			TransactionRecord.TransactionId = Journal.Id;
			TransactionRecord.Date = Journal.Date.IsUtc();
			TransactionRecord.Category = category;
			TransactionRecord.Income = Income;
			TransactionRecord.Expense = Expense;
			TransactionRecord.Journal = Journal;
			TransactionRecord.Comment = Journal.Comment;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
	}
}