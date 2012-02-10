// <copyright file="ServiceErrorMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;

namespace Fab.Client.Shell
{
	/// <summary>
	/// Send any time when some service call was ended with error.
	/// </summary>
	public class ServiceErrorMessage
	{
		/// <summary>
		/// Gets or sets unexpected service error.
		/// </summary>
		public Exception Error { get; set; }
	}
}