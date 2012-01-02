// <copyright file="DepositDTO.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-29" />
// <summary>Deposit data transfer object.</summary>

using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Deposit data transfer object.
	/// </summary>
	[DataContract]
	public class DepositDTO : TransactionDTO
	{
	}
}