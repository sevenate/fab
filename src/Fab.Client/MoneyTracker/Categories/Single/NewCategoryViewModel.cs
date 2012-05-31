//------------------------------------------------------------
// <copyright file="NewCategoryViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Framework.Filters;
using Fab.Client.Framework.Results;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Core.Framework;

namespace Fab.Client.MoneyTracker.Categories.Single
{
	/// <summary>
	/// View model for new account dialog.
	/// </summary>
	[Export(typeof (NewCategoryViewModel))]
	public class NewCategoryViewModel : LocalizableScreen, ICanBeBusy
	{
		#region Dependency

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly ICategoriesRepository repository = IoC.Get<ICategoriesRepository>();

		#endregion

		#region Id

		/// <summary>
		/// Gets or sets category ID for existing category (in "edit mode" only).
		/// </summary>
		public int? CategoryId { get; set; }

		#endregion

		#region Category name

		/// <summary>
		/// Category name.
		/// </summary>
		private string categoryName;

		/// <summary>
		/// Gets or sets category name.
		/// </summary>
		public string CategoryName
		{
			get { return categoryName; }
			set
			{
				if (categoryName != value)
				{
					categoryName = value;
					NotifyOfPropertyChange(() => CategoryName);
				}
			}
		}

		#endregion

		#region Category type

		/// <summary>
		/// Selected category type.
		/// </summary>
		private CategoryType selectedCategoryType;

		/// <summary>
		/// Gets or sets selected category type.
		/// </summary>
		public CategoryType SelectedCategoryType
		{
			get { return selectedCategoryType; }
			set
			{
				selectedCategoryType = value;
				NotifyOfPropertyChange(() => SelectedCategoryType);
			}
		}

		/// <summary>
		/// Gets all possible types of category.
		/// </summary>
		public EnumWrapperList<CategoryType> CategoryTypes { get; private set; }

		#endregion

		#region Is edit mode

		/// <summary>
		/// Indicate whether view is in "edit" mode for existing category.
		/// </summary>
		private bool isEditMode;

		/// <summary>
		/// Gets or sets a value indicating whether view is in "edit" mode for existing category.
		/// </summary>
		public bool IsEditMode
		{
			get { return isEditMode; }
			set
			{
				isEditMode = value;
				NotifyOfPropertyChange(() => IsEditMode);
			}
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="NewCategoryViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public NewCategoryViewModel()
		{
			CategoryTypes = new EnumWrapperList<CategoryType>();
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets the dialog title.
		/// </summary>
		public override string DisplayName
		{
			get { return IsEditMode ? Resources.Strings.CategoryView_Edit : Resources.Strings.NewCategoryView_New; }
		}

		#endregion

		#region Implementation of ICanBeBusy

		/// <summary>
		/// Gets or sets a value indicating weather a login view model has a long running operation in the background.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether a view model has a long running operation in the background.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets a value indicating whether all data for new category filled correctly.
		/// </summary>
		public bool CanSave
		{
			get { return !string.IsNullOrWhiteSpace(CategoryName); }
		}

		/// <summary>
		/// Create new category on server or update existing.
		/// </summary>
		/// <returns>Async operation result.</returns>
		[SetBusy]
		[Dependencies("CategoryName")]
		public IEnumerable<IResult> Save()
		{
			IsBusy = true;
			yield return new SingleResult
			             	{
			             		Action = () =>
			             		         	{
			             		         		if (IsEditMode)
			             		         		{
			             		         			if (CategoryId.HasValue)
			             		         			{
			             		         				repository.Update(CategoryId.Value, CategoryName.Trim(),
			             		         				                  SelectedCategoryType);
			             		         			}
			             		         			else
			             		         			{
			             		         				throw new Exception(
			             		         					"Category ID is not specified for \"Update\" operation.");
			             		         			}
			             		         		}
			             		         		else
			             		         		{
			             		         			repository.Create(CategoryName.Trim(), SelectedCategoryType);
			             		         		}
			             		         	}
			             	};

			IsBusy = false;

			Close();
		}

		/// <summary>
		/// Cancel category creation or edition.
		/// </summary>
		public void Cancel()
		{
			Close();
		}

		#endregion

		#region Private Method

		/// <summary>
		/// Close dialog and empty current dialog data.
		/// </summary>
		private void Close()
		{
			(Parent as IConductor).CloseItem(this);

			CategoryId = null;
			CategoryName = string.Empty;
			SelectedCategoryType = CategoryType.Common;

			IsEditMode = false;
		}

		#endregion
	}
}