// <copyright file="AsyncProgressIndicatorViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-08-31" />

using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Client.Shell.Async
{
	/// <summary>
	/// View model for "background operations in progress indicator" view.
	/// </summary>
	[Export(typeof(AsyncProgressIndicatorViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
	public class AsyncProgressIndicatorViewModel : Screen, IHandle<AsyncOperationStartedMessage>, IHandle<AsyncOperationCompleteMessage>
	{
		#region Has background operation

		/// <summary>
		/// A value indicating whether application have any background operation in progress.
		/// </summary>
		private bool hasBackgroundOperation;

		/// <summary>
		/// Gets or sets a value indicating whether application have any background operation in progress.
		/// </summary>
		public bool HasBackgroundOperation
		{
			get { return hasBackgroundOperation; }
			set
			{
				if (hasBackgroundOperation != value)
				{
					hasBackgroundOperation = value;
					NotifyOfPropertyChange(() => HasBackgroundOperation);
				}
			}
		}

		#endregion

		#region Background tasks count

		/// <summary>
		/// Count of the background operations.
		/// </summary>
		private int backgroundTasksCount;

		/// <summary>
		/// Gets or sets a value indicating whether application have any background operation in progress.
		/// </summary>
		public int BackgroundTasksCount
		{
			get { return backgroundTasksCount; }
			set
			{
				if (backgroundTasksCount != value)
				{
					backgroundTasksCount = value;
					NotifyOfPropertyChange(() => BackgroundTasksCount);
				}
			}
		}

		#endregion

		#region Last started operation name

		/// <summary>
		/// Last started background operation name.
		/// </summary>
		private string lastStartedOperation;

		/// <summary>
		/// Gets or sets last started background operation name.
		/// </summary>
		public string LastStartedOperation
		{
			get { return lastStartedOperation; }
			set
			{
				if (lastStartedOperation != value)
				{
					lastStartedOperation = value;
					NotifyOfPropertyChange(() => LastStartedOperation);
				}
			}
		}

		#endregion

		#region Private properties

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncProgressIndicatorViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public AsyncProgressIndicatorViewModel(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);
		}

		#endregion

		#region Implementation of IHandle<BackgroundOperationStartedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AsyncOperationStartedMessage message)
		{
			BackgroundTasksCount++;
			LastStartedOperation = message.OperationName;
			HasBackgroundOperation = BackgroundTasksCount != 0;
		}

		#endregion

		#region Implementation of IHandle<AsyncOperationCompleteMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AsyncOperationCompleteMessage message)
		{
			BackgroundTasksCount--;
			HasBackgroundOperation = BackgroundTasksCount != 0;

			if (!HasBackgroundOperation)
			{
				LastStartedOperation = string.Empty;
			}
		}

		#endregion
	}
}