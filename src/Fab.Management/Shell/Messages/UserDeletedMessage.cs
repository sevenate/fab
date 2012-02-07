//------------------------------------------------------------
// <copyright file="UserDeletedMessage.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

namespace Fab.Managment.Shell.Messages
{
	/// <summary>
	/// Send by <see cref="UserViewModel"/> if user has been successfully deleted.
	/// </summary>
	public class UserDeletedMessage
	{
		public UserViewModel User { get; set; }
	}
}