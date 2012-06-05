//------------------------------------------------------------
// <copyright file="LoggedInMessage.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

namespace Fab.Client.Authentication
{
	/// <summary>
	/// Send by <see cref="LoginViewModel"/> or <see cref="RegistrationViewModel"/> after successful login.
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