//------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Xunit;

namespace Fab.Core.Tests
{
	/// <summary>
	/// Unit tests for <see cref="DateTimeExtensions"/> class.
	/// </summary>
	public class DateTimeExtensionsTests
	{
		[Fact]
// ReSharper disable InconsistentNaming
		public void JustNow_For_0_Sec()
// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate;
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("just now", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void JustNow_For_1_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddSeconds(1);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("just now", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void JustNow_For_4_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 59);
			var toDate = fromDate.AddSeconds(4);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("just now", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void FiveSecondsAgo_For_5_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 59);
			var toDate = fromDate.AddSeconds(5);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("5 seconds ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void FiftyNineSecondsAgo_For_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("59 seconds ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AMinuteAgo_For_1_Min()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddMinutes(1);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a minute ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AMinuteAgo_For_1_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddMinutes(1).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a minute ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoMinutesAgo_For_2_Min()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddMinutes(2);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 minutes ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void FiftyNineMinutesAgo_For_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddMinutes(59).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("59 minutes ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AnHourAgo_For_1_Hour()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddHours(1);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("an hour ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AnHourAgo_For_1_Hour_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddHours(1).AddMinutes(59).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("an hour ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoHoursAgo_For_2_Hours()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddHours(2);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 hours ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwentyThreeHoursAgo_For_23_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddHours(23).AddMinutes(59).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("23 hours ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void Yesterday_For_1_Day()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(1);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("yesterday", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void Yesterday_For_1_Day_23_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(1).AddHours(23).AddMinutes(59).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("yesterday", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoDaysAgo_For_2_Days()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(2);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 days ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwentyNineDaysAgo_For_29_Days_23_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(29).AddHours(23).AddMinutes(59).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("29 days ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AMonthAgo_For_30_Days()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(30);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a month ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AMonthAgo_For_59_Days_23_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(59).AddHours(23).AddMinutes(59).AddSeconds(59);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a month ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoMonthsAgo_For_60_Days()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(60);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 months ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void ElevenMonthsAgo_For_12x30_Days_Minus_1_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(12*30).AddSeconds(-1);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("11 months ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AYearAgo_For_12x30_Days()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(12 * 30);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a year ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AYearAgo_For_2x365_Days_Minus_1_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(2*365).AddSeconds(-1);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a year ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoYearsAgo_For_2x365_Days()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = fromDate.AddDays(2*365);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 years ago", smartTimespan);
		}
	}
}