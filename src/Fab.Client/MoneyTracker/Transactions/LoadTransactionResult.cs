// <copyright file="EditTransactionResult.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-06-12" />
// <summary>Load specific transaction async result.</summary>

using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// Load specific transaction async result.
	/// </summary>
	public class LoadTransactionResult : IResult
	{
		private readonly int accountId;
		private readonly int transactionId;
		private readonly Guid userId;

		public LoadTransactionResult(Guid userId, int accountId, int transactionId)
		{
			this.userId = userId;
			this.accountId = accountId;
			this.transactionId = transactionId;
		}

		public JournalDTO Transaction { get; set; }

		#region IResult Members

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();

			proxy.GetJournalCompleted += (sender, args) =>
			                             {
			                             	if (args.Error != null)
			                             	{
			                             		Caliburn.Micro.Execute.OnUIThread(
			                             			() => Completed(this, new ResultCompletionEventArgs
			                             			                      {
			                             			                      	Error = args.Error
			                             			                      }));
			                             	}
			                             	else
			                             	{
			                             		Transaction = args.Result;
			                             		Caliburn.Micro.Execute.OnUIThread(
			                             			() => Completed(this, new ResultCompletionEventArgs()));
			                             	}
			                             };
			proxy.GetJournalAsync(userId,
			                      accountId,
			                      transactionId);
		}

		#endregion
	}
}