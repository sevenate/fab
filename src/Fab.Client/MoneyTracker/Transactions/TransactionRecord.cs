// <copyright file="TransactionRecord.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-16" />
// <summary>Simple "income / expense / balance" data object.</summary>

using System;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transactions
{
	/// <summary>
	/// Simple "income / expense / balance" data object.
	/// </summary>
	public class TransactionRecord
	{
		/// <summary>
		/// Gets or sets unique (for account) transaction ID.
		/// </summary>
		public int TransactionId { get; set; }

		/// <summary>
		/// Gets or sets operation date.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets transaction category.
		/// </summary>
		public CategoryDTO Category { get; set; }

		/// <summary>
		/// Gets or sets income part of the transaction record.
		/// </summary>
		public decimal Income { get; set; }

		/// <summary>
		/// Gets or sets expense part of the transaction record.
		/// </summary>
		public decimal Expense { get; set; }

		/// <summary>
		/// Gets or sets balance part of the transaction record.
		/// </summary>
		public decimal Balance { get; set; }

		/// <summary>
		/// Gets or sets transaction optional comment.
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets original transaction data.
		/// </summary>
		public JournalDTO Journal { get; set; }
	}
}