//------------------------------------------------------------
// <copyright file="ManagmentServiceValidator.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Web.Security;
using Common.Logging;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core
{
	/// <summary>
	/// Custom validation of user credentials for every WCF service call.
	/// </summary>
	public class ManagmentServiceValidator : UserNamePasswordValidator
	{
		public override void Validate(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
			{
				throw new SecurityTokenException("Username and password should not be empty.");
			}

			bool isAuthenticated;

			try
			{
				isAuthenticated = FormsAuthentication.Authenticate(userName, password);
			}
			catch (Exception e)
			{
				var log = LogManager.GetCurrentClassLogger();
				log.Fatal("Unhandled exception: " + e);
				throw new SecurityTokenException("Unable to authenticate by the reason of service internal error.");
			}

			if (!isAuthenticated)
			{
				// User name or password is incorrect.
				var log = LogManager.GetCurrentClassLogger();
				log.Warn("Management authentication failed. Attempt to use username: " + userName);

//				throw new SecurityTokenException("Validation failed.");

				// To provide detailed information about failed validation use FaultException
				// Note: this is NOT recommended for production by security reason

				var faultDetail = new FaultDetail
				{
					ErrorCode = "AUTH-0",
					ErrorMessage = "Authentication failed.",
					Description = "Username or password is incorrect."
				};

				throw new FaultException<FaultDetail>(
					faultDetail,
					new FaultReason(faultDetail.Description),
					new FaultCode("Receiver"));
			}
		}
	}
}