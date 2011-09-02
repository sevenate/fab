// <copyright file="AccountsDashboardModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-02" />

using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Accounts.Single;
using Fab.Client.MoneyTracker.Filters;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// Money module dashboard with accounts and transactions.
	/// </summary>
	[Export(typeof(IModule))]
	public class AccountsDashboardModel : Conductor<AccountViewModel>.Collection.OneActive, IModule
	{
		#region Fields

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository repository = IoC.Get<IAccountsRepository>();

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return "Accounts"; }
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

		#region Properties

		/// <summary>
		/// Gets <see cref="PostingsFilterViewModel"/>.
		/// </summary>
		public PostingsFilterViewModel PostingsFilter { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsDashboardModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public AccountsDashboardModel(PostingsFilterViewModel postingsFilterVM)
		{
			PostingsFilter = postingsFilterVM;
			Items.AddRange(repository.Entities.Select(AccountsExtensions.Map));
			repository.Entities.CollectionChanged += OnEntitiesCollectionChanged;
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

					break;

				case NotifyCollectionChangedAction.Remove:
					foreach (var oldItem in eventArgs.OldItems)
					{
						var accountDTO = (AccountDTO)oldItem;
						var accountViewModel = Items.Where(a => a.Id == accountDTO.Id).Single();
						Items.Remove(accountViewModel);
					}

					break;

				case NotifyCollectionChangedAction.Replace:
					Items.Clear();
					Items.AddRange(repository.Entities.Select(AccountsExtensions.Map));
					ActivateFirstItem();
					break;

				case NotifyCollectionChangedAction.Reset:
					Items.Clear();
					Items.AddRange(repository.Entities.Select(AccountsExtensions.Map));
					ActivateFirstItem();
					break;
			}

			NotifyOfPropertyChange(() => Items);
		}

		#endregion

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

		/// <summary>
		/// Open new account dialog.
		/// </summary>
		public void CreateAccount()
		{
			var shell = IoC.Get<IShell>();
			var newAccountViewModel = IoC.Get<NewAccountViewModel>();
			shell.Dialogs.ShowDialog(newAccountViewModel);
		}
	}
}