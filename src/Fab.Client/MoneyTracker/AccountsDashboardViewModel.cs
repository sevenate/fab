//------------------------------------------------------------
// <copyright file="AccountsDashboardViewModel.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Localization;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Accounts.Single;
using Fab.Client.MoneyTracker.Filters;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// Money module dashboard with accounts and transactions.
	/// </summary>
	[Export(typeof (IModule))]
	public class AccountsDashboardViewModel : LocalizableConductor<AccountViewModel>,
	                                      IModule,
	                                      IHandle<AccountUpdatedMessage>,
	                                      IHandle<AccountsUpdatedMessage>,
	                                      IHandle<AccountDeletedMessage>
	{
		#region Fields

		/// <summary>
		/// Gets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository repository = IoC.Get<IAccountsRepository>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets <see cref="PostingsFilterViewModel"/>.
		/// </summary>
		public PostingsFilterViewModel PostingsFilter { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsDashboardViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public AccountsDashboardViewModel(PostingsFilterViewModel postingsFilterVM)
		{
			PostingsFilter = postingsFilterVM;
			eventAggregator.Subscribe(this);
			ResetAccounts();
			Translator.CultureChanged += (sender, args) => NotifyOfPropertyChange(() => Name);
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return string.Format(Resources.Strings.AccountsDashboardView_Name_Counts, Items.Count); }
		}

		public void Show()
		{
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

		#region Create account

		/// <summary>
		/// Open "new account" dialog.
		/// </summary>
		public void CreateAccount()
		{
			var shell = IoC.Get<IShell>();
			var newAccountViewModel = IoC.Get<NewAccountViewModel>();
			shell.Dialogs.ShowDialog(newAccountViewModel);
		}

		#endregion

		#region Implementation of IHandle<AccountUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountUpdatedMessage message)
		{
			AccountViewModel accountViewModel = Items.Where(viewModel => viewModel.Id == message.Account.Id).SingleOrDefault();

			if (accountViewModel != null)
			{
				// Update existing
				// TODO: find a way to use any kind of "auto mapper" here
				accountViewModel.Name = message.Account.Name;
				accountViewModel.AssetTypeId = message.Account.AssetTypeId;
				accountViewModel.Balance = message.Account.Balance;
				accountViewModel.PostingsCount = message.Account.PostingsCount;
				accountViewModel.FirstPostingDate = message.Account.FirstPostingDate;
				accountViewModel.LastPostingDate = message.Account.LastPostingDate;
			}
			else
			{
				// Add new 
				Items.Add(message.Account.Map());
			}

			NotifyOfPropertyChange(() => Name);
		}

		#endregion

		#region Implementation of IHandle<AccountsUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountsUpdatedMessage message)
		{
			ResetAccounts();
		}

		#endregion

		#region Implementation of IHandle<AccountDeletedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountDeletedMessage message)
		{
			AccountViewModel accountViewModel = Items.Where(viewModel => viewModel.Id == message.Account.Id).SingleOrDefault();

			if (accountViewModel != null)
			{
				// Remove existing
				Items.Remove(accountViewModel);
			}

			NotifyOfPropertyChange(() => Name);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Clean current list of accounts and fill it with new items from repository.
		/// </summary>
		private void ResetAccounts()
		{
			Items.Clear();
			Items.AddRange(repository.Entities.Select(AccountsExtensions.Map));
			ActivateFirstItem();
			NotifyOfPropertyChange(() => Name);
		}

		/// <summary>
		/// Try to activate first account if there are any.
		/// </summary>
		private void ActivateFirstItem()
		{
			if (ActiveItem == null && Items.Count > 0)
			{
				ActivateItem(Items.First());
			}
		}

		#endregion
	}
}