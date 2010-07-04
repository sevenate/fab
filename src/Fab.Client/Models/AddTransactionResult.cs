﻿using System;
using System.ComponentModel;
using Caliburn.PresentationFramework.RoutedMessaging;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.Models
{
	public class AddTransactionResult : IResult
	{
		private readonly Guid userId;
		private readonly int accountId;
		private readonly DateTime operationDate;
		private readonly decimal price;
		private readonly decimal quantity;
		private readonly string comment;
		private readonly int? categoryId;
		private readonly bool isDeposit;

		public AddTransactionResult(Guid userId, int accountId, DateTime operationDate, decimal price, decimal quantity, string comment, int? categoryId, bool isDeposit)
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

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ResultExecutionContext context)
		{
			var proxy = new MoneyServiceClient();

			if (isDeposit)
			{
				proxy.DepositCompleted += OnSavingCompleted;
				proxy.DepositAsync(userId: userId,
									accountId: accountId,
									operationDate: operationDate,
									price: price,
									quantity: quantity,
									comment: comment,
									categoryId: categoryId
								  );
			}
			else
			{
				proxy.WithdrawalCompleted += OnSavingCompleted;
				proxy.WithdrawalAsync(userId: userId,
									accountId: accountId,
									operationDate: operationDate,
									price: price,
									quantity: quantity,
									comment: comment,
									categoryId: categoryId
								  );
			}
		}

		private void OnSavingCompleted(object s, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Caliburn.PresentationFramework.Invocation.Execute.OnUIThread(
					() => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
			}
			else
			{
				Caliburn.PresentationFramework.Invocation.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
			}
		}
	}
}