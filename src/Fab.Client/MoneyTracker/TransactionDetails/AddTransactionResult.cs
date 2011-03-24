using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.TransactionDetails
{
	public class AddTransactionResult : IResult
	{
		private readonly int accountId;
		private readonly int? categoryId;
		private readonly string comment;
		private readonly bool isDeposit;
		private readonly DateTime operationDate;
		private readonly decimal price;
		private readonly decimal quantity;
		private readonly Guid userId;

		public AddTransactionResult(Guid userId, int accountId, DateTime operationDate, decimal price, decimal quantity,
		                            string comment, int? categoryId, bool isDeposit)
		{
			this.userId = userId;
			this.accountId = accountId;
			this.operationDate = operationDate;
			this.price = price;
			this.quantity = quantity;
			this.comment = comment;
			this.categoryId = categoryId;
			this.isDeposit = isDeposit;
		}

		#region IResult Members

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();

			if (isDeposit)
			{
				proxy.DepositCompleted += OnSavingCompleted;
				proxy.DepositAsync(userId, accountId, operationDate, price, quantity, categoryId, comment);
			}
			else
			{
				proxy.WithdrawalCompleted += OnSavingCompleted;
				proxy.WithdrawalAsync(userId, accountId, operationDate, price, quantity, categoryId, comment);
			}
		}

		#endregion

		private void OnSavingCompleted(object s, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Caliburn.Micro.Execute.OnUIThread(
					() => Completed(this, new ResultCompletionEventArgs {Error = e.Error}));
			}
			else
			{
				Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
			}
		}
	}
}