//------------------------------------------------------------
// <copyright file="CustomUserNameValidator.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Web.Security;
using Common.Logging;

namespace Fab.Server.Core
{
	/// <summary>
	/// Custom validation of user credentials for every WCF service call.
	/// </summary>
	public class ManagmentServiceValidator : UserNamePasswordValidator
	{
		public override void Validate(string userName, string password)
		{
			if (!FormsAuthentication.Authenticate(userName, password))
			{
				// User name or password is incorrect.
				var log = LogManager.GetCurrentClassLogger();
				log.Warn("Managment authentication failed. Attempt to use username: " + userName);

				throw new SecurityTokenException("Validation failed.");

				// To provide detailed information about failed validation use FaultException
				// Note: this is NOT recommendted for production by security reason

				//throw new FaultException("Validation failed.");
			}
		}
	}
}