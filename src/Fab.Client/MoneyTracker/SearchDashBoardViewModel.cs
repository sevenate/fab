//------------------------------------------------------------
// <copyright file="SearchDashBoardViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// General search screen model.
	/// </summary>
	[Export(typeof(IModule))]
	public class SearchDashBoardViewModel : Conductor<CategoriesViewModel>.Collection.AllActive, IModule
	{
		#region Fields

		/// <summary>
		/// Category repository.
		/// </summary>
		private readonly ICategoriesRepository repository = IoC.Get<ICategoriesRepository>();

		#endregion

		#region Deposit categories

		private CategoriesViewModel depositCategories;

		public CategoriesViewModel DepositCategories
		{
			get { return depositCategories; }
			set
			{
				if (depositCategories != value)
				{
					depositCategories = value;
					NotifyOfPropertyChange(() => DepositCategories);
				}
			}
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Caliburn.Micro.Conductor`1.Collection.AllActive"/> class.
		/// </summary>
		[ImportingConstructor]
		public SearchDashBoardViewModel()
		{
			DepositCategories = IoC.Get<CategoriesViewModel>();
			DepositCategories.CategoryType = CategoryType.Deposit;

			repository.Entities.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return "Search (" + repository.Entities.Count + ")"; }
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
	}
}