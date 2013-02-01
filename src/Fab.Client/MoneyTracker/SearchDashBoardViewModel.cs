//------------------------------------------------------------
// <copyright file="SearchDashBoardViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;
using Fab.Client.MoneyTracker.Postings;
using Fab.Client.MoneyTracker.Postings.Transactions;
using Fab.Client.MoneyTracker.Postings.Transfers;
using Fab.Client.Resources.Icons;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// General search screen model.
	/// </summary>
	[Export(typeof (IModule))]
	public class SearchDashBoardViewModel : PostingViewModelBase, IModule, IHandle<LoggedOutMessage>, IHandle<AccountsUpdatedMessage>, IHandle<CategoriesUpdatedMessage>
	{
		#region Fields

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();

		/// <summary>
		/// Categories repository.
		/// </summary>
		private readonly ICategoriesRepository categoriesRepository = IoC.Get<ICategoriesRepository>();

		#endregion

		private readonly CollectionViewSource sourceAccountsViewSource = new CollectionViewSource();
		private readonly CollectionViewSource sourceCategoriesViewSource = new CollectionViewSource();

		public ICollectionView Accounts
		{
			get { return sourceAccountsViewSource.View; }
		}

		public ICollectionView Categories
		{
			get { return sourceCategoriesViewSource.View; }
		}

		private void InitSourceAccounts()
		{
			if (!Accounts.IsEmpty)
			{
				Accounts.MoveCurrentToFirst();
			}
		}

		private void InitSourceCategories()
		{
			if (!Categories.IsEmpty)
			{
				Categories.MoveCurrentToFirst();
			}
		}

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchDashBoardViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public SearchDashBoardViewModel(TransactionViewModel transactionDetails, TransferViewModel transferDetails)
			: base(transactionDetails, transferDetails)
		{
			Icon = new MagnifyIcon();

			TransactionRecords.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
			Translator.CultureChanged += delegate
			{
				StartDate = DateTime.Now;
				EndDate = StartDate;
				NotifyOfPropertyChange(() => Name);
				SearchStatus = Resources.Strings.PostingViewModelBase_SearchStatus_Search;
			};
			
			sourceAccountsViewSource.Source = accountsRepository.Entities;
			sourceCategoriesViewSource.Source = categoriesRepository.Entities;
		}

		#endregion

		#region Overrides of PostingViewModelBase

		protected override IEnumerable<IResult> PreAction()
		{
			// Initialize account for search postings
			AccountId = ((AccountDTO)Accounts.CurrentItem).Id;
			yield break;
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get
			{
				return TransactionRecords.Count > 0
						? string.Format(Resources.Strings.SearchDashBoardView_Name_Results, TransactionRecords.Count)
				       	: Resources.Strings.SearchDashBoardView_Name_Empty;
			}
		}

		private Control icon;

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
			get { return 3; }
		}

		public void Show()
		{
			if (!Accounts.IsEmpty && Accounts.CurrentItem == null)
			{
				Accounts.MoveCurrentToFirst();
			}

			if (!Categories.IsEmpty && Categories.CurrentItem == null)
			{
				Categories.MoveCurrentToFirst();
			}

			//TODO: make this method common for all IModels
			if (Parent is IHaveActiveItem && ((IHaveActiveItem) Parent).ActiveItem == this)
			{
				DisplayName = Name;
			}
			else
			{
				((IConductor) Parent).ActivateItem(this);
			}
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(LoggedOutMessage message)
		{
			Init();
		}

		#endregion

		#region Implementation of IHandle<AccountsUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountsUpdatedMessage message)
		{
			InitSourceAccounts();
		}

		#endregion

		#region Implementation of IHandle<AccountsUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(CategoriesUpdatedMessage message)
		{
			InitSourceCategories();
		}

		#endregion
	}
}