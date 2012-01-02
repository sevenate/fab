// <copyright file="PostingsFilterUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-05-13" />

using System;

namespace Fab.Client.MoneyTracker.Filters
{
	/// <summary>
	/// Send by <see cref="PostingsFilterViewModel"/> after filtered period is changed.
	/// </summary>
	public class PostingsFilterUpdatedMessage
	{
		/// <summary>
		/// Gets or sets start date of the filtered period of time.
		/// </summary>
		public DateTime Start { get; set; }
		
		/// <summary>
		/// Gets or sets end date of the filtered period of time.
		/// </summary>
		public DateTime End { get; set; }
	}
}