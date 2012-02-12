//------------------------------------------------------------
// <copyright file="ApplicationErrorMessage.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;

namespace Fab.Client.Shell
{
	/// <summary>
	/// Send any time when some service call was ended with error.
	/// </summary>
	public class ApplicationErrorMessage
	{
		/// <summary>
		/// Gets or sets unexpected application error.
		/// </summary>
		public Exception Error { get; set; }
	}
}