//------------------------------------------------------------
// <copyright file="CategoriesViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories.Single;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Categories view model.
	/// </summary>
	[Export(typeof (CategoriesViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class CategoriesViewModel : Conductor<CategoryViewModel>.Collection.AllActive,
	                                   IHandle<CategoryUpdatedMessage>,
	                                   IHandle<CategoriesUpdatedMessage>,
	                                   IHandle<CategoryDeletedMessage>
	{
		#region Dependencies

		/// <summary>
		/// Gets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

		/// <summary>
		/// Category repository.
		/// </summary>
		private readonly ICategoriesRepository repository = IoC.Get<ICategoriesRepository>();

		#endregion

		#region Type

		/// <summary>
		/// Category type.
		/// </summary>
		private CategoryType categoryType;

		/// <summary>
		/// Gets or sets category type.
		/// </summary>
		public CategoryType CategoryType
		{
			get { return categoryType; }
			set
			{
				if (categoryType != value)
				{
					categoryType = value;
					NotifyOfPropertyChange(() => CategoryType);
					ResetCategories();
				}
			}
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public CategoriesViewModel()
		{
			eventAggregator.Subscribe(this);
			ResetCategories();
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets or Sets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return CategoryType + " categories (" + Items.Count + ")"; }
		}

		#endregion

		#region Create Category

		/// <summary>
		/// Open "new category" dialog.
		/// </summary>
		/// <returns>Operation result.</returns>
		public void CreateCategory()
		{
			var shell = IoC.Get<IShell>();
			var categoryViewModel = IoC.Get<NewCategoryViewModel>();
			categoryViewModel.SelectedCategoryType = CategoryType;
			shell.Dialogs.ShowDialog(categoryViewModel);
		}

		#endregion

		#region Implementation of IHandle<CategoryUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoryUpdatedMessage message)
		{
			CategoryViewModel categoryViewModel = Items.Where(viewModel => viewModel.Id == message.Category.Id).SingleOrDefault();

			if (categoryViewModel != null)
			{
				if (message.Category.CategoryType == CategoryType)
				{
					// Update existing
					// TODO: find a way to use any kind of "auto mapper" here
					categoryViewModel.Name = message.Category.Name;
					categoryViewModel.Popularity = message.Category.Popularity;
				}
				else
				{
					// Remove old copy since CategoryType was changed
					Items.Remove(categoryViewModel);
				}
			}
			else
			{
				// Add new 
				if (message.Category.CategoryType == CategoryType)
				{
					Items.Add(message.Category.Map());
				}
			}

			NotifyOfPropertyChange(() => DisplayName);
		}

		#endregion

		#region Implementation of IHandle<CategoriesUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoriesUpdatedMessage message)
		{
			ResetCategories();
		}

		#endregion

		#region Implementation of IHandle<CategoryDeletedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoryDeletedMessage message)
		{
			CategoryViewModel categoryViewModel = Items.Where(viewModel => viewModel.Id == message.Category.Id).SingleOrDefault();

			if (categoryViewModel != null)
			{
				// Remove existing
				Items.Remove(categoryViewModel);
			}

			NotifyOfPropertyChange(() => DisplayName);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Clean current list of categories and fill it with new items filtered by <see cref="CategoryType"/> from repository.
		/// </summary>
		private void ResetCategories()
		{
			Items.Clear();
			Items.AddRange(repository.Entities.Where(dto => dto.CategoryType == CategoryType).Select(CategoriesExtensions.Map));
			NotifyOfPropertyChange(() => DisplayName);
		}

		#endregion
	}
}