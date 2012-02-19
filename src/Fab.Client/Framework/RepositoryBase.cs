// <copyright file="RepositoryBase.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Base class for all entity repositories that handle logged-in and logged-out global messages.
	/// </summary>
	/// TODO: convert <see cref="Create"/> and other related into <see cref="Entities"/> own "add()" operations.
	/// <typeparam name="T">Entity type.</typeparam>
	public abstract class RepositoryBase<T> : IRepository<T, int>
	{
		#region Properties

		/// <summary>
		/// Gets all entities for user.
		/// TODO: change to "read-only".
		/// </summary>
		public IObservableCollection<T> Entities { get; private set; }

		/// <summary>
		/// Gets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		protected IEventAggregator EventAggregator { get; private set; }

		/// <summary>
		/// Gets currently logged in user ID.
		/// </summary>
		protected Guid UserId { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		protected RepositoryBase(IEventAggregator eventAggregator)
		{
			Entities = new BindableCollection<T>();
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
			Download();
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
			Entities.Clear();
		}

		#endregion

		#region Implementation of IRepository<T,int>

		/// <summary>
		/// Retrieve specific entity by unique key.
		/// </summary>
		/// <param name="key">Entity key.</param>
		/// <returns>Corresponding entity or null if not found.</returns>
		public abstract T ByKey(int key);

		/// <summary>
		/// Download all entities from server.
		/// </summary>
		public abstract void Download();

		/// <summary>
		/// Download one entity from server.
		/// </summary>
		/// <param name="key">Entity key.</param>
		public virtual void Download(int key)
		{
		}

		/// <summary>
		/// Create new entity.
		/// </summary>
		/// <param name="entity">New entity data to create from.</param>
		/// <returns>Created entity with filled server-side updated properties.</returns>
		public virtual T Create(T entity)
		{
			return entity;
		}

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Updated entity data.</param>
		/// <returns>Updated entity with filled server-side updated properties.</returns>
		public virtual T Update(T entity)
		{
			return entity;
		}

		/// <summary>
		/// Delete existing entity.
		/// </summary>
		/// <param name="key">Key if the entity to delete.</param>
		public virtual void Delete(int key)
		{
		}

		#endregion
	}
}