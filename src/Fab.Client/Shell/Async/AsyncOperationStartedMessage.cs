// <copyright file="AsyncOperationStartedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov <78@nreez.com>" date="2011-08-31" />

namespace Fab.Client.Shell.Async
{
	/// <summary>
	/// Broadcasted when any new background operation has been started.
	/// </summary>
	public class AsyncOperationStartedMessage
	{
		/// <summary>
		/// Gets or sets started async operation name.
		/// </summary>
		public string OperationName { get; set; }
	}
}