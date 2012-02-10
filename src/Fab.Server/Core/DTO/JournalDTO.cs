// <copyright file="JournalDTO.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-29" />
// <summary>Journal data transfer object.</summary>

using System;
using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Journal record data transfer object.
	/// </summary>
	[DataContract]
	public abstract class JournalDTO
	{
		/// <summary>
		/// Gets or sets journal ID.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets posting date.
		/// </summary>
		[DataMember]
		public DateTime Date { get; set; }

		/// <summary>
		/// Gets or sets rate.
		/// </summary>
		[DataMember]
		public decimal Rate { get; set; }

		/// <summary>
		/// Gets or sets quantity.
		/// </summary>
		[DataMember]
		public decimal Quantity { get; set; }

		/// <summary>
		/// Gets or sets amount.
		/// </summary>
		// TODO: Consider implement as calculated property = Rate * Quantity.
		[DataMember]
		public decimal Amount { get; set; }

		/// <summary>
		/// Gets or sets comment.
		/// </summary>
		[DataMember]
		public string Comment { get; set; }
	}
}