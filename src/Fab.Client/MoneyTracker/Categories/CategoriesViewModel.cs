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
	[Export(typeof(IModule))]
	public class CategoriesViewModel : Conductor<CategoryViewModel>.Collection.AllActive, IModule
	{
		#region Fields

		/// <summary>
		/// Category repository.
		/// </summary>
		private readonly ICategoriesRepository repository = IoC.Get<ICategoriesRepository>();

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public CategoriesViewModel()
		{
			Items.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => Name); 
			Items.AddRange(repository.Entities.Select(CategoriesExtensions.Map));
			repository.Entities.CollectionChanged += OnEntitiesCollectionChanged;
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return "Categories (" + Items.Count + ")"; }
		}

		public void Show()
		{
			//TODO: make this method common for all IModels
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

		#region Event Handlers

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
					int i = eventArgs.NewStartingIndex;

					foreach (var newItem in eventArgs.NewItems)
					{
						Items.Insert(i++, ((CategoryDTO)newItem).Map());
					}

					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (var oldItem in eventArgs.OldItems)
					{
						var accountDTO = (CategoryDTO)oldItem;
						var accountViewModel = Items.Where(a => a.Id == accountDTO.Id).Single();
						Items.Remove(accountViewModel);
					}

					break;

				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Reset:
					Items.Clear();
					Items.AddRange(repository.Entities.Select(CategoriesExtensions.Map));
					break;
			}

			NotifyOfPropertyChange(() => Items);
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
			shell.Dialogs.ShowDialog(newAccountViewModel);
		}

		#endregion
	}
}