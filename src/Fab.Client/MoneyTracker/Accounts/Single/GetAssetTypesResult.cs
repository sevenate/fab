using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Accounts.Single
{
	public class GetAssetTypesResult : IResult
	{
		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		public GetAssetTypesResult(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
		}

		public AssetTypeDTO[] Assets { get; set; }

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();
			
			proxy.GetAllAssetTypesCompleted += (s, e) =>
			{
				if (e.Error != null)
				{
					Caliburn.Micro.Execute.OnUIThread(
						() => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
				}
				else
				{
					Assets = e.Result;
					Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
				}
				
				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.GetAllAssetTypesAsync();
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Downloading asset types"});
		}
	}
}