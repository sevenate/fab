// <copyright file="RepositoryBase.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Base class for all entity repositories that handle logged-in and logged-out global messages.
	/// </summary>
	public class RepositoryBase : IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
	{
		#region Properties

		/// <summary>
		/// Gets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		protected internal IEventAggregator EventAggregator { get; private set; }

		/// <summary>
		/// Gets currently logged in user ID.
		/// </summary>
		protected internal Guid UserId { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryBase"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public RepositoryBase(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);
		}

		#endregion

		#region Implementation of IHandle<in LoggedInMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public virtual void Handle(LoggedInMessage message)
		{
			UserId = message.Credentials.UserId;
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public virtual void Handle(LoggedOutMessage message)
		{
			UserId = Guid.Empty;
		}

		#endregion
	}
}