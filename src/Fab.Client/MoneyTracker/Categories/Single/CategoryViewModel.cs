﻿//------------------------------------------------------------
// <copyright file="CategoryViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Framework.Results;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories.Single
{
	/// <summary>
	/// View model for single category.
	/// </summary>
	[Export(typeof(CategoryViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class CategoryViewModel : LocalizableScreen
	{
		#region Id

		/// <summary>
		/// Category unique ID.
		/// </summary>
		private int id;

		/// <summary>
		/// Gets or sets category unique ID.
		/// </summary>
		public int Id
		{
			get { return id; }
			set
			{
				if (id != value)
				{
					id = value;
					NotifyOfPropertyChange(() => Id);
				}
			}
		}

		#endregion

		#region Name

		/// <summary>
		/// Category name.
		/// </summary>
		private string name;

		/// <summary>
		/// Gets or sets category name.
		/// </summary>
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
					NotifyOfPropertyChange(() => Name);
				}
			}
		}

		#endregion

		#region Type

		/// <summary>
		/// Category type.
		/// </summary>
		private EnumWrapper<CategoryType> categoryTypeWrapper;

		/// <summary>
		/// Gets or sets category type.
		/// </summary>
		public EnumWrapper<CategoryType> CategoryTypeWrapper
		{
			get { return categoryTypeWrapper; }
			set
			{
				if (categoryTypeWrapper != value)
				{
					categoryTypeWrapper = value;
					NotifyOfPropertyChange(() => CategoryTypeWrapper);
				}
			}
		}

		#endregion

		#region Popularity

		/// <summary>
		/// Category popularity.
		/// </summary>
		private int popularity;

		/// <summary>
		/// Gets or sets category popularity.
		/// </summary>
		public int Popularity
		{
			get { return popularity; }
			set
			{
				if (popularity != value)
				{
					popularity = value;
					NotifyOfPropertyChange(() => Popularity);
				}
			}
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoryViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">The event aggregator to listen for the specific notifications.</param>
		/// <param name="categoriesRepository">Category repository to work with.</param>
		[ImportingConstructor]
		public CategoryViewModel(IEventAggregator eventAggregator, ICategoriesRepository categoriesRepository)
		{
			if (eventAggregator == null)
			{
				throw new ArgumentNullException("eventAggregator");
			}

			if (categoriesRepository == null)
			{
				throw new ArgumentNullException("categoriesRepository");
			}

			EventAggregator = eventAggregator;
			Repository = categoriesRepository;
			CategoryTypeWrapper = new EnumWrapper<CategoryType>(CategoryType.Common);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Delete category after confirmation.
		/// </summary>
		/// <returns>Result of async operation.</returns>
		public IEnumerable<IResult> Delete()
		{
			var openConfirmationResult = new OpenConfirmationResult(EventAggregator)
			{
				Message = string.Format(Resources.Strings.CategoryView_Delete_Confirmation_Message, Name),
				Title = Resources.Strings.CategoryView_Delete_Confirmation_Title,
				Options = MessageBoxOptions.Yes | MessageBoxOptions.Cancel,
			};

			yield return openConfirmationResult;

			if (openConfirmationResult.Selected == MessageBoxOptions.Yes)
			{
				Repository.Delete(Id);
			}
		}

		/// <summary>
		/// Edit category.
		/// </summary>
		/// <returns>Result of async operation.</returns>
		public IEnumerable<IResult> Edit()
		{
			yield return Animation.Stop("ShowActionsPanel");
			yield return Animation.Stop("HideActionsPanel");

			var shell = IoC.Get<IShell>();
			var categoryViewModel = IoC.Get<NewCategoryViewModel>();
			
			categoryViewModel.CategoryId = Id;
			categoryViewModel.CategoryName = Name;
			categoryViewModel.SelectedCategoryType = CategoryTypeWrapper.EnumValue;
			categoryViewModel.IsEditMode = true;
			
			shell.Dialogs.ShowDialog(categoryViewModel);
		}

		public IEnumerable<IResult> ShowActions()
		{
			yield return Animation.Stop("HideActionsPanel");
			yield return Animation.Begin("ShowActionsPanel");
		}

		public IEnumerable<IResult> HideActions()
		{
			yield return Animation.Stop("ShowActionsPanel");
			yield return Animation.Begin("HideActionsPanel");
		}

		#endregion

		#region Private

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		/// <summary>
		/// Gets or sets categories repository to work with.
		/// </summary>
		private ICategoriesRepository Repository { get; set; }

		#endregion
	}
}