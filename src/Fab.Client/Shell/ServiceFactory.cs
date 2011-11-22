//------------------------------------------------------------
// <copyright file="ServiceFactory.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Fab.Client.Authentication;
using Fab.Client.MoneyServiceReference;
using Fab.Client.UserServiceReference;

namespace Fab.Client.Shell
{
	/// <summary>
	/// Create WCF service instances with settings based on Debug/Release configuration.
	/// </summary>
	public static class ServiceFactory
	{
		/// <summary>
		/// Gets or sets personal service address.
		/// </summary>
		public static string PersonalServiceUrl { get; set; }

		/// <summary>
		/// Create UserService client proxy.
		/// </summary>
		/// <returns>New UserService client proxy.</returns>
		public static UserServiceClient CreateUserService()
		{
			var proxy = new UserServiceClient();
			SetupClientCredentials(proxy.ClientCredentials);
			return proxy;
		}

		/// <summary>
		/// Create MoneyService client proxy.
		/// </summary>
		/// <returns>New MoneyService client proxy.</returns>
		public static MoneyServiceClient CreateMoneyService()
		{
			var proxy = new MoneyServiceClient();
			SetupClientCredentials(proxy.ClientCredentials);
			SetupPersonalServiceHost(proxy);
			return proxy;
		}

		/// <summary>
		/// Specify user client credentials for proxy.
		/// </summary>
		/// <param name="clientCredentials">User credentials.</param>
		private static void SetupClientCredentials(ClientCredentials clientCredentials)
		{
			if (clientCredentials == null)
			{
				throw new ArgumentNullException("clientCredentials");
			}

			if (UserCredentials.IsAvailable)
			{
				clientCredentials.UserName.UserName = UserCredentials.Current.UserName;
				clientCredentials.UserName.Password = UserCredentials.Current.Password;
			}
		}

		/// <summary>
		/// Use personal service host only if available.
		/// </summary>
		/// <param name="proxy">Client proxy instance.</param>
		private static void SetupPersonalServiceHost(MoneyServiceClient proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}

			if (!string.IsNullOrEmpty(PersonalServiceUrl))
			{
				proxy.Endpoint.Address = new EndpointAddress(PersonalServiceUrl);
			}
		}
	}
}