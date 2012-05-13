//------------------------------------------------------------
// <copyright file="UserCredentials.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// Represent user name and unique ID.
	/// </summary>
	public class UserCredentials
	{
		/// <summary>
		/// Holds current user credentials.
		/// </summary>
		private static UserCredentials current;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserCredentials"/> class.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="userName">User name.</param>
		/// <param name="password">User password.</param>
		public UserCredentials(Guid userId, string userName, string password)
		{
			UserId = userId;
			UserName = userName;
			Password = password;
		}

		/// <summary>
		/// Gets current user name.
		/// </summary>
		public string UserName { get; private set; }

		/// <summary>
		/// Gets user password.
		/// </summary>
		public string Password { get; private set; }

		/// <summary>
		/// Gets user unique identifier.
		/// TODO: update to use session key instead.
		/// </summary>
		public Guid UserId { get; private set; }

		/// <summary>
		/// Gets or sets current user credentials.
		/// </summary>
		public static UserCredentials Current
		{
			get
			{
				if (!IsAvailable)
				{
					throw new UnauthorizedAccessException();
				}

				return current;
			}

			set { current = value; }
		}

		/// <summary>
		/// Gets a value indicating whether a current user credentials are available.
		/// </summary>
		public static bool IsAvailable
		{
			get { return current != null; }
		}
	}
}