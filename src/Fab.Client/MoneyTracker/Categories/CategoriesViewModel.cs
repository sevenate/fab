// <copyright file="CategoriesViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2010-04-12" />

using System.Collections.Specialized;
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
	[Export(typeof(CategoriesViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class CategoriesViewModel : Conductor<CategoryViewModel>.Collection.AllActive
	{
		#region Fields

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
					ResetCategories();
					NotifyOfPropertyChange(() => CategoryType);
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
			//Items.AddRange(repository.Entities.Select(CategoriesExtensions.Map));
			repository.Entities.CollectionChanged += OnEntitiesCollectionChanged;
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
			var newAccountViewModel = IoC.Get<NewCategoryViewModel>();
			newAccountViewModel.SelectedCategoryType = CategoryType;
			shell.Dialogs.ShowDialog(newAccountViewModel);
		}

		#endregion

		#region Private Methods

		private void ResetCategories()
		{
			Items.Clear();
			Items.AddRange(repository.Entities.Where(dto => dto.CategoryType == CategoryType).Select(CategoriesExtensions.Map));
		}

		/// <summary>
		/// Occurs when the items list of the collection has changed, or the collection is reset.
		/// </summary>
		/// <param name="o">Collection that was changed.</param>
		/// <param name="eventArgs">Change event data.</param>
		private void OnEntitiesCollectionChanged(object o, NotifyCollectionChangedEventArgs eventArgs)
		{
			switch (eventArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach (var newItem in eventArgs.NewItems)
					{
						var category = (CategoryDTO) newItem;
						
						if (category.CategoryType == CategoryType)
						{
							Items.Insert(Items.Count, category.Map());
						}
					}

					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (var oldItem in eventArgs.OldItems)
					{
						var category = (CategoryDTO) oldItem;

						if (category.CategoryType == CategoryType)
						{
							var categoryViewModel = Items.Where(a => a.Id == category.Id).Single();
							Items.Remove(categoryViewModel);
						}
					}

					break;

				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					ResetCategories();
					break;
			}

			NotifyOfPropertyChange(() => Items);
			NotifyOfPropertyChange(() => DisplayName);
		}

		#endregion
	}
}