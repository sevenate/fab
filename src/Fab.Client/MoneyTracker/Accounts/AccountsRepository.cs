// <copyright file="AccountsRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-03-26" />

using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Provide access to all user accounts.
	/// </summary>
	[Export(typeof (IAccountsRepository))]
	public class AccountsRepository : RepositoryBase<AccountDTO>, IAccountsRepository
	{
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

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public override void Handle(LoggedOutMessage message)
		{
			base.Handle(message);
			EventAggregator.Publish(new AccountsUpdatedMessage
			                        {
			                        	Accounts = Entities
			                        });
		}

		#endregion

		#region Overrides of RepositoryBase<AccountDTO>

		/// <summary>
		/// Retrieve account by unique key.
		/// </summary>
		/// <param name="key">Account key.</param>
		/// <returns>Corresponding account or null if not found.</returns>
		public override AccountDTO ByKey(int key)
		{
			return Entities.Where(a => a.Id == key).SingleOrDefault();
		}

		/// <summary>
		/// Download all <see cref="AccountDTO"/> entities from server.
		/// </summary>
		public override void Download()
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.GetAllAccountsCompleted += (s, e) =>
			                                 {
			                                 	if (e.Error == null)
			                                 	{
			                                 		Entities.Clear();
													Entities.AddRange(e.Result);
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                 		                                                 {
			                                 		                                                 	Accounts = Entities
			                                 		                                                 }));
			                                 	}
			                                 	else
			                                 	{
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
			                                 		                                                 {
			                                 		                                                 	Error = e.Error
			                                 		                                                 }));
			                                 	}

												EventAggregator.Publish(new AsyncOperationCompleteMessage());
											 };

			proxy.GetAllAccountsAsync(UserId);
			EventAggregator.Publish(new AsyncOperationStartedMessage{OperationName = "Downloading accounts"});
		}

		/// <summary>
		/// Download one entity from server.
		/// </summary>
		/// <param name="key">Entity key.</param>
		public override void Download(int key)
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.GetAccountCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					var account = ByKey(key);
					var index = Entities.IndexOf(account);
					Entities[index] = e.Result;

					Execute.OnUIThread(() => EventAggregator.Publish(new AccountUpdatedMessage
					{
						Account = e.Result
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
					{
						Error = e.Error
					}));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.GetAccountAsync(UserId, key);
			EventAggregator.Publish(new AsyncOperationStartedMessage{OperationName = "Downloading account #" + key});
		}

		/// <summary>
		/// Create new account.
		/// </summary>
		/// <param name="entity">New account data to create from.</param>
		/// <returns>Created account with filled server-side updated properties.</returns>
		public override AccountDTO Create(AccountDTO entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			var proxy = ServiceFactory.CreateMoneyService();
			proxy.CreateAccountCompleted += (s, e) =>
			                                {
			                                	if (e.Error == null)
			                                	{
			                                		entity.Id = e.Result;
			                                		Entities.Add(entity);
			                                		Execute.OnUIThread(() => EventAggregator.Publish(new AccountUpdatedMessage
			                                		                                                 {
																										 Account = entity
			                                		                                                 }));
			                                	}
			                                	else
			                                	{
													Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
			                                		                                                 {
			                                		                                                 	Error = e.Error
			                                		                                                 }));
			                                	}

												EventAggregator.Publish(new AsyncOperationCompleteMessage());
											};

			proxy.CreateAccountAsync(UserId, entity.Name, entity.AssetTypeId);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Creating new account \"" + entity.Name + "\"" });

			return entity;
		}

		/// <summary>
		/// Update existing account.
		/// </summary>
		/// <param name="entity">Updated account data.</param>
		/// <returns>Updated account with filled server-side updatable properties.</returns>
		public override AccountDTO Update(AccountDTO entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			var proxy = ServiceFactory.CreateMoneyService();
			proxy.UpdateAccountCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					// Update account in repository only if server update was successful
					var account = ByKey(entity.Id);
					account.Name = entity.Name;

					Execute.OnUIThread(() => EventAggregator.Publish(new AccountUpdatedMessage
					{
						Account = account
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
					{
						Error = e.Error
					}));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.UpdateAccountAsync(UserId, entity.Id, entity.Name);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Updating account \"" + entity.Name + "\"" });

			return entity;
		}

		/// <summary>
		/// Delete existing account.
		/// </summary>
		/// <param name="key">Account ID to delete.</param>
		public override void Delete(int key)
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.DeleteAccountCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					var deletedAccount = ByKey(key);
					Entities.Remove(deletedAccount);

					Execute.OnUIThread(() => EventAggregator.Publish(new AccountDeletedMessage
					{
						Account = deletedAccount
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
					{
						Error = e.Error
					}));
				}

				EventAggregator.Publish(new AsyncOperationCompleteMessage());
			};

			proxy.DeleteAccountAsync(UserId, key);
			EventAggregator.Publish(new AsyncOperationStartedMessage { OperationName = "Deleting account #" + key});
		}

		#endregion

		#region Implementation of IAccountsRepository

		/// <summary>
		/// Create new account for specific user.
		/// </summary>
		/// <param name="name">New account name.</param>
		/// <param name="assetTypeId">New account asset type.</param>
		/// <returns>Created account.</returns>
		public AccountDTO Create(string name, int assetTypeId)
		{
			var accountDTO = new AccountDTO
			                 {
			                 	AssetTypeId = assetTypeId,
			                 	Balance = 0,
			                 	Created = DateTime.UtcNow,
			                 	FirstPostingDate = null,
			                 	Id = 0,
			                 	LastPostingDate = null,
			                 	Name = name,
			                 	PostingsCount = 0
			                 };

			return Create(accountDTO);
		}

		/// <summary>
		/// Update account.
		/// </summary>
		/// <param name="id">Existing account id.</param>
		/// <param name="name">New account name.</param>
		/// <returns>Updated account.</returns>
		public AccountDTO Update(int id, string name)
		{
			var account = new AccountDTO
			{
				Id = id,
				Name = name,
			};

			return Update(account);
		}

		#endregion
	}
}