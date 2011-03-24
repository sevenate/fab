using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.TransactionDetails
{
	public class EditTransactionResult : IResult
	{
		private readonly int transactionId;
		private readonly Guid userId;
		private readonly int accountId;
		private readonly DateTime operationDate;
		private readonly decimal price;
		private readonly decimal quantity;
		private readonly string comment;
		private readonly int? categoryId;
		private readonly bool isDeposit;

		public EditTransactionResult(int transactionId, Guid userId, int accountId, DateTime operationDate, decimal price, decimal quantity, string comment, int? categoryId, bool isDeposit)
		{
			this.transactionId = transactionId;
			this.userId = userId;
			this.accountId = accountId;
			this.operationDate = operationDate;
			this.price = price;
			this.quantity = quantity;
			this.comment = comment;
			this.categoryId = categoryId;
			this.isDeposit = isDeposit;
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();
			proxy.UpdateTransactionCompleted += OnUpdateTransactionCompleted;
			proxy.UpdateTransactionAsync(userId,
										 accountId,
										 transactionId,
										 isDeposit,
										 operationDate,
										 price,
										 quantity,
										 categoryId,
										 comment);
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
		}
	}
}