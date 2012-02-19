//------------------------------------------------------------
// <copyright file="PostingsFilterViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.MoneyTracker.Filters
{
	/// <summary>
	/// Filter for postings.
	/// </summary>
	[Export(typeof(PostingsFilterViewModel))]
	public class PostingsFilterViewModel : Screen, IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
	{
		#region Fields

		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator;

		#endregion

		#region Selected range

		/// <summary>
		/// Start (Item1) and end (Item2) dates for filtering transactions.
		/// </summary>
		private Tuple<DateTime, DateTime> selectedRange;

		/// <summary>
		/// Gets or sets start (Item1) and end (Item2) dates for filtering transactions.
		/// </summary>
		public Tuple<DateTime, DateTime> SelectedRange
		{
			get { return selectedRange; }
			set
			{
				if (selectedRange != value)
				{
					selectedRange = value;
					NotifyOfPropertyChange(() => SelectedRange);

					eventAggregator.Publish(new PostingsFilterUpdatedMessage
					                        	{
					                        		Start = selectedRange.Item1 < selectedRange.Item2
					                        		        	? selectedRange.Item1
					                        		        	: selectedRange.Item2,
					                        		End = selectedRange.Item1 >= selectedRange.Item2
					                        		      	? selectedRange.Item1
					                        		      	: selectedRange.Item2,
					                        	});
				}
			}
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="PostingsFilterViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">
		/// The event Aggregator.
		/// </param>
		[ImportingConstructor]
		public PostingsFilterViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			this.eventAggregator.Subscribe(this);
			ResetToCurrentDate();
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets or Sets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return "Postings Filter"; }
		}

		#endregion

		#region Implementation of IHandle<in LoggedInMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(LoggedInMessage message)
		{
			ResetToCurrentDate();
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(LoggedOutMessage message)
		{
			ResetToCurrentDate();
		}

		#endregion

		#region Actions

		/// <summary>
		/// Set current range to current day.
		/// </summary>
		public void Today()
		{
			ResetToCurrentDate();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Set <see cref="SelectedRange"/> to current date.
		/// </summary>
		private void ResetToCurrentDate()
		{
			var initDate = DateTime.Now.Date;
			SelectedRange = Tuple.Create(initDate, initDate);
		}

		#endregion
	}
}