//------------------------------------------------------------
// <copyright file="PostingRecordBase.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Postings
{
	/// <summary>
	/// Simple "income / expense" data object.
	/// </summary>
	public class PostingRecordBase : PropertyChangedBase
	{
		private CategoryDTO category;

		/// <summary>
		/// Gets or sets transaction category.
		/// </summary>
		public CategoryDTO Category
		{
			get { return category; }
			set {
				if (category != value)
				{
					category = value;
					NotifyOfPropertyChange(() => Category);
				}
			}
		}

		/// <summary>
		/// Gets or sets income part of the transaction record.
		/// </summary>
		public decimal Income { get; set; }

		/// <summary>
		/// Gets or sets expense part of the transaction record.
		/// </summary>
		public decimal Expense { get; set; }

		/// <summary>
		/// Gets or sets transaction optional comment.
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets original transaction data.
		/// </summary>
		public JournalDTO Journal { get; set; }

		/// <summary>
		/// Gets or sets unique (for account) transaction ID.
		/// </summary>
		public int TransactionId { get; set; }

		/// <summary>
		/// Gets or sets operation date.
		/// </summary>
		public DateTime Date { get; set; }
	}
}