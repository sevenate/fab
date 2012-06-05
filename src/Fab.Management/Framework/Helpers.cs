//------------------------------------------------------------
// <copyright file="Helpers.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ServiceModel;
using Fab.Managment.AdminServiceReference;

namespace Fab.Managment.Framework
{
	public static class Helpers
	{
		public static string EndPointAddress { get; set; }
		public static string Username { get; set; }
		public static string Password { get; set; }

		public static AdminServiceClient CreateClientProxy(string endPointAddress, string username, string password)
		{
			var binding = new WSHttpBinding
			              {
			              	MaxBufferPoolSize = 2147483647,
			              	MaxReceivedMessageSize = 2147483647
			              };

			binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
			
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
			binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
			binding.Security.Transport.Realm = string.Empty;

			binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
			binding.Security.Message.NegotiateServiceCredential = true;
			binding.Security.Message.EstablishSecurityContext = true;

//			var endPoint = new EndpointAddress(
//				Application.Current.Host.Source.AbsoluteUri.Replace(
//					Application.Current.Host.Source.AbsolutePath, "/Api/FeedbackService.svc"));

			var endPoint = new EndpointAddress(endPointAddress ?? EndPointAddress);

			var proxy = new AdminServiceClient(binding, endPoint);
			proxy.ClientCredentials.UserName.UserName = username ?? Username;
			proxy.ClientCredentials.UserName.Password = password ?? Password;

			return proxy;
		}
	}
}