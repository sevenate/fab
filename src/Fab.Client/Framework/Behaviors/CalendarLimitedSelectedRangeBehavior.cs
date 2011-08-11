// <copyright file="CalendarLimitedSelectedRangeBehavior.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-09" />

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Fab.Client.Framework.Behaviors
{
	/// <summary>
	/// Allow <see cref="Calendar"/> to select only limited range of dates.
	/// </summary>
	public class CalendarLimitedSelectedRangeBehavior : Behavior<Calendar>
	{
		#region Maximum Single Range Length

		/// <summary>
		/// Dependency property registration for <see cref="MaxSingleRangeLength"/>.
		/// </summary>
		public static readonly DependencyProperty MaxSingleRangeLengthProperty = DependencyProperty.RegisterAttached(
			"MaxSingleRangeLength",
			typeof (int),
			typeof (CalendarLimitedSelectedRangeBehavior),
			new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets maximum allowed single date range length (i.e. "number of days in period").
		/// </summary>
		[Description("Gets or sets maximum allowed single date range length (i.e. \"number of days in period\").")]
		public int MaxSingleRangeLength
		{
			get { return (int) GetValue(MaxSingleRangeLengthProperty); }
			set { SetValue(MaxSingleRangeLengthProperty, value); }
		}

		#endregion

		#region Start Date

		/// <summary>
		/// Dependency property registration for <see cref="StartDate"/>.
		/// </summary>
		public static readonly DependencyProperty StartDateProperty = DependencyProperty.RegisterAttached(
			"StartDate",
			typeof(DateTime),
			typeof(CalendarLimitedSelectedRangeBehavior),
			new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets start date of the selected range (i.e. "first day in period").
		/// </summary>
		[Description("Gets or sets start date of the selected range (i.e. \"first day in period\").")]
		public DateTime StartDate
		{
			get { return (DateTime)GetValue(StartDateProperty); }
			set { SetValue(StartDateProperty, value); }
		}

		#endregion

		#region End Date

		/// <summary>
		/// Dependency property registration for <see cref="EndDate"/>.
		/// </summary>
		public static readonly DependencyProperty EndDateProperty = DependencyProperty.RegisterAttached(
			"EndDate",
			typeof(DateTime),
			typeof(CalendarLimitedSelectedRangeBehavior),
			new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets end date of the selected range (i.e. "last day in period").
		/// </summary>
		[Description("Gets or sets end date of the selected range (i.e. \"last day in period\").")]
		public DateTime EndDate
		{
			get { return (DateTime)GetValue(EndDateProperty); }
			set { SetValue(EndDateProperty, value); }
		}

		#endregion

		#region Overrides of Behavior

		/// <summary>
		/// Called after the behavior is attached to an AssociatedObject.
		/// </summary>
		/// <remarks>
		/// Override this to hook up functionality to the AssociatedObject.
		/// </remarks>
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.SelectedDatesChanged += OnSelectedDatesChanged;
		}

		/// <summary>
		/// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		/// <remarks>
		/// Override this to unhook functionality from the AssociatedObject.
		/// </remarks>
		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.SelectedDatesChanged -= OnSelectedDatesChanged;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Flag that prevent recursive "selection changed" calls.
		/// </summary>
		private bool isUpdating;

		/// <summary>
		/// Handle <see cref="Calendar.SelectedDates"/> event to restrict selected range length.
		/// </summary>
		/// <param name="sender">Calendar element.</param>
		/// <param name="selectionChangedEventArgs">Event data.</param>
		private void OnSelectedDatesChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			// Not applicable to other selection modes
			if (AssociatedObject.SelectionMode != CalendarSelectionMode.SingleRange
			    || AssociatedObject.SelectedDates.Count == 0)
			{
				return;
			}

			var firstDate = AssociatedObject.SelectedDates.Min();
			var lastDate = AssociatedObject.SelectedDates.Max();
			var maxEndDate = lastDate;
			var currentLength = (lastDate - firstDate).Days;

			// Restrict selection range to MaxSingleRangeLength days
			if (!isUpdating && MaxSingleRangeLength != -1 && currentLength > MaxSingleRangeLength)
			{
				isUpdating = true;
				
				AssociatedObject.SelectedDates.Clear();
				maxEndDate = firstDate.AddDays(MaxSingleRangeLength - 1);
				
				AssociatedObject.SelectedDates.AddRange(firstDate, maxEndDate);
//				StartDate = firstDate;
//				EndDate = maxEndDate;
				
				isUpdating = false;
			}
			else if (!isUpdating && MaxSingleRangeLength == -1 && firstDate.AddMonths(1) < lastDate)
			{
				//"MaxSingleRangeLength == -1" is special "days count" that means "1 month"

				isUpdating = true;
				
				AssociatedObject.SelectedDates.Clear();
				maxEndDate = firstDate.AddMonths(1).AddDays(-1);
				
				AssociatedObject.SelectedDates.AddRange(firstDate, maxEndDate);
//				StartDate = firstDate;
//				EndDate = maxEndDate;
				
				isUpdating = false;
			}

			if (!isUpdating)
			{
				if (StartDate != firstDate)
				{
					StartDate = firstDate;
				}

				if (EndDate != maxEndDate)
				{
					EndDate = maxEndDate;
				}
			}
		}

		#endregion
	}
}