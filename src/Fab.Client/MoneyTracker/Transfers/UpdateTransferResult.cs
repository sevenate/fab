using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transfers
{
	public class UpdateTransferResult : IResult
	{
		private readonly int transactionId;
		private readonly Guid user1Id;
		private readonly int account1Id;
		private readonly int account2Id;
		private readonly DateTime operationDate;
		private readonly decimal amount;
		private readonly string comment;

		public UpdateTransferResult(int transactionId, Guid user1Id, int account1Id, int account2Id, DateTime operationDate, decimal amount, string comment)
		{
			this.transactionId = transactionId;
			this.user1Id = user1Id;
			this.account1Id = account1Id;
			this.account2Id = account2Id;
			this.operationDate = operationDate;
			this.amount = amount;
			this.comment = comment;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();
			proxy.UpdateTransferCompleted += OnUpdateTransferCompleted;
			proxy.UpdateTransferAsync(user1Id,
									  transactionId,
									  account1Id,
									  account2Id,
									  operationDate,
									  amount,
									  1,	//TODO: add "quantity" parameter here
									  comment
								);
		}

		private void OnUpdateTransferCompleted(object s, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Caliburn.Micro.Execute.OnUIThread(
					() => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
			}
			else
			{
				Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
			}
		}
	}
}