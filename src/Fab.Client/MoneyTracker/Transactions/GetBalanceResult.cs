using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transactions
{
	public class GetBalanceResult : IResult
	{
		public GetBalanceResult(Guid userId, int accountId, DateTime date)
		{
			UserId = userId;
			AccountId = accountId;
			Date = date;
		}

		public Guid UserId { get; private set; }

		public int AccountId { get; private set; }

		public DateTime Date { get; private set; }

		public decimal Balance { get; set; }

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();

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
			};

			proxy.GetAccountBalanceAsync(UserId, AccountId, Date);
		}
	}
}