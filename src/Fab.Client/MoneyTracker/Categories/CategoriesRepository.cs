// <copyright file="CategoriesRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Provide access to all user categories.
	/// </summary>
	[Export(typeof (ICategoriesRepository))]
	public class CategoriesRepository : RepositoryBase<CategoryDTO>, ICategoriesRepository
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesRepository"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public CategoriesRepository(IEventAggregator eventAggregator) : base(eventAggregator)
		{
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public override void Handle(LoggedOutMessage message)
		{
			base.Handle(message);
			EventAggregator.Publish(new CategoriesUpdatedMessage
			                        {
			                        	Categories = Entities
			                        });
		}

		#endregion

		#region Overrides of RepositoryBase<CategoryDTO>

		/// <summary>
		/// Retrieve category by unique key.
		/// </summary>
		/// <param name="key">Category key.</param>
		/// <returns>Corresponding category or null if not found.</returns>
		public override CategoryDTO ByKey(int key)
		{
			return Entities.Where(c => c.Id == key).SingleOrDefault();
		}

		/// <summary>
		/// Download all <see cref="CategoryDTO"/> entities from server.
		/// </summary>
		public override void Download()
		{
			var proxy = new MoneyServiceClient();

			proxy.GetAllCategoriesCompleted += (s, e) =>
			                                   {
			                                   	if (e.Error == null)
			                                   	{
			                                   		Entities.Clear();
			                                   		Entities.AddRange(e.Result);
			                                   		Execute.OnUIThread(() => EventAggregator.Publish(new CategoriesUpdatedMessage
			                                   		                                                 {
			                                   		                                                 	Categories = Entities
			                                   		                                                 }));
			                                   	}
			                                   	else
			                                   	{
			                                   		Execute.OnUIThread(() => EventAggregator.Publish(new CategoriesUpdatedMessage
			                                   		                                                 {
			                                   		                                                 	Error = e.Error
			                                   		                                                 }));
			                                   	}
			                                   };

			proxy.GetAllCategoriesAsync(UserId);
		}

		/// <summary>
		/// Create new category.
		/// </summary>
		/// <param name="entity">New category data to create from.</param>
		/// <returns>Created category with filled server-side updated properties.</returns>
		public override CategoryDTO Create(CategoryDTO entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			var proxy = new MoneyServiceClient();

			proxy.CreateCategoryCompleted += (s, e) =>
			                                 {
			                                 	if (e.Error == null)
			                                 	{
			                                 		entity.Id = e.Result;
			                                 		Entities.Add(entity);
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new CategoriesUpdatedMessage
			                                 		                                                 {
			                                 		                                                 	Categories = Entities
			                                 		                                                 }));
			                                 	}
			                                 	else
			                                 	{
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new CategoriesUpdatedMessage
			                                 		                                                 {
			                                 		                                                 	Error = e.Error
			                                 		                                                 }));
			                                 	}
			                                 };

			proxy.CreateCategoryAsync(UserId, entity.Name, entity.CategoryType);

			return entity;
		}

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Updated entity data.</param>
		/// <returns>Updated entity with filled server-side updatable properties.</returns>
		public override CategoryDTO Update(CategoryDTO entity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Entity to delete.</param>
		public override void Delete(CategoryDTO entity)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IAccountsRepository

		/// <summary>
		/// Create new category for specific user.
		/// </summary>
		/// <param name="name">New category name.</param>
		/// <param name="categoryType">New category type.</param>
		/// <returns>Created category.</returns>
		public CategoryDTO Create(string name, CategoryType categoryType)
		{
			var category = new CategoryDTO
			               {
			               	Id = 0,
			               	Name = name,
			               	CategoryType = categoryType,
			               	Popularity = 0,
			               };

			return Create(category);
		}

		#endregion
	}
}