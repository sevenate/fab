// <copyright file="AccountsViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-04-11" />

using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts.Single;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Accounts view model.
	/// </summary>
	[Export(typeof(AccountsViewModel))]
	public class AccountsViewModel : Conductor<AccountViewModel>.Collection.OneActive
	{
		#region Fields

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository repository;

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsViewModel"/> class.
		/// </summary>
		/// <param name="accountsRepository">The accounts repository to work with.</param>
		[ImportingConstructor]
		public AccountsViewModel(IAccountsRepository accountsRepository)
		{
			repository = accountsRepository;
			Items.AddRange(accountsRepository.Entities.Select(AccountsExtensions.Map));
			repository.Entities.CollectionChanged += OnEntitiesCollectionChanged;
		}

		#endregion

		#region Create Account

		/// <summary>
		/// Name for new account.
		/// </summary>
		private string accountName;

		/// <summary>
		/// Gets or sets name for the new account.
		/// </summary>
		public string AccountName
		{
			get { return accountName; }
			set
			{
				accountName = value;
				NotifyOfPropertyChange(AccountName);
			}
		}

		/// <summary>
		/// Create new account for specific user.
		/// </summary>
		public void CreateAccount()
		{
			//TODO: customize account asset type here
			repository.Create(AccountName.Trim(), 1);
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
						Items.Insert(i++, ((AccountDTO)newItem).Map());
					}

					ActivateFirstItem();

					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (var oldItem in eventArgs.OldItems)
					{
						var accountDTO = (AccountDTO) oldItem;
						var accountViewModel = Items.Where(a => a.Id == accountDTO.Id).Single();
						Items.Remove(accountViewModel);
					}

					break;

				case NotifyCollectionChangedAction.Replace:
					Items.AddRange(repository.Entities.Select(AccountsExtensions.Map));
					ActivateFirstItem();
					break;

				case NotifyCollectionChangedAction.Reset:
					Items.AddRange(repository.Entities.Select(AccountsExtensions.Map));
					ActivateFirstItem();
					break;
			}

			NotifyOfPropertyChange(() => Items);
		}

		#endregion

		/// <summary>
		/// Try to activate first item if there are any.
		/// </summary>
		private void ActivateFirstItem()
		{
			if (ActiveItem == null && Items.Count > 0)
			{
				ActivateItem(Items.First());
			}
		}
	}
}