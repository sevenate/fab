// <copyright file="TransactionDTO.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-29" />
// <summary>Transaction data transfer object.</summary>

using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Transaction data transfer object.
	/// </summary>
	[DataContract]
	public abstract class TransactionDTO : JournalDTO
	{
		/// <summary>
		/// Gets or sets transaction category.
		/// </summary>
		[DataMember]
		public int? CategoryId { get; set; }
	}
}