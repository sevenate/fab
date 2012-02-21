using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories;

namespace Fab.Client.MoneyTracker.Postings
{
	public class AddTransactionRecordResult : AddTransactionRecordBaseResult
	{
		public decimal Balance { get; private set; }

		public AddTransactionRecordResult(JournalDTO journal, ICategoriesRepository repository, decimal prevBalance)
			: base(journal, repository)
		{
			Balance = prevBalance;
		}

		#region Overrides of AddTransactionRecordBaseResult

		protected override void InitializeTransactionRecorcd()
		{
			base.InitializeTransactionRecorcd();
			Balance += Journal.Amount;
			((PostingRecord) TransactionRecord).Balance = Balance;
		}

		#endregion
	}
}