// <copyright file="AccountsRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Provide access to all user accounts.
	/// </summary>
	[Export(typeof (IAccountsRepository))]
	public class AccountsRepository : RepositoryBase, IAccountsRepository
	{
		#region Entities Collections

		/// <summary>
		/// List of all user accounts.
		/// </summary>
		private readonly List<AccountDTO> accounts = new List<AccountDTO>();

		/// <summary>
		/// Gets all user accounts.
		/// </summary>
		public IEnumerable<AccountDTO> Accounts
		{
			get { return accounts; }
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountsRepository"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public AccountsRepository(IEventAggregator eventAggregator)
			: base(eventAggregator)
		{
		}

		#endregion

		#region Implementation of IHandle<in LoggedInMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public override void Handle(LoggedInMessage message)
		{
			base.Handle(message);
			DownloadAll();
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public override void Handle(LoggedOutMessage message)
		{
			base.Handle(message);
			accounts.Clear();
			EventAggregator.Publish(new AccountsUpdatedMessage
			                        {
			                        	Accounts = Accounts
			                        });
		}

		#endregion

		#region Implementation of IAccountsRepository

		/// <summary>
		/// Download all <see cref="AccountDTO"/> entities from server.
		/// </summary>
		public void DownloadAll()
		{
			var proxy = new MoneyServiceClient();

			proxy.GetAllAccountsCompleted += (s, e) =>
			                                 {
			                                 	if (e.Error == null)
			                                 	{
			                                 		accounts.Clear();
			                                 		accounts.AddRange(e.Result);
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                 		                                                 {
			                                 		                                                 	Accounts = Accounts
			                                 		                                                 }));
			                                 	}
			                                 	else
			                                 	{
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                 		                                                 {
			                                 		                                                 	Error = e.Error
			                                 		                                                 }));
			                                 	}
			                                 };

			proxy.GetAllAccountsAsync(UserId);
		}

		/// <summary>
		/// Create new account for specific user.
		/// </summary>
		/// <param name="name">New account name.</param>
		/// <param name="assetTypeId">New account asset type.</param>
		/// <returns>Created account.</returns>
		public AccountDTO Create(string name, int assetTypeId)
		{
			var account = new AccountDTO
			              {
			              	Id = 0,
			              	Name = name,
			              	AssetTypeId = assetTypeId,
			              	Created = DateTime.UtcNow,
			              	Balance = 0,
			              	FirstPostingDate = null,
			              	LastPostingDate = null,
			              	PostingsCount = 0
			              };

			var proxy = new MoneyServiceClient();

			proxy.CreateAccountCompleted += (s, e) =>
			                                {
			                                	if (e.Error == null)
			                                	{
			                                		account.Id = e.Result;
			                                		accounts.Add(account);
			                                		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                		                                                 {
			                                		                                                 	Accounts = Accounts
			                                		                                                 }));
			                                	}
			                                	else
			                                	{
			                                		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                		                                                 {
			                                		                                                 	Error = e.Error
			                                		                                                 }));
			                                	}
			                                };

			proxy.CreateAccountAsync(UserId, name, assetTypeId);

			return account;
		}

		#endregion
	}
}