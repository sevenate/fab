// <copyright file="OpenMessageBoxMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-05-15" />

using System;
using Fab.Client.Framework;

namespace Fab.Client.Shell
{
	/// <summary>
	/// Send by any class that have need in modal operation via dialog box.
	/// </summary>
	public class OpenMessageBoxMessage
	{
		/// <summary>
		/// Gets or sets Message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets Title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets Options.
		/// </summary>
		public MessageBoxOptions Options { get; set; }

		/// <summary>
		/// Gets or sets Callback.
		/// </summary>
		public Action<IMessageBox> Callback { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenMessageBoxMessage"/> class.
		/// </summary>
		public OpenMessageBoxMessage()
		{
			Options = MessageBoxOptions.Ok;
		}
	}
}