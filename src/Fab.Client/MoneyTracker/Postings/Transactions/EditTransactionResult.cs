using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings.Transactions
{
	public class EditTransactionResult : IResult
	{
		private readonly int transactionId;
		private readonly int accountId;
		private readonly DateTime operationDate;
		private readonly decimal price;
		private readonly decimal quantity;
		private readonly string comment;
		private readonly int? categoryId;
		private readonly bool isDeposit;

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public EditTransactionResult(int transactionId, int accountId, DateTime operationDate, decimal price, decimal quantity, string comment, int? categoryId, bool isDeposit, IEventAggregator eventAggregator)
		{
			this.transactionId = transactionId;
			this.accountId = accountId;
			this.operationDate = operationDate;
			this.price = price;
			this.quantity = quantity;
			this.comment = comment;
			this.categoryId = categoryId;
			this.isDeposit = isDeposit;
			EventAggregator = eventAggregator;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.UpdateTransactionCompleted += OnUpdateTransactionCompleted;
			proxy.UpdateTransactionAsync(accountId,
										 transactionId,
										 isDeposit,
										 operationDate,
										 price,
										 quantity,
										 categoryId,
										 comment);
			
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Saving updated " + (isDeposit ? "income" : "expense") });
		}

		private void OnUpdateTransactionCompleted(object s, AsyncCompletedEventArgs e)
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