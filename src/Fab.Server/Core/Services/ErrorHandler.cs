//------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Common.Logging;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Services
{
	/// <summary>
	/// Common service unhandled error handler.
	/// </summary>
	public class ErrorHandler : IErrorHandler
	{ 
		#region Implementation of IErrorHandler

		/// <summary>
		/// Enables the creation of a custom System.ServiceModel.FaultException<TDetail> that is returned from an exception in the course of a service method.
		/// </summary>
		/// <param name="error">The System.Exception object thrown in the course of the service operation.</param>
		/// <param name="version">The SOAP version of the message.</param>
		/// <param name="fault">The System.ServiceModel.Channels.Message object that is returned to the client, or service, in the duplex case.</param>
		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
		}

		/// <summary>
		/// Enables error-related processing and returns a value that indicates whether subsequent HandleError implementations are called.
		/// </summary>
		/// <param name="error">The exception thrown during processing.</param>
		/// <returns>true if subsequent <see cref="System.ServiceModel.Dispatcher.IErrorHandler"/> implementations must not be called; otherwise, false. The default is false.</returns>
		public bool HandleError(Exception error)
		{
			// Log only unhandled exceptions here; all handled faults should be logged before they were raised with additional details.
			if (!(error is FaultException<FaultDetail>))
			{
				var log = LogManager.GetCurrentClassLogger();
				log.Fatal("Unhandled exception:", error);
			}

			return false;
		}

		#endregion
	}
}