//------------------------------------------------------------
// <copyright file="SearchDashBoardViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Postings;
using Fab.Client.MoneyTracker.Postings.Transactions;
using Fab.Client.MoneyTracker.Postings.Transfers;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// General search screen model.
	/// </summary>
	[Export(typeof (IModule))]
	public class SearchDashBoardViewModel : PostingViewModelBase, IModule, IHandle<LoggedOutMessage>
	{
		#region Fields

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();

		#endregion

		private readonly CollectionViewSource sourceAccountsViewSource = new CollectionViewSource();

		public ICollectionView Accounts
		{
			get { return sourceAccountsViewSource.View; }
		}

		private void InitSourceAccounts()
		{
			sourceAccountsViewSource.Source = accountsRepository.Entities;

			if (!Accounts.IsEmpty)
			{
				Accounts.MoveCurrentToFirst();
			}
		}

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Caliburn.Micro.Conductor`1.Collection.AllActive"/> class.
		/// </summary>
		[ImportingConstructor]
		public SearchDashBoardViewModel(TransactionViewModel transactionDetails, TransferViewModel transferDetails)
			: base(transactionDetails, transferDetails)
		{
			TransactionRecords.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
			Translator.CultureChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
			InitSourceAccounts();
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

		public void Show()
		{
			if (!Accounts.IsEmpty && Accounts.CurrentItem == null)
			{
				Accounts.MoveCurrentToFirst();
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

		#region Overrides of PostingViewModelBase

		protected override IEnumerable<IResult> PreAction()
		{
			// Initialize account for search postings
			AccountId = ((AccountDTO)Accounts.CurrentItem).Id;
			yield break;
		}

		#endregion
	}
}