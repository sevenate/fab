// <copyright file="AccountsViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-04-11</date>
// </author>
// <editor name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-04-11</date>
// </editor>
// <summary>Accounts view model.</summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
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

		/// <summary>
		/// Accounts owner ID.
		/// </summary>
		private readonly Guid userId = new Guid("DC57BFF0-57A6-4BFC-9104-5F323ABBEDAB"); // 7F06BFA6-B675-483C-9BF3-F59B88230382

		private readonly BindableCollection<AccountDTO> accounts = new BindableCollection<AccountDTO>();

		private readonly CollectionViewSource accountsCollectionViewSource = new CollectionViewSource();

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsViewModel"/> class.
		/// </summary>
		public AccountsViewModel()
		{
			accountsCollectionViewSource.Source = accounts;
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

			var request = new AccountsResult(userId);
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
	}
}