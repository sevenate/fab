using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Postings
{
	public class GetPostingsResult : IResult
	{
		public GetPostingsResult(Guid userId, int accountId, QueryFilter queryFilter)
		{
			UserId = userId;
			AccountId = accountId;
			QueryFilter = queryFilter;
		}

		public Guid UserId { get; private set; }

		public int AccountId { get; private set; }

		public QueryFilter QueryFilter { get; private set; }

		public JournalDTO[] TransactionRecords { get; set; }

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();
			
			proxy.GetJournalsCompleted += (s, e) =>
			{
				if (e.Error != null)
				{
					Caliburn.Micro.Execute.OnUIThread(
						() => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
				}
				else
				{
					TransactionRecords = e.Result;
					Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
				}
			};

			proxy.GetJournalsAsync(UserId, AccountId, QueryFilter);
		}
	}
}