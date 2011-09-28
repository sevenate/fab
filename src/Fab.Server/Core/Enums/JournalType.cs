// <copyright file="JournalType.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-02-23" />

namespace Fab.Server.Core.Enums
{
	/// <summary>
	/// Specify journal records types.
	/// </summary>
	public enum JournalType
	{
		/// <summary>
		/// Deposit journal record (id = 0).
		/// </summary>
		Deposit = 0,

		/// <summary>
		/// Withdrawal journal record (id = 1).
		/// </summary>
		Withdrawal = 1,
		
		/// <summary>
		/// Transfer journal record (id = 2).
		/// </summary>
		Transfer = 2
	}
}