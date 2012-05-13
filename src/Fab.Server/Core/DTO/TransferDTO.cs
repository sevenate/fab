// <copyright file="TransferDTO.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-29" />
// <summary>Transfer data transfer object.</summary>

using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Transfer data transfer object.
	/// </summary>
	[DataContract]
	public abstract class TransferDTO : JournalDTO
	{
		/// <summary>
		/// Gets or sets 2nd account ID from which the funds were written off
		/// or for which funds have been credited (depending on sign of the "amount" value).
		/// </summary>
		/// <remarks>
		/// <c>null</c> value indicates that 2nd account ID is unavailable.
		/// </remarks>
		[DataMember]
		public int? SecondAccountId { get; set; }
	}
}