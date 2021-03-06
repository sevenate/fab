﻿using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings.Transactions
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

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public AddTransactionResult(int accountId, DateTime operationDate, decimal price, decimal quantity,
									string comment, int? categoryId, bool isDeposit, IEventAggregator eventAggregator)
		{
			this.accountId = accountId;
			this.operationDate = operationDate;
			this.price = price;
			this.quantity = quantity;
			this.comment = comment;
			this.categoryId = categoryId;
			this.isDeposit = isDeposit;
			EventAggregator = eventAggregator;
		}

		#region IResult Members

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateMoneyService();

			if (isDeposit)
			{
				proxy.DepositCompleted += OnSavingCompleted;
				proxy.DepositAsync(accountId, operationDate, price, quantity, categoryId, comment);
			}
			else
			{
				proxy.WithdrawalCompleted += OnSavingCompleted;
				proxy.WithdrawalAsync(accountId, operationDate, price, quantity, categoryId, comment);
			}

			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Saving new " + (isDeposit ? "income" : "expense") });
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

			EventAggregator.Publish(new AsyncOperationCompleteMessage());
		}
	}
}