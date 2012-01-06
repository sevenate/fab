using System.ComponentModel;
using System.ServiceModel;
using System.Windows;
using Fab.Managment.AdminServiceReference;

namespace Fab.Managment.Core
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

		/// <summary>
		/// Display error message.
		/// </summary>
		/// <param name="args">Async WCF service call result data.</param>
		public static void ErrorProcessing(AsyncCompletedEventArgs args)
		{
			if (args.Error is FaultException<FaultDetail>)
			{
				var ex = args.Error as FaultException<FaultDetail>;

				MessageBox.Show(ex.Detail.Description, "FaultException<FaultDetail> " + ex.Detail.ErrorCode + ": " + ex.Detail.ErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else if (args.Error is FaultException)
			{
				var ex = args.Error as FaultException;
				var reason = ex.Reason.GetMatchingTranslation() ?? ex.Reason.Translations[0];

				MessageBox.Show(ex.Message, "FaultException " + reason.Text, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				if (args.Error.InnerException != null)
				{
					var ex = args.Error.InnerException as FaultException;
					
					if (ex != null)
					{
						var reason = ex.Reason.GetMatchingTranslation() ?? ex.Reason.Translations[0];
						MessageBox.Show("Message: " + ex.Message + "\nReason: " + reason.Text, "FaultException", MessageBoxButton.OK, MessageBoxImage.Error);
					}
					else
					{
						MessageBox.Show(args.Error.InnerException.ToString(), "InnerException", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				else
				{
					MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}