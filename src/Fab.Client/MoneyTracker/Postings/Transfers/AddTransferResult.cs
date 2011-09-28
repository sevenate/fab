using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Postings.Transfers
{
	public class AddTransferResult : IResult
	{
		private readonly int account1Id;
		private readonly int account2Id;
		private readonly decimal amount;
		private readonly string comment;
		private readonly DateTime operationDate;
		private readonly Guid user1Id;

		public AddTransferResult(Guid user1Id, int account1Id, int account2Id, DateTime operationDate, decimal amount,
		                         string comment)
		{
			this.user1Id = user1Id;
			this.account1Id = account1Id;
			this.account2Id = account2Id;
			this.operationDate = operationDate;
			this.amount = amount;
			this.comment = comment;
		}

		#region IResult Members

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();

			proxy.TransferCompleted += OnTransferCompleted;
			proxy.TransferAsync(user1Id,
			                    account1Id,
			                    account2Id,
			                    operationDate,
			                    amount,
			                    1, //TODO: add "quantity" param here
			                    comment
				);
		}

		#endregion

		private void OnTransferCompleted(object s, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Caliburn.Micro.Execute.OnUIThread(
					() => Completed(this, new ResultCompletionEventArgs
					                      {
					                      	Error = e.Error
					                      }));
			}
			else
			{
				Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
			}
		}
	}
}