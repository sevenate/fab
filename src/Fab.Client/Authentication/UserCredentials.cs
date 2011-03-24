// <copyright file="UserCredentials.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-17" />
// <summary>Represent user name, password and user ID.</summary>

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
		public UserCredentials(Guid userId)
		{
			UserId = userId;
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
	}
}