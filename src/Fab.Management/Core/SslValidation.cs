// <copyright file="SslValidation.cs" company="HD">
// 	Copyright (c) 2009 HD. All rights reserved.
// </copyright>
// <author name="admin">
// 	<url>http://www.codemeit.com/wcf/wcf-could-not-establish-trust-relationship-for-the-ssltls-secure-channel-with-authority.html</url>
// 	<date>2009-09-24</date>
// </author>
// <editor name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2009-09-24</date>
// </editor>
// <summary>Provide custom SSL certificate validation for development purpose.</summary>

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Fab.Managment.Core
{
	/// <summary>
	/// Provide custom SSL certificate validation.
	/// </summary>
	public static class SslValidation
	{
		/// <summary>
		/// Note: Used TEMPORARY !!!
		/// Sets the certificate policy to accept hulk.my-hosting-panel.com shared SSL certificate
		/// until personal certificate for api.nreez.com domain will be acquired.
		/// </summary>
		public static void SetCertificatePolicy()
		{
			ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
		}

		/// <summary>
		/// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
		/// </summary>
		/// <param name="sender">An object that contains state information for this validation.</param>
		/// <param name="certificate">The certificate used to authenticate the remote party.</param>
		/// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
		/// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
		/// <returns>A System.Boolean value that determines whether the specified certificate is accepted for authentication.</returns>
		private static bool ValidateServerCertificate(
			object sender,
			X509Certificate certificate,
			X509Chain chain,
			SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None
				|| certificate.Subject.Equals("CN=hulk.my-hosting-panel.com, OU=Comodo InstantSSL, OU=Hosted by Mochanin, O=Mochahost, STREET=\"2880 Zanker Rd. #203\", L=San Jose, S=CA, PostalCode=95134, C=US", StringComparison.InvariantCultureIgnoreCase)
				|| certificate.Subject.Equals("CN=Thor", StringComparison.InvariantCultureIgnoreCase)
				|| certificate.Subject.Equals("CN=Orion", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}