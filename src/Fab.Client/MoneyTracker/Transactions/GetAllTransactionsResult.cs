using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transactions
{
	public class GetAllTransactionsResult : IResult
	{
		public GetAllTransactionsResult(Guid userId, int accountId)
		{
			UserId = userId;
			AccountId = accountId;
		}

		public Guid UserId { get; private set; }

		public int AccountId { get; private set; }

		public TransactionRecord[] TransactionRecords { get; set; }

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();
			
			proxy.GetAllTransactionsCompleted += (s, e) =>
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

			proxy.GetAllTransactionsAsync(UserId, AccountId);
		}
	}
}