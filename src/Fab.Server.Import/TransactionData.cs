// <copyright file="TransactionData.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-07-02" />
// <summary>Data object for imported transaction.</summary>

using System;

namespace Fab.Server.Import
{
	/// <summary>
	/// Data object for imported transaction.
	/// </summary>
	public class TransactionData
	{
		/// <summary>
		/// Gets or sets transaction date.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets associated category ID.
		/// </summary>
		public int CategoryId { get; set; }

		/// <summary>
		/// Gets or sets comment about operation.
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Gets or sets price information.
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// Gets or sets quantity information.
		/// </summary>
		public decimal Quantity { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a transaction is withdrawal (<c>true</c> value)
		/// or deposit (<c>false</c> value).
		/// </summary>
		public bool IsWithdrawal { get; set; }
	}
}