using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings.Transfers
{
	public class AddTransferResult : IResult
	{
		private readonly int account1Id;
		private readonly int account2Id;
		private readonly decimal amount;
		private readonly string comment;
		private readonly DateTime operationDate;

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public AddTransferResult(int account1Id, int account2Id, DateTime operationDate, decimal amount,
								 string comment, IEventAggregator eventAggregator)
		{
			this.account1Id = account1Id;
			this.account2Id = account2Id;
			this.operationDate = operationDate;
			this.amount = amount;
			this.comment = comment;
			EventAggregator = eventAggregator;
		}

		#region IResult Members

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.TransferCompleted += OnTransferCompleted;
			proxy.TransferAsync(account1Id,
			                    account2Id,
			                    operationDate,
			                    amount,
			                    1, //TODO: add "quantity" param here
			                    comment
				);

			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Saving new transfer" });
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

			EventAggregator.Publish(new AsyncOperationCompleteMessage());
		}
	}
}