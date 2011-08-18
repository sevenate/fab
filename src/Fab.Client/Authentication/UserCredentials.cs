// <copyright file="UserCredentials.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-17" />

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
		private static UserCredentials currentCredentials;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserCredentials"/> class.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="userName">User name.</param>
		public UserCredentials(Guid userId, string userName)
		{
			UserId = userId;
			UserName = userName;
		}

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

				return currentCredentials;
			}

			set { currentCredentials = value; }
		}

		/// <summary>
		/// Gets a value indicating whether a current user credentials are available.
		/// </summary>
		public static bool IsAvailable
		{
			get { return currentCredentials != null; }
		}

		/// <summary>
		/// Gets user unique identifier.
		/// TODO: update to use session key instead.
		/// </summary>
		public Guid UserId { get; private set; }

		/// <summary>
		/// Gets current user name.
		/// </summary>
		public string UserName { get; private set; }
	}
}