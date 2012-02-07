//------------------------------------------------------------
// <copyright file="ICanBeBusy.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

namespace Fab.Core.Framework
{
	/// <summary>
	/// Busy contract for view-models.
	/// </summary>
	public interface ICanBeBusy
	{
		/// <summary>
		/// Gets or sets a value indicating whether view-model is busy by some background operation.
		/// </summary>
		bool IsBusy { get; set; }
	}
}