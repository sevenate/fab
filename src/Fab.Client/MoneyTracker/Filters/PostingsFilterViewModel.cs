// <copyright file="PostingsFilterViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-05-12" />

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

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		private DateTime fromDate;

		/// <summary>
		/// Start date for filtering transactions.
		/// </summary>
		private DateTime tillDate;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets start date for filtering transactions.
		/// </summary>
		public DateTime FromDate
		{
			get { return fromDate; }
			set
			{
				if (fromDate != value)
				{
					fromDate = value;
					NotifyOfPropertyChange(() => FromDate);
					eventAggregator.Publish(new PostingsFilterUpdatedMessage
					{
						Start = fromDate,
						End = tillDate
					});
				}
			}
		}

		/// <summary>
		/// Gets or sets end date for filtering transactions.
		/// </summary>
		public DateTime TillDate
		{
			get { return tillDate; }
			set
			{
				if (tillDate != value)
				{
					tillDate = value;
					NotifyOfPropertyChange(() => TillDate);
					eventAggregator.Publish(new PostingsFilterUpdatedMessage
					{
						Start = fromDate,
						End = tillDate
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

		#region Methods

		/// <summary>
		/// Set <see cref="FromDate"/> and <see cref="TillDate"/> to current date.
		/// </summary>
		private void ResetToCurrentDate()
		{
			fromDate = DateTime.Now.Date;
			tillDate = DateTime.Now.Date;
			NotifyOfPropertyChange(() => FromDate);
			NotifyOfPropertyChange(() => TillDate);
		}

		#endregion
	}
}