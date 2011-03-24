// <copyright file="WithdrawalDTO.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-29" />
// <summary>Withdrawal data transfer object.</summary>

using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Withdrawal data transfer object.
	/// </summary>
	[DataContract]
	public class WithdrawalDTO : TransactionDTO
	{
	}
}