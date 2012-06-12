using System;
using System.ComponentModel;
using Caliburn.Micro;
using Fab.Client.Shell;

namespace Fab.Client.MoneyTracker.Postings
{
	public class DeleteTransactionResult : IResult
	{
		private readonly int accountId;
		private readonly int transactionId;

		public DeleteTransactionResult(int accountId, int transactionId)
		{
			this.accountId = accountId;
			this.transactionId = transactionId;
		}

		#region IResult Members

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.DeleteJournalCompleted += OnDeleteCompleted;
			proxy.DeleteJournalAsync(accountId, transactionId);
		}

		#endregion

		private void OnDeleteCompleted(object s, AsyncCompletedEventArgs e)
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