// <copyright file="CategoriesRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-03-26" />

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Provide access to all user categories.
	/// </summary>
	[Export(typeof(ICategoriesRepository))]
	public class CategoriesRepository : RepositoryBase<CategoryDTO>, ICategoriesRepository
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesRepository"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public CategoriesRepository(IEventAggregator eventAggregator)
			: base(eventAggregator)
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
													   Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
																										{
																											Error = e.Error
																										}));
												   }

												   EventAggregator.Publish(new AsyncOperationCompleteMessage());
											   };

			proxy.GetAllCategoriesAsync(UserId);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Downloading categories" });
		}

		/// <summary>
		/// Download one entity from server.
		/// </summary>
		/// <param name="key">Entity key.</param>
		public override void Download(int key)
		{
			var proxy = new MoneyServiceClient();
			proxy.GetCategoryCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					var category = ByKey(key);

					// TODO: find a way to use Automapper for SL4 here
					// TODO: Use approach like in AccountsRepository here instead of copy prop
					category.CategoryType = e.Result.CategoryType;
					category.Id = e.Result.Id;
					category.Name = e.Result.Name;
					category.Popularity = e.Result.Popularity;

					Execute.OnUIThread(() => EventAggregator.Publish(new CategoryUpdatedMessage
					{
						Category = e.Result
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
					{
						Error = e.Error
					}));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.GetCategoryAsync(UserId, key);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Downloading category #" + key });
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
													 Execute.OnUIThread(() => EventAggregator.Publish(new CategoryUpdatedMessage
																									  {
																										  Category = entity
																									  }));
												 }
												 else
												 {
													 Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
																									  {
																										  Error = e.Error
																									  }));
												 }

												 EventAggregator.Publish(new AsyncOperationCompleteMessage());
											 };

			proxy.CreateCategoryAsync(UserId, entity.Name, entity.CategoryType);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Creating new category \"" + entity.Name + "\"" });

			return entity;
		}

		/// <summary>
		/// Update existing category.
		/// </summary>
		/// <param name="entity">Updated category data.</param>
		/// <returns>Updated category with filled server-side updatable properties.</returns>
		public override CategoryDTO Update(CategoryDTO entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			var proxy = new MoneyServiceClient();
			proxy.UpdateCategoryCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					var category = Entities.Where(dto => dto.Id == entity.Id).Single();

					// Copy new values on client side after update confirmation on server
					category.Name = entity.Name;
					category.CategoryType = entity.CategoryType;

					Execute.OnUIThread(() => EventAggregator.Publish(new CategoryUpdatedMessage
					{
						Category = category
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
					{
						Error = e.Error
					}));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.UpdateCategoryAsync(UserId, entity.Id, entity.Name, entity.CategoryType);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Updating category \"" + entity.Name + "\"" });

			return entity;
		}

		/// <summary>
		/// Delete existing category.
		/// </summary>
		/// <param name="key">Category ID to delete.</param>
		public override void Delete(int key)
		{
			var proxy = new MoneyServiceClient();

			proxy.DeleteCategoryCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					var deletedCategory = ByKey(key);
					Entities.Remove(deletedCategory);

					Execute.OnUIThread(() => EventAggregator.Publish(new CategoryDeletedMessage
					{
						Category = deletedCategory
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
					{
						Error = e.Error
					}));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.DeleteCategoryAsync(UserId, key);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Deleting category #" + key });
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

		/// <summary>
		/// Update category for specific user.
		/// </summary>
		/// <param name="id">Existing category id.</param>
		/// <param name="name">New category name.</param>
		/// <param name="categoryType">New category type.</param>
		/// <returns>Updated category.</returns>
		public CategoryDTO Update(int id, string name, CategoryType categoryType)
		{
			var category = new CategoryDTO
			{
				Id = id,
				Name = name,
				CategoryType = categoryType,
				Popularity = 0,
			};

			return Update(category);
		}

		#endregion
	}
}