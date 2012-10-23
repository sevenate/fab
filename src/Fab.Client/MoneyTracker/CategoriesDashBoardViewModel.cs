//------------------------------------------------------------
// <copyright file="CategoriesDashBoardViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Windows.Controls;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Categories;
using Fab.Client.Resources.Icons;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// General categories screen model.
	/// </summary>
	[Export(typeof(IModule))]
	public class CategoriesDashBoardViewModel : Conductor<CategoriesViewModel>.Collection.AllActive, IModule
	{
		#region Fields

		/// <summary>
		/// Category repository.
		/// </summary>
		private readonly ICategoriesRepository repository = IoC.Get<ICategoriesRepository>();

		#endregion

		#region Common categories

		private CategoriesViewModel commonCategories;

		public CategoriesViewModel CommonCategories
		{
			get { return commonCategories; }
			set {
				if (commonCategories != value)
				{
					commonCategories = value;
					NotifyOfPropertyChange(() => CommonCategories);
				}
			}
		}

		#endregion

		#region Withdrawal categories

		private CategoriesViewModel withdrawalCategories;

		public CategoriesViewModel WithdrawalCategories
		{
			get { return withdrawalCategories; }
			set
			{
				if (withdrawalCategories != value)
				{
					withdrawalCategories = value;
					NotifyOfPropertyChange(() => WithdrawalCategories);
				}
			}
		}

		#endregion

		#region Deposit categories

		private CategoriesViewModel depositCategories;
		private Control icon;

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
		public CategoriesDashBoardViewModel()
		{
			Icon = new CategoryIcon();

			CommonCategories = IoC.Get<CategoriesViewModel>();
			CommonCategories.CategoryType = CategoryType.Common;

			WithdrawalCategories = IoC.Get<CategoriesViewModel>();
			WithdrawalCategories.CategoryType = CategoryType.Withdrawal;

			DepositCategories = IoC.Get<CategoriesViewModel>();
			DepositCategories.CategoryType = CategoryType.Deposit;

			repository.Entities.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
			Translator.CultureChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return string.Format(Resources.Strings.CategoriesDashBoardView_Name_Counts, repository.Entities.Count); }
		}

		public Control Icon
		{
			get { return icon; }
			private set { icon = value; }
		}

		/// <summary>
		/// Determine order in UI representation (like position in ItemsControl.Items).
		/// </summary>
		public int Order
		{
			get { return 2; }
		}

		public void Show()
		{
			//TODO: make this method common for all IModels
//			if (Parent is IHaveActiveItem && ((IHaveActiveItem)Parent).ActiveItem == this)
//			{
//				DisplayName = Name;
//			}
//			else
//			{
//				((IConductor)Parent).ActivateItem(this);
//			}
		}

		#endregion
	}
}