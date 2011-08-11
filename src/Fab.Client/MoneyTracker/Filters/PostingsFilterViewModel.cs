// <copyright file="PostingsFilterViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-05-12" />

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Client.MoneyTracker.Filters
{
	/// <summary>
	/// Filter for postings.
	/// </summary>
	[Export(typeof(PostingsFilterViewModel))]
	public class PostingsFilterViewModel : Screen
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
			DisplayName = "Postings Filter";
			fromDate = DateTime.Now.Date;
			tillDate = DateTime.Now.Date;
		}

		#endregion
	}
}