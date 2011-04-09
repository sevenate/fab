// <copyright file="AccountsRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System;
using System.ComponentModel.Composition;
using System.Linq;
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
			var proxy = new MoneyServiceClient();

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
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                 		                                                 {
			                                 		                                                 	Error = e.Error
			                                 		                                                 }));
			                                 	}
			                                 };

			proxy.GetAllAccountsAsync(UserId);
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

			var proxy = new MoneyServiceClient();

			proxy.CreateAccountCompleted += (s, e) =>
			                                {
			                                	if (e.Error == null)
			                                	{
			                                		entity.Id = e.Result;
			                                		Entities.Add(entity);
			                                		Execute.OnUIThread(() => EventAggregator.Publish(new AccountsUpdatedMessage
			                                		                                                 {
			                                		                                                 	Accounts = Entities
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

			proxy.CreateAccountAsync(UserId, entity.Name, entity.AssetTypeId);

			return entity;
		}

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Updated entity data.</param>
		/// <returns>Updated entity with filled server-side updatable properties.</returns>
		public override AccountDTO Update(AccountDTO entity)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Entity to delete.</param>
		public override void Delete(AccountDTO entity)
		{
			throw new NotImplementedException();
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

			return Create(account);
		}

		#endregion
	}
}