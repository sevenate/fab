// <copyright file="AccountsViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-04-11" />

using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Accounts view model.
	/// </summary>
	[Export(typeof(AccountsViewModel))]
	public class AccountsViewModel : Screen, IHandle<AccountsUpdatedMessage>
	{
		#region Fields

		private readonly BindableCollection<AccountDTO> accounts = new BindableCollection<AccountDTO>();

		private readonly CollectionViewSource accountsCollectionView = new CollectionViewSource();
		private readonly IAccountsRepository accountsRepository;

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		/// <summary>
		/// Gets accounts for specific user.
		/// </summary>
		public ICollectionView Accounts
		{
			get { return accountsCollectionView.View; }
		}

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public AccountsViewModel(IEventAggregator eventAggregator, IAccountsRepository accountsRepository)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);

			this.accountsRepository = accountsRepository;
			accountsCollectionView.Source = accounts;
			accountsCollectionView.View.CurrentChanged += (sender, args) => EventAggregator.Publish(new CurrentAccountChangedMessage
			                                                                                        {
			                                                                                        	CurrentAccount = accountsCollectionView.View.CurrentItem as AccountDTO
			                                                                                        });
		}

		#endregion

		#region Create Account

		private string name;

		/// <summary>
		/// Gets or sets name for the new account.
		/// </summary>
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				NotifyOfPropertyChange(Name);
			}
		}

		/// <summary>
		/// Create new account for specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public void CreateAccount()
		{
			accountsRepository.Create(Name.Trim(), 1);
		}

		#endregion

		#region Implementation of IHandle<in AccountsUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountsUpdatedMessage message)
		{
			if (message.Error == null)
			{
				accounts.Clear();
				accounts.AddRange(message.Accounts);
				accountsCollectionView.View.MoveCurrentToFirst();
			}
			else
			{
				//TODO: show error dialog here
			}
		}

		#endregion
	}
}