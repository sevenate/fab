// <copyright file="CategoriesViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-12" />
// <summary>Categories view model.</summary>

using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Categories view model.
	/// </summary>
	[Export(typeof(CategoriesViewModel))]
	public class CategoriesViewModel : Screen, IHandle<CategoriesUpdatedMessage>
	{
		#region Fields

		private readonly BindableCollection<CategoryDTO> categories = new BindableCollection<CategoryDTO>();
		private readonly CollectionViewSource categoriesViewSource = new CollectionViewSource();
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
			get { return categoriesViewSource.View; }
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
			categoriesViewSource.Source = categories;
		}

		#endregion

		#region Implementation of IHandle<in CategoriesUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoriesUpdatedMessage message)
		{
			if (message.Error == null)
			{
				categories.Clear();
				categories.AddRange(message.Categories);

				if (!categoriesViewSource.View.IsEmpty)
				{
					categoriesViewSource.View.MoveCurrentToFirst();
				}
			}
			else
			{
				//TODO: show error dialog here
			}
		}

		#endregion
	}
}