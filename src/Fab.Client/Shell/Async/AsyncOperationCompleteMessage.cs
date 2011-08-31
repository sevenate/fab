// <copyright file="AsyncOperationCompleteMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov <78@nreez.com>" date="2011-08-31" />

namespace Fab.Client.Shell.Async
{
	/// <summary>
	/// Broadcasted when any of the background operations has been complete.
	/// </summary>
	public class AsyncOperationCompleteMessage
	{
		/// <summary>
		/// Gets or sets complete async operation name.
		/// </summary>
		public string OperationName { get; set; }
	}
}