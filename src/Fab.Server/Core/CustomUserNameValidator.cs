//------------------------------------------------------------
// <copyright file="CustomUserNameValidator.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using Common.Logging;
using Fab.Server.Core.Services;

namespace Fab.Server.Core
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

			bool isAuthenticated;

			try
			{
				isAuthenticated = client.Authenticate(userName, password);
			}
			catch (Exception e)
			{
				var log = LogManager.GetCurrentClassLogger();
				log.Fatal("Unhandled exception:", e);
				throw;
			}

			if (!isAuthenticated)
			{
				var log = LogManager.GetCurrentClassLogger();
				log.Warn("Authentication failed. Attempt to use username: " + userName);
				
//				throw new SecurityTokenException("Username or password is incorrect");

				// To provide detailed information about failed validation use FaultException
				// Note: this is NOT recommended for production by security reason

				var faultDetail = new FaultDetail
				                  {
				                  	ErrorCode = "AUTH",
				                  	ErrorMessage = "Authentication failed.",
				                  	Description = "Username or password is incorrect."
				                  };

				throw new FaultException<FaultDetail>(
					faultDetail,
					new FaultReason(faultDetail.ErrorMessage),
					new FaultCode("Receiver"));
			}
		}

		#endregion
    }
}