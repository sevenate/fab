// <copyright file="OpenDialogMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-05-14" />

using Caliburn.Micro;

namespace Fab.Client.Shell
{
	/// <summary>
	/// Send by any class that have need in modal operation via dialog box.
	/// </summary>
	public class OpenDialogMessage
	{
		/// <summary>
		/// Gets or sets a modal screen to popup.
		/// </summary>
		public IScreen Dialog { get; set; }
	}
}