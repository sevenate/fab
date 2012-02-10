//------------------------------------------------------------
// <copyright file="AccountMaintenanceDTOl.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// User maintenance result data transfer object.
	/// </summary>
	[DataContract]
	public class AccountMaintenanceDTO : AccountDTO
	{
		/// <summary>
		/// Gets or sets actual account balance.
		/// </summary>
		[DataMember]
		public decimal ActualBalance { get; set; }

		/// <summary>
		/// Gets or sets actual number of total account postings.
		/// </summary>
		[DataMember]
		public decimal ActualPostingsCount { get; set; }

		/// <summary>
		/// Gets or sets account actual first posting date.
		/// </summary>
		[DataMember]
		public DateTime? ActualFirstPostingDate { get; set; }

		/// <summary>
		/// Gets or sets account actual first posting date.
		/// </summary>
		[DataMember]
		public DateTime? ActualLastPostingDate { get; set; }
	}
}