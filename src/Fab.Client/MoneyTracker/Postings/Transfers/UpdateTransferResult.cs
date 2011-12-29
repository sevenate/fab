using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings.Transfers
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

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public UpdateTransferResult(int transactionId, Guid user1Id, int account1Id, int account2Id, DateTime operationDate, decimal amount, string comment, IEventAggregator eventAggregator)
		{
			this.transactionId = transactionId;
			this.user1Id = user1Id;
			this.account1Id = account1Id;
			this.account2Id = account2Id;
			this.operationDate = operationDate;
			this.amount = amount;
			this.comment = comment;
			EventAggregator = eventAggregator;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateMoneyService();
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

			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Saving updated transfer" });
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

			EventAggregator.Publish(new AsyncOperationCompleteMessage());
		}
	}
}