// <copyright file="ILoginViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-11-17" />

using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.Framework;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// Interface for login view model.
	/// </summary>
	public interface ILoginViewModel : IModule
	{
		/// <summary>
		/// Gets or sets user name.
		/// </summary>
		string Username { get; set; }

		/// <summary>
		/// Gets or sets user password.
		/// </summary>
		string Password { get; }

		/// <summary>
		/// Gets a value indicating whether a user is authenticated.
		/// </summary>
		bool IsAuthenticated { get; }

		/// <summary>
		/// Gets or sets status message.
		/// </summary>
		string Status { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a status message should be visible.
		/// </summary>
		bool ShowStatus { get; }

		/// <summary>
		/// Gets or sets a value indicating whether a password characters should be visible to user.
		/// </summary>
		bool ShowCharacters { get; set; }

		/// <summary>
		/// Login with specified credentials.
		/// </summary>
		/// <returns>Common co-routine result.</returns>
		IEnumerable<IResult> Login();

		/// <summary>
		/// Check if the credentials meets the security requirements.
		/// </summary>
		/// <returns><c>true</c> if the username and password meets the security requirements.</returns>
		bool CanLogin();
	}
}