//------------------------------------------------------------
// <copyright file="PostingRecord.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

namespace Fab.Client.MoneyTracker.Postings
{
	/// <summary>
	/// Simple "income / expense / balance" data object.
	/// </summary>
	public class PostingRecord : PostingRecordBase
	{
		/// <summary>
		/// Gets or sets balance after the transaction record.
		/// </summary>
		public decimal Balance { get; set; }
	}
}