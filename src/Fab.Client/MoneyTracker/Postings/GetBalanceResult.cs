using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings
{
	public class GetBalanceResult : IResult
	{
		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public GetBalanceResult(Guid userId, int accountId, DateTime date, IEventAggregator eventAggregator)
		{
			UserId = userId;
			AccountId = accountId;
			Date = date;
			EventAggregator = eventAggregator;
		}

		public Guid UserId { get; private set; }

		public int AccountId { get; private set; }

		public DateTime Date { get; private set; }

		public decimal Balance { get; set; }

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.GetAccountBalanceCompleted += (s, e) =>
			{
				if (e.Error != null)
				{
					Caliburn.Micro.Execute.OnUIThread(
						() => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
				}
				else
				{
					Balance = e.Result;
					Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.GetAccountBalanceAsync(UserId, AccountId, Date);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Get account #" + AccountId + " balance" });
		}
	}
}