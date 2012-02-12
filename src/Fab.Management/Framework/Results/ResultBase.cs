//------------------------------------------------------------
// <copyright file="ResultBase.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Caliburn.Micro;
using Fab.Client.Shell;

namespace Fab.Managment.Framework.Results
{
	/// <summary>
	/// <see cref="IEventAggregator"/>-aware base class for all <see cref="IResult"/>.
	/// </summary>
	public abstract class ResultBase
	{
		/// <summary>
		/// Send error message with exception details.
		/// </summary>
		/// <param name="e">Exception details.</param>
		protected void SendErrorMessage(Exception e)
		{
			var eventAggregator = IoC.Get<IEventAggregator>();
			eventAggregator.Publish(new ApplicationErrorMessage
			                        	{
			                        		Error = e
			                        	});
		}
	}
}