// <copyright file="AccountsViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-04-11" />

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Accounts view model.
	/// </summary>
	[Export(typeof(IAccountsViewModel))]
	public class AccountsViewModel : Screen, IAccountsViewModel
	{
		#region Fields

		private readonly BindableCollection<AccountDTO> accounts = new BindableCollection<AccountDTO>();

		private readonly CollectionViewSource accountsCollectionViewSource = new CollectionViewSource();

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public AccountsViewModel(IEventAggregator eventAggregator)
		{
			accountsCollectionViewSource.Source = accounts;
			eventAggregator.Subscribe(this);
		}

		#endregion

		#region Imlementation of IAccountsViewModel

		/// <summary>
		/// Gets accounts for specific user.
		/// </summary>
		public ICollectionView Accounts
		{
			get
			{
				return accountsCollectionViewSource.View;
			}
		}

		/// <summary>
		/// Download all accounts for specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public IEnumerable<IResult> LoadAllAccounts()
		{
			yield return Loader.Show("Loading...");

			var request = new AccountsResult(UserCredentials.Current.UserId);
			yield return request;
			
			accounts.Clear();
			accounts.AddRange(request.Accounts);
			accountsCollectionViewSource.View.MoveCurrentToFirst();

			if (Reloaded != null)
			{
				Reloaded(this, EventArgs.Empty);
			}

			yield return Loader.Hide();
		}

		/// <summary>
		/// Raised right after accounts were reloaded from server.
		/// </summary>
		public event EventHandler<EventArgs> Reloaded;

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the <see cref="LoggedOutMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="LoggedOutMessage"/>.</param>
		public void Handle(LoggedOutMessage message)
		{
			accounts.Clear();
		}

		#endregion
	}
}