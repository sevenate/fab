// <copyright file="LoggedInMessage.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-24" />

namespace Fab.Client.Authentication
{
	/// <summary>
	/// Send by <see cref="ILoginViewModel"/> after successful login.
	/// </summary>
	public class LoggedInMessage
	{
		/// <summary>
		/// Gets logged in user credentials.
		/// </summary>
		public UserCredentials Credentials { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggedInMessage"/> class.
		/// </summary>
		/// <param name="credentials">Logged in user credentials.</param>
		public LoggedInMessage(UserCredentials credentials)
		{
			Credentials = credentials;
		}
	}
}