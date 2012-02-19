using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories;

namespace Fab.Client.MoneyTracker.Postings
{
	public class AddTransactionRecordResult : AddTransactionRecordBaseResult
	{
		public AddTransactionRecordResult(JournalDTO journal, ICategoriesRepository repository, decimal prevBalance)
			: base(journal, repository)
		{
			((PostingRecord)TransactionRecord).Balance = prevBalance;
		}

		#region Overrides of AddTransactionRecordBaseResult

		protected override void InitializeTransactionRecorcd()
		{
			base.InitializeTransactionRecorcd();
			((PostingRecord)TransactionRecord).Balance += Journal.Amount;
		}

		#endregion
	}
}