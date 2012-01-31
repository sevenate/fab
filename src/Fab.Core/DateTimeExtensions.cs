using System;
using System.Collections.Generic;

namespace Fab.Core
{
	/// <summary>
	/// 
	/// </summary>
	/// <note>Original from http://stackoverflow.com/questions/11/how-do-i-calculate-relative-time/12#12.</note>
	public static class DateTimeExtensions
	{
		public static string ToSmartTimespan(this DateTime fromDate, DateTime toDate)
		{
			var thresholds = new Dictionary<long, string>();
			
			int minute = 60;
			int hour = 60 * minute;
			int day = 24 * hour;

			thresholds.Add(5, "just now");
			thresholds.Add(60, "{0} seconds ago");
			thresholds.Add(minute * 2, "a minute ago");
			thresholds.Add(45 * minute, "{0} minutes ago");
			thresholds.Add(120 * minute, "an hour ago");
			thresholds.Add(day, "{0} hours ago");
			thresholds.Add(day * 2, "yesterday");
			thresholds.Add(day * 30, "{0} days ago");
			thresholds.Add(day * 365, "{0} months ago");
			thresholds.Add(long.MaxValue, "{0} years ago");

			long since = (toDate.Ticks - fromDate.Ticks) / 10000000;
			
			foreach (long threshold in thresholds.Keys)
			{
				if (since < threshold)
				{
					var t = new TimeSpan((toDate.Ticks - fromDate.Ticks));
					return string.Format(thresholds[threshold],
						(t.Days > 365
							? t.Days / 365
							: (t.Days > 0
								? t.Days
								: (t.Hours > 0
									? t.Hours
									: (t.Minutes > 0
										? t.Minutes
										: (t.Seconds > 5
											? t.Seconds
											: (t.Seconds > 0
												? t.Seconds :
											0)))))).ToString());
				}
			}
			
			return "";
		}
	}
}
