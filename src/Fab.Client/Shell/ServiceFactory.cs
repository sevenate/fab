//------------------------------------------------------------
// <copyright file="ServiceFactory.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;
using Fab.Client.Authentication;
using Fab.Client.MoneyServiceReference;
using Fab.Client.RegistrationServiceReference;
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
		public static string PersonalServiceUri { get; set; }

		/// <summary>
		/// Create UserService client proxy.
		/// </summary>
		/// <returns>New UserService client proxy.</returns>
		public static UserServiceClient CreateUserService()
		{
			var proxy = new UserServiceClient();
			SetupServiceUri(proxy.Endpoint);
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
			SetupServiceUri(proxy.Endpoint);
			SetupClientCredentials(proxy.ClientCredentials);
			SetupPersonalServiceUri(proxy);
			return proxy;
		}

		/// <summary>
		/// Create RegistrationService client proxy.
		/// </summary>
		/// <returns>New RegistrationService client proxy.</returns>
		public static RegistrationServiceClient CreateRegistrationService()
		{
			var proxy = new RegistrationServiceClient();
			SetupServiceUri(proxy.Endpoint);
			return proxy;
		}

		/// <summary>
		/// Change development service url to the relative to host from where the xap was loaded.
		/// </summary>
		/// <param name="serviceEndpoint">Original service endpoint whose address should be updated.</param>
		private static void SetupServiceUri(ServiceEndpoint serviceEndpoint)
		{
#if !DEBUG
			var serviceName = serviceEndpoint.Address.Uri.LocalPath.Split('/').Last();
// ReSharper disable AssignNullToNotNullAttribute
			string absoluteUri = new Uri(Application.Current.Host.Source, Path.Combine(@"../sl/", serviceName)).AbsoluteUri;
// ReSharper restore AssignNullToNotNullAttribute
			serviceEndpoint.Address = new EndpointAddress(absoluteUri);
#endif
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
		private static void SetupPersonalServiceUri(MoneyServiceClient proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}

			if (!string.IsNullOrEmpty(PersonalServiceUri))
			{
				proxy.Endpoint.Address = new EndpointAddress(PersonalServiceUri);
			}
		}
	}
}