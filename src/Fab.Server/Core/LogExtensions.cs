//------------------------------------------------------------
// <copyright file="LogExtensions.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;
using Common.Logging;

namespace Fab.Server.Core
{
	/// <summary>
	/// Several extensions to <see cref="ILog"/>.
	/// </summary>
	public static class LogExtensions
	{
		/// <summary>
		/// Log WCF service client IP address.
		/// </summary>
		/// <param name="log">Current logger.</param>
		/// <param name="operation">Operation contract name.</param>
		public static void LogClientIP(this ILog log, string operation)
		{
			OperationContext context = OperationContext.Current;

			if (context != null)
			{
				MessageProperties messageProperties = context.IncomingMessageProperties;

				if (messageProperties != null)
				{
					var endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

					if (endpointProperty != null)
					{
						log.Info(string.Format("[{0}] Client IP - {1}:{2}", operation, endpointProperty.Address, endpointProperty.Port));
					}
				}
			}
		}
	}
}