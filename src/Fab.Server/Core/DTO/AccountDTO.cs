// <copyright file="AccountDTO.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-28" />
// <summary>Account data transfer object.</summary>

using System;
using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Account data transfer object.
	/// </summary>
	[DataContract]
	public class AccountDTO
	{
		/// <summary>
		/// Gets or sets account unique ID.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets account name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets account creation date.
		/// </summary>
		[DataMember]
		public DateTime Created { get; set; }

		/// <summary>
		/// Gets or sets account asset type ID.
		/// </summary>
		[DataMember]
		public int AssetTypeId { get; set; }

		/// <summary>
		/// Gets or sets account balance.
		/// </summary>
		[DataMember]
		public decimal Balance { get; set; }

		/// <summary>
		/// Gets or sets number of total account postings.
		/// </summary>
		[DataMember]
		public decimal PostingsCount { get; set; }

		/// <summary>
		/// Gets or sets account first posting date.
		/// </summary>
		[DataMember]
		public DateTime? FirstPostingDate { get; set; }

		/// <summary>
		/// Gets or sets account first posting date.
		/// </summary>
		[DataMember]
		public DateTime? LastPostingDate { get; set; }
	}
}