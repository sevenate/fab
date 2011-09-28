//------------------------------------------------------------
// <copyright file="CalendarLimitedSelectedRangeBehavior.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

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

		#region Selected Range

		/// <summary>
		/// Dependency property registration for <see cref="SelectedRange"/>.
		/// </summary>
		public static readonly DependencyProperty SelectedRangeProperty = DependencyProperty.RegisterAttached(
			"SelectedRange",
			typeof(Tuple<DateTime, DateTime>),
			typeof(CalendarLimitedSelectedRangeBehavior),
			new PropertyMetadata(OnSelectedRangeChanged));

		/// <summary>
		/// Flag that prevent recursive "OnSelectedRangeChanged" calls.
		/// </summary>
		private bool isUpdatingBack;

		/// <summary>
		/// <see cref="SelectedRangeProperty"/> changed event handler.
		/// </summary>
		/// <param name="o"><see cref="CalendarLimitedSelectedRangeBehavior"/> instance.</param>
		/// <param name="args">Event data.</param>
		private static void OnSelectedRangeChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
		{
			var b = (CalendarLimitedSelectedRangeBehavior)o;
			var newValue = (Tuple<DateTime, DateTime>)args.NewValue;

			if (!b.isUpdatingBack)
			{
				b.isUpdatingBack = true;

				b.AssociatedObject.SelectedDates.Clear();

				if (newValue.Item1.Date == newValue.Item2.Date)
				{
					b.AssociatedObject.SelectedDates.Add(newValue.Item1);
				}
				else
				{
					b.AssociatedObject.SelectedDates.AddRange(newValue.Item1, newValue.Item2);
				}

				b.isUpdatingBack = false;
			}
		}

		/// <summary>
		/// Gets or sets start (min) and end (max) date of the selected range.
		/// </summary>
		[Description("Gets or sets start (min) and end (max) date of the selected range.")]
		public Tuple<DateTime, DateTime> SelectedRange
		{
			get { return (Tuple<DateTime, DateTime>)GetValue(SelectedRangeProperty); }
			set { SetValue(SelectedRangeProperty, value); }
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
				
				isUpdating = false;
			}
			else if (!isUpdating && MaxSingleRangeLength == -1 && firstDate.AddMonths(1) < lastDate)
			{
				//"MaxSingleRangeLength == -1" is special "days count" that means "1 month"

				isUpdating = true;
				
				AssociatedObject.SelectedDates.Clear();
				maxEndDate = firstDate.AddMonths(1).AddDays(-1);
				
				AssociatedObject.SelectedDates.AddRange(firstDate, maxEndDate);
				
				isUpdating = false;
			}

			if (!isUpdating)
			{
				SelectedRange = Tuple.Create(firstDate, maxEndDate);
			}
		}

		#endregion
	}
}