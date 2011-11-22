//------------------------------------------------------------
// <copyright file="CustomUserNameValidator.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Fab.Server
{
	/// <summary>
	/// Validate each WCF service call with username and password.
	/// </summary>
    public class CustomUserNameValidator : UserNamePasswordValidator
    {
		#region Overrides of UserNamePasswordValidator

		/// <summary>
		/// When overridden in a derived class, validates the specified username and password.
		/// </summary>
		/// <param name="userName">The username to validate.</param><param name="password">The password to validate.</param>
		public override void Validate(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
			{
				throw new SecurityTokenException("Username and password should not be empty");
			}
			var client = new UserService();

			var user = client.GetUser(userName, password);

			if (user == null || user.Id == Guid.Empty)
			{
				throw new SecurityTokenException("Username or password is incorrect");
			}
		}

		#endregion
    }
}