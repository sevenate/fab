//------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Xunit;

namespace Fab.Core.Tests
{
	public class DateTimeExtensionsTests
	{
		[Fact]
// ReSharper disable InconsistentNaming
		public void JustNow_For_0_Sec()
// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("just now", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void JustNow_For_1_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 7, 9);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("just now", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void JustNow_For_4_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 59);
			var toDate = new DateTime(2012, 1, 31, 18, 8, 3);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("just now", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void FiveSecondsAgo_For_5_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 59);
			var toDate = new DateTime(2012, 1, 31, 18, 8, 4);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("5 seconds ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void FiftyNineSecondsAgo_For_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 8, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("59 seconds ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AMinuteAgo_For_60_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 8, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a minute ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AMinuteAgo_For_119_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 9, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a minute ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoMinutesAgo_For_120_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 9, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 minutes ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void FortyFourMinutesAgo_For_44_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 52, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("44 minutes ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AnHourAgo_For_45_Min()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 18, 52, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("an hour ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void AnHourAgo_For_119_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 20, 7, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("an hour ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoHoursAgo_For_120_Min()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 1, 31, 20, 7, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 hours ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwentyThreeHoursAgo_For_23_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 2, 1, 18, 7, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("23 hours ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void Yesterday_For_24_Hours()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 2, 1, 18, 7, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("yesterday", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void Yesterday_For_47_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 2, 2, 18, 7, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("yesterday", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwoDaysAgo_For_48_Hours()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 1, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 2, 2, 18, 7, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("2 days ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void TwentyNineDaysAgo_For_29_Days_23_Hours_59_Min_59_Sec()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 4, 30, 18, 7, 7);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("29 days ago", smartTimespan);
		}

		[Fact]
		// ReSharper disable InconsistentNaming
		public void ThirtyDaysAgo_For_30_Days()
		// ReSharper restore InconsistentNaming
		{
			var fromDate = new DateTime(2012, 3, 31, 18, 7, 8);
			var toDate = new DateTime(2012, 4, 30, 18, 7, 8);
			var smartTimespan = fromDate.ToSmartTimespan(toDate);
			Assert.Equal("a month ago", smartTimespan);
		}
	}
}