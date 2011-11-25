//------------------------------------------------------------
// <copyright file="CustomUserNameValidator.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common.Logging;
using Fab.Server.Core.DTO;

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

			UserDTO user = null;

			try
			{
				user = client.GetUser(userName, password);
			}
			catch (Exception e)
			{
				var log = LogManager.GetCurrentClassLogger();
				log.Fatal("Unhandled exception:", e);
				throw;
			}

			if (user == null || user.Id == Guid.Empty)
			{
				// User name or password is incorrect.
				var log = LogManager.GetCurrentClassLogger();
				log.Warn("Authentication failed. Attempt to use username: " + userName);
				
				throw new SecurityTokenException("Username or password is incorrect");

				// To provide detailed information about failed validation use FaultException
				// Note: this is NOT recommended for production by security reason

				//throw new FaultException("Validation failed.");
			}
		}

		#endregion
    }
}