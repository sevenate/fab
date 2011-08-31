// <copyright file="EditTransactionResult.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-06-12" />
// <summary>Load specific transaction async result.</summary>

using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Postings
{
	/// <summary>
	/// Load specific transaction async result.
	/// </summary>
	public class GetPostingResult : IResult
	{
		private readonly int accountId;
		private readonly int transactionId;
		private readonly Guid userId;

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public GetPostingResult(Guid userId, int accountId, int transactionId, IEventAggregator eventAggregator)
		{
			this.userId = userId;
			this.accountId = accountId;
			this.transactionId = transactionId;
			EventAggregator = eventAggregator;
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

											EventAggregator.Publish(new AsyncOperationCompleteMessage());
			                             };
			proxy.GetJournalAsync(userId,
			                      accountId,
			                      transactionId);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Downloading posting #" + transactionId });
		}

		#endregion
	}
}