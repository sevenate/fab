// <copyright file="ILoginViewModel.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-11-17" />
// <summary>Interface for login view model.</summary>

using System;
using System.Collections.Generic;
using Caliburn.Micro;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// Interface for login view model.
	/// </summary>
	public interface ILoginViewModel: IScreen
	{
		/// <summary>
		/// Gets or sets user name.
		/// </summary>
		string Username { get; }

		/// <summary>
		/// Gets or sets user password.
		/// </summary>
		string Password { get; }

		/// <summary>
		/// Gets or sets user unique identifier.
		/// </summary>
		Guid UserId { get; }

		/// <summary>
		/// Login with specified credentials.
		/// </summary>
		/// <returns>Common co-routine result.</returns>
		IEnumerable<IResult> Login();

		/// <summary>
		/// Logout from the system.
		/// </summary>
		void Logout();
	}
}