using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings
{
	public class GetPostingsResult : IResult
	{
		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public GetPostingsResult(Guid userId, int accountId, QueryFilter queryFilter, IEventAggregator eventAggregator)
		{
			UserId = userId;
			AccountId = accountId;
			QueryFilter = queryFilter;
			EventAggregator = eventAggregator;
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
				
				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.GetJournalsAsync(UserId, AccountId, QueryFilter);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Downloading postings for account #" + AccountId});
		}
	}
}