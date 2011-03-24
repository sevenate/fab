// <copyright file="CategoryType.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-07-01" />

namespace Fab.Server.Core.Enums
{
	/// <summary>
	/// Specify category types.
	/// </summary>
	public enum CategoryType : byte
	{
		/// <summary>
		/// Category for withdrawal and deposits (id = 0).
		/// </summary>
		Common = 0,

		/// <summary>
		/// Category for withdrawal (id = 1).
		/// </summary>
		Withdrawal = 1,

		/// <summary>
		/// Category for deposit (id = 2).
		/// </summary>
		Deposit = 2
	}
}