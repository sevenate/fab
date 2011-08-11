// <copyright file="CategoriesViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-12" />
// <summary>Categories view model.</summary>

using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Categories view model.
	/// </summary>
	[Export(typeof(IModule))]
	public class CategoriesViewModel : Screen, IModule, IHandle<CategoriesUpdatedMessage>
	{
		#region Fields

		private readonly CollectionViewSource categoriesCollectionView = new CollectionViewSource();
		private readonly ICategoriesRepository categoriesRepository;

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		/// <summary>
		/// Gets categories for specific user.
		/// </summary>
		public ICollectionView Categories
		{
			get { return categoriesCollectionView.View; }
		}

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public CategoriesViewModel(IEventAggregator eventAggregator, ICategoriesRepository categoriesRepository)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);

			this.categoriesRepository = categoriesRepository;
			categoriesCollectionView.Source = categoriesRepository.Entities;
		}

		#endregion

		#region Implementation of IHandle<in CategoriesUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoriesUpdatedMessage message)
		{
			if (!categoriesCollectionView.View.IsEmpty)
			{
				categoriesCollectionView.View.MoveCurrentToFirst();
			}

			NotifyOfPropertyChange(() => Categories);
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return "Categories"; }
		}

		public void Show()
		{
			if (Parent is IHaveActiveItem && ((IHaveActiveItem)Parent).ActiveItem == this)
			{
				DisplayName = Name;
			}
			else
			{
				((IConductor)Parent).ActivateItem(this);
			}
		}

		#endregion

		#region Create Category

		private string categoryName;

		/// <summary>
		/// Gets or sets name for the new category.
		/// </summary>
		public string CategoryName
		{
			get { return categoryName; }
			set
			{
				categoryName = value;
				NotifyOfPropertyChange(CategoryName);
			}
		}

		/// <summary>
		/// Create new account for specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public void CreateCategory()
		{
			//TODO: customize category type here
			categoriesRepository.Create(CategoryName.Trim(), CategoryType.Common);
		}

		#endregion
	}
}