//------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;

namespace Fab.Core
{
	/// <summary>
	/// Date and time extensions.
	/// </summary>
	public static class DateTimeExtensions
	{
		#region Constants

		/// <summary>
		/// Seconds in minute.
		/// </summary>
		private const int Minute = 60;

		/// <summary>
		/// Seconds in hour.
		/// </summary>
		private const int Hour = 60*Minute;

		/// <summary>
		/// Seconds in day.
		/// </summary>
		private const int Day = 24*Hour;

		/// <summary>
		/// Seconds in 30 days ("month").
		/// </summary>
		private const int Month = 30*Day;

		/// <summary>
		/// Seconds in 12*30 days ("year").
		/// </summary>
		private const int Year = 12*Month;

		#endregion


		/// <summary>
		/// Get friendly timespan representation like "58 minutes ago", "16 hours ago", "2 days ago" etc.
		/// </summary>
		/// <param name="fromDate">Start date to caclculate offset from. Usually <see cref="DateTime.UtcNow"/>.</param>
		/// <param name="toDate">End offset date. Usually taked database or other permanent storage.</param>
		/// <returns>Friendly timespan string.</returns>
		/// <note>
		/// Inspired from http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time/12#12.
		/// </note>
		public static string ToSmartTimespan(this DateTime fromDate, DateTime toDate)
		{
			var ts = new TimeSpan(Math.Abs(fromDate.Ticks - toDate.Ticks));
			double delta = Math.Abs(ts.TotalSeconds);

			if (delta < 5)
			{
				return "just now";
			}

			if (delta < Minute)
			{
				return ts.Seconds + " seconds ago";
			}

			if (delta < 2 * Minute)
			{
				return "a minute ago";
			}

			if (delta < Hour)
			{
				return ts.Minutes + " minutes ago";
			}

			if (delta < 2 * Hour)
			{
				return "an hour ago";
			}

			if (delta < Day)
			{
				return ts.Hours + " hours ago";
			}

			if (delta < 2 * Day)
			{
				return "yesterday";
			}

			if (delta < Month)
			{
				return ts.Days + " days ago";
			}

			if (delta < Year)
			{
				int months = Convert.ToInt32(Math.Floor(ts.Days / 30.0));
				return months <= 1 ? "a month ago" : months + " months ago";
			}

			int years = Convert.ToInt32(Math.Floor(ts.Days / 365.0));
			return years <= 1 ? "a year ago" : years + " years ago";
		}
	}
}
