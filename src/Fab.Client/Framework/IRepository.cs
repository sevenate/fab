// <copyright file="IRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-05" />

using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Specify interface of common operation with entities.
	/// </summary>
	/// <typeparam name="T">User specific entity type.</typeparam>
	/// <typeparam name="K">Entity specific unique key type.</typeparam>
	public interface IRepository<T, K> : IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
	{
		/// <summary>
		/// Gets all entities for user.
		/// </summary>
		IEnumerable<T> All { get; }

		/// <summary>
		/// Retrieve specific entity by unique key.
		/// </summary>
		/// <param name="key">Entity key.</param>
		/// <returns>Corresponding entity or null if not found.</returns>
		T ByKey(K key);

		/// <summary>
		/// Download all entities from server.
		/// </summary>
		void Download();

		/// <summary>
		/// Create new entity.
		/// </summary>
		/// <param name="entity">New entity data to create from.</param>
		/// <returns>Created entity with filled server-side updated properties.</returns>
		T Create(T entity);

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Updated entity data.</param>
		/// <returns>Updated entity with filled server-side updated properties.</returns>
		T Update(T entity);

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Entity to delete.</param>
		void Delete(T entity);
	}
}