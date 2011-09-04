//---------------------------------------------------------------------------
// <copyright file="NewCategoryViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-09-03" />
//---------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Framework.Filters;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// View model for new account dialog.
	/// </summary>
	[Export(typeof (NewCategoryViewModel))]
	public class NewCategoryViewModel : Screen, ICanBeBusy
	{
		#region Private

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly ICategoriesRepository repository = IoC.Get<ICategoriesRepository>();

		#endregion

		#region Name

		/// <summary>
		/// New category name.
		/// </summary>
		private string name;

		/// <summary>
		/// Gets or sets new category name.
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

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="NewCategoryViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public NewCategoryViewModel(IEventAggregator eventAggregator)
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
			get { return "Create new category"; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create new category on server.
		/// </summary>
		/// <returns>Async operation result.</returns>
		[SetBusy]
		[Dependencies("Name")]
		public IEnumerable<IResult> Create()
		{
			repository.Create(Name.Trim(), SelectedCategoryType);
			Cancel();
			yield break;
		}

		/// <summary>
		/// Check new category name for empty string.
		/// </summary>
		/// <returns><c>true</c> if the name is not empty.</returns>
		public bool CanCreate()
		{
			return !string.IsNullOrWhiteSpace(Name);
		}

		/// <summary>
		/// Cancel category creation.
		/// </summary>
		public void Cancel()
		{
			Name = string.Empty;
			(Parent as IConductor).CloseItem(this);
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
	}
}