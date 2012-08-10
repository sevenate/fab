//------------------------------------------------------------
// <copyright file="MoneyService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using EmitMapper;
using Fab.Core;
using Fab.Server.Core.Contracts;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Enums;
using Fab.Server.Core.Filters;

namespace Fab.Server.Core.Services
{
	/// <summary>
	/// Money service.
	/// </summary>
	[ErrorHandlingBehavior]
	public class MoneyService : IMoneyService
	{
		#region Dependencies

		/// <summary>
		/// Database manager dependency.
		/// </summary>
		private readonly DatabaseManager dbManager;

		#endregion

		#region Default folder

		/// <summary>
		/// Gets or sets default root folder for master and personal databases = |DataDirectory|.
		/// </summary>
		public string DefaultFolder { private get; set; }

		#endregion

		#region Current user

		/// <summary>
		/// Primary identity name.
		/// </summary>
		private string userName;

		/// <summary>
		/// Gets or sets primary identity name.
		/// </summary>
		public string UserName
		{
			private get
			{
				return ServiceSecurityContext.Current == null || ServiceSecurityContext.Current.IsAnonymous
				       	? userName
				       	: ServiceSecurityContext.Current.PrimaryIdentity.Name;
			}
			set { userName = value; }
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="MoneyService"/> class.
		/// </summary>
		public MoneyService()
		{
			DefaultFolder = "|DataDirectory|";
			dbManager = new DatabaseManager();
		}

		#endregion

		#region Implementation of IMoneyService

		#region Accounts

	    /// <summary>
	    /// Create new account.
	    /// </summary>
	    /// <param name="name">Account name.</param>
	    /// <param name="assetTypeId">The asset type ID.</param>
	    /// <returns>Created account ID.</returns>
	    public int CreateAccount(string name, int assetTypeId)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Account name must not be empty.");
			}

			// Check account name max length
			if (name.Length > 50)
			{
				throw new Exception("Account name is too long. Maximum length is 50.");
			}

			using (var pc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var assetType = ModelHelper.GetAssetTypeById(pc, assetTypeId);

				var account = new Account
				              {
				              	AssetType = assetType,
				              	Name = name,
				              	Created = DateTime.UtcNow,
				              	Balance = 0,
				              	IsClosed = false,
				              	ClosedChanged = null,
								PostingsCount = 0,
								FirstPostingDate = null,
								LastPostingDate = null,
							  };

				pc.Accounts.AddObject(account);
				pc.SaveChanges();

				return account.Id;
			}
		}

	    /// <summary>
	    /// Retrieve specific accounts by ID.
	    /// </summary>
	    /// <param name="accountId">Account ID to retrieve.</param>
	    /// <returns>Account data transfer object.</returns>
	    public AccountDTO GetAccount(int accountId)
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var accountMapper = ObjectMapperManager.DefaultInstance.GetMapper<Account, AccountDTO>(AccountMapper.AccountMappingConfigurator);
				var result = accountMapper.Map(ModelHelper.GetAccountById(mc, accountId));
				return AccountMapper.SetUTCforAccountNullableDates(result);
			}
		}

	    /// <summary>
	    /// Update account details to new values.
	    /// </summary>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="name">Account new name.</param>
	    public void UpdateAccount(int accountId, string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Account name must not be empty.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var account = ModelHelper.GetAccountById(mc, accountId);
				account.Name = name;
				mc.SaveChanges();
			}
		}

	    /// <summary>
	    /// Mark account as "deleted".
	    /// </summary>
	    /// <param name="accountId">Account ID to mark as deleted.</param>
	    public void DeleteAccount(int accountId)
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var account = ModelHelper.GetAccountById(mc, accountId);

				account.IsClosed = true;
				account.ClosedChanged = DateTime.UtcNow;

				mc.SaveChanges();
			}
		}

		/// <summary>
		/// Retrieve all accounts for user.
		/// </summary>
		/// <returns>All accounts.</returns>
		public IList<AccountDTO> GetAllAccounts()
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var accountMapper = ObjectMapperManager.DefaultInstance.GetMapper<Account, AccountDTO>(AccountMapper.AccountMappingConfigurator);

				var result = mc.Accounts.Include("AssetType")
					.Where(a => !a.IsSystem && !a.IsClosed)
					.OrderBy(a => a.Created)
					.ToList()
					.Select(accountMapper.Map)
					.ToList();

				foreach (var account in result)
				{
					AccountMapper.SetUTCforAccountNullableDates(account);
				}

				return result;
			}
		}

	    /// <summary>
	    /// Gets account balance before specific date.
	    /// </summary>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="dateTime">Specific date.</param>
	    /// <returns>Account balance at the specific date.</returns>
	    public decimal GetAccountBalance(int accountId, DateTime dateTime)
		{
			decimal balance;

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				// Todo: Fix Sum() of Postings when there is no any posting yet.
				var firstPosting = mc.Postings.FirstOrDefault(p => p.Account.Id == accountId && p.Date < dateTime);

				balance = firstPosting != null
				          	? mc.Postings.Where(p => p.Account.Id == accountId && p.Date < dateTime)
				          	  	.Sum(p => p.Amount)
				          	: 0;
			}

			return balance;
		}

		#endregion

		#region Categories

	    /// <summary>
	    /// Create new category.
	    /// </summary>
	    /// <param name="name">Category name.</param>
	    /// <param name="categoryType">Category type.</param>
	    /// <returns>Created category ID.</returns>
	    public int CreateCategory(string name, CategoryType categoryType)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Category name must not be empty.");
			}

			// Check category name max length
			if (name.Length > 50)
			{
				throw new Exception("Category name is too long. Maximum length is 50.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var category = new Category
				               {
				               	Name = name,
				               	CategoryType = (byte) categoryType,
				               	Popularity = 0,
				               	Deleted = null,
				               };

				mc.Categories.AddObject(category);
				mc.SaveChanges();

				return category.Id;
			}
		}

	    /// <summary>
	    /// Retrieve specific category by ID.
	    /// </summary>
	    /// <param name="categoryId">Category ID to retrieve.</param>
	    /// <returns>Category data transfer object.</returns>
	    public CategoryDTO GetCategory(int categoryId)
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var categoryMapper = ObjectMapperManager.DefaultInstance.GetMapper<Category, CategoryDTO>();
				return categoryMapper.Map(ModelHelper.GetCategoryById(mc, categoryId));
			}
		}

	    /// <summary>
	    /// Update category details to new values.
	    /// </summary>
	    /// <param name="categoryId">Category ID.</param>
	    /// <param name="name">Category new name.</param>
	    /// <param name="categoryType">Category new type.</param>
	    public void UpdateCategory(int categoryId, string name, CategoryType categoryType)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Category name must not be empty.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var category = ModelHelper.GetCategoryById(mc, categoryId);
				category.Name = name;
				category.CategoryType = (byte) categoryType;
				mc.SaveChanges();
			}
		}

	    /// <summary>
	    /// Mark category as "deleted".
	    /// </summary>
	    /// <param name="categoryId">Category ID to mark as deleted.</param>
	    public void DeleteCategory(int categoryId)
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var category = ModelHelper.GetCategoryById(mc, categoryId);
				category.Deleted = DateTime.UtcNow;
				mc.SaveChanges();
			}
		}

	    /// <summary>
	    /// Retrieve all categories for user.
	    /// </summary>
	    /// <returns>All categories.</returns>
	    public IList<CategoryDTO> GetAllCategories()
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var categoryMapper = ObjectMapperManager.DefaultInstance.GetMapper<Category, CategoryDTO>();
				var categories = mc.Categories.Where(c => c.Deleted == null)
											  .OrderBy(c => c.Name)
											  .ToList();

				return categories.Select(categoryMapper.Map).ToList();
			}
		}

		#endregion

		#region Journals

	    /// <summary>
	    /// Delete specific journal record.
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="journalId">Journal ID.</param>
	    public void DeleteJournal(int accountId, int journalId)
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				ModelHelper.DeleteJournal(mc, journalId);
				mc.SaveChanges();
			}
		}

	    /// <summary>
	    /// Return single journal record (either <see cref="TransactionDTO"/> or <see cref="TransferDTO"/>).
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="journalId">Journal ID.</param>
	    /// <returns>Journal record details.</returns>
	    public JournalDTO GetJournal(int accountId, int journalId)
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				/*
				var accountMapper = ObjectMapperManager.DefaultInstance.GetMapper<Account, AccountDTO>();
				var postingMapper = ObjectMapperManager.DefaultInstance.GetMapper<Posting, PostingDTO>();
				var categoryMapper = ObjectMapperManager.DefaultInstance.GetMapper<Category, CategoryDTO>();
				var transactionMapper = ObjectMapperManager.DefaultInstance.GetMapper<Journal, TransactionDTO>(
										new DefaultMapConfig()
										.ConvertUsing<Account, AccountDTO>(accountMapper.Map)
										.ConvertUsing<EntityCollection<Posting>, List<PostingDTO>>(postings => postings.Select(postingMapper.Map).ToList())
										.ConvertUsing<Posting, PostingDTO>(postingMapper.Map)
										.ConvertUsing<Category, CategoryDTO>(categoryMapper.Map));
				 */

				// Todo: add user ID account ID to the GetJournalById() method call to
				// join them with transaction ID to prevent unauthorized delete 
				// Do this for all user-aware calls (i.e. Categories, Accounts etc.)

				/*
				return transactionMapper.Map(ModelHelper.GetJournalById(mc, journalId));
				 */

				var journal = ModelHelper.GetJournalById(mc, journalId);

				var posting = journal.Postings.Single(p => p.Account.Id == accountId);

				switch (journal.JournalType)
				{
					case (byte) JournalType.Deposit:
						return new DepositDTO
						       {
						       	Amount = posting.Amount,
						       	CategoryId =
						       		journal.Category != null && journal.Category.Deleted == null ? journal.Category.Id : (int?) null,
						       	Comment = journal.Comment,
						       	Date = posting.Date.IsUtc(),
						       	Id = journal.Id,
						       	Quantity = journal.Quantity,
						       	Rate = journal.Rate
						       };

					case (byte) JournalType.Withdrawal:
						return new WithdrawalDTO
						       {
						       	Amount = posting.Amount,
						       	CategoryId =
						       		journal.Category != null && journal.Category.Deleted == null ? journal.Category.Id : (int?) null,
						       	Comment = journal.Comment,
								Date = posting.Date.IsUtc(),
						       	Id = journal.Id,
						       	Quantity = journal.Quantity,
						       	Rate = journal.Rate
						       };

					case (byte) JournalType.Transfer:
					{
						var posting2 = journal.Postings.Single(p => p.Account.Id != accountId);

						return posting.Amount >= 0
						       	? (JournalDTO) new IncomingTransferDTO
						       	               {
						       	               	Amount = posting.Amount,
						       	               	Comment = journal.Comment,
												Date = posting.Date.IsUtc(),
						       	               	Id = journal.Id,
						       	               	Quantity = journal.Quantity,
						       	               	Rate = journal.Rate,
						       	               	SecondAccountId = posting2.Account.Id
						       	               }
						       	: new OutgoingTransferDTO
						       	  {
						       	  	Amount = posting.Amount,
						       	  	Comment = journal.Comment,
									Date = posting.Date.IsUtc(),
						       	  	Id = journal.Id,
						       	  	Quantity = journal.Quantity,
						       	  	Rate = journal.Rate,
						       	  	SecondAccountId = posting2.Account.Id
						       	  };
					}

					default:
						throw new NotSupportedException(string.Format("Unknown journal type - {0}.", journal.JournalType));
				}
			}
		}

	    /// <summary>
	    /// Return filtered journal records count.
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="queryFilter">Specify conditions for filtering journal records.</param>
	    /// <returns>Filtered journal records count.</returns>
	    public int GetJournalsCount(int accountId, IQueryFilter queryFilter)
		{
			if (queryFilter == null)
			{
				throw new ArgumentNullException("queryFilter");
			}

			int count;

			// Bug: warning security weakness!
			// Check User.IsDisabled + Account.IsDeleted also
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				//TODO: remove duplicated code with GetJournals() method
				var query = from p in mc.Postings
				            where p.Account.Id == accountId
				            orderby p.Date, p.Journal.Id
				            select new
				                   {
				                   	Posting = p,
				                   	p.Journal,
				                   	p.Journal.Category,
				                   	p.Journal.JournalType
				                   };

				if (queryFilter is TextSearchFilter)
				{
					var textSearchFilter = queryFilter as TextSearchFilter;

					if (!string.IsNullOrEmpty(textSearchFilter.Contains))
					{
						query = from t in query
								where t.Journal.Comment.Contains(textSearchFilter.Contains)
								select t;
					}
				}

				if (queryFilter is CategoryFilter)
				{
					var categoryFilter = queryFilter as CategoryFilter;

					if (categoryFilter.CategoryId.HasValue)
					{
						query = from c in query
								where c.Category != null && c.Category.Id == categoryFilter.CategoryId.Value
								select c;
					}
				}

				if (queryFilter.NotOlderThen.HasValue)
				{
					query = from t in query
							where t.Posting.Date >= queryFilter.NotOlderThen.Value
					        select t;
				}

				if (queryFilter.Upto.HasValue)
				{
					query = from t in query
							where t.Posting.Date < queryFilter.Upto.Value
					        select t;
				}

				// Todo: consider dynamic order specification
				//query = query.OrderBy(t => t.Posting.Date);

				if (queryFilter.Skip.HasValue)
				{
					query = query.Skip(queryFilter.Skip.Value);
				}

				if (queryFilter.Take.HasValue)
				{
					query = query.Take(queryFilter.Take.Value);
				}
				// End of duplicated code

				count = query.Count();
			}

			return count;
		}

	    /// <summary>
	    /// Return filtered list of journal records for specific account.
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="queryFilter">Specify conditions for filtering journal records.</param>
	    /// <returns>List of journal records.</returns>
	    public IList<JournalDTO> GetJournals(int accountId, IQueryFilter queryFilter)
		{
			if (queryFilter == null)
			{
				throw new ArgumentNullException("queryFilter");
			}

			var records = new List<JournalDTO>();

			// Bug: warning security weakness!
			// Check User.IsDisabled + Account.IsDeleted also
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				//TODO: remove duplicated code with GetJournalsCounts() method
				var query = from p in mc.Postings
				            where p.Account.Id == accountId
				            orderby p.Date, p.Journal.Id
				            select new
				                   {
				                   	Posting = p,
				                   	p.Journal,
				                   	p.Journal.Category,
				                   	p.Journal.JournalType
				                   };

				if (queryFilter is TextSearchFilter)
				{
					var textSearchFilter = queryFilter as TextSearchFilter;

					if (!string.IsNullOrEmpty(textSearchFilter.Contains))
					{
						query = from t in query
								where t.Journal.Comment.Contains(textSearchFilter.Contains)
						        select t;
					}
				}

				if (queryFilter is CategoryFilter)
				{
					var categoryFilter = queryFilter as CategoryFilter;

					if (categoryFilter.CategoryId.HasValue)
					{
						query = from c in query
								where c.Category != null && c.Category.Id == categoryFilter.CategoryId
								select c;
					}
				}

				if (queryFilter.NotOlderThen.HasValue)
				{
					query = from t in query
					        where t.Posting.Date >= queryFilter.NotOlderThen.Value
					        select t;
				}

				if (queryFilter.Upto.HasValue)
				{
					query = from t in query
					        where t.Posting.Date < queryFilter.Upto.Value
					        select t;
				}

				// Todo: consider dynamic order specification
				//query = query.OrderBy(t => t.Posting.Date);

				if (queryFilter.Skip.HasValue)
				{
					query = query.Skip(queryFilter.Skip.Value);
				}

				if (queryFilter.Take.HasValue)
				{
					query = query.Take(queryFilter.Take.Value);
				}
				// End of duplicated code

				var res = query.ToList();

				// No transactions take place yet, so nothing to return
				if (res.Count == 0)
				{
					return records;
				}

				foreach (var r in res)
				{
					switch (r.JournalType)
					{
						case (byte) JournalType.Deposit:
							records.Add(new DepositDTO
							            {
							            	Amount = r.Posting.Amount,
							            	CategoryId = r.Category != null && r.Category.Deleted == null ? r.Category.Id : (int?) null,
							            	Comment = r.Journal.Comment,
											Date = r.Posting.Date.IsUtc(),
							            	Id = r.Journal.Id,
							            	Quantity = r.Journal.Quantity,
							            	Rate = r.Journal.Rate
							            });
							break;

						case (byte) JournalType.Withdrawal:
							records.Add(new WithdrawalDTO
							            {
							            	Amount = r.Posting.Amount,
							            	CategoryId = r.Category != null && r.Category.Deleted == null ? r.Category.Id : (int?) null,
							            	Comment = r.Journal.Comment,
											Date = r.Posting.Date.IsUtc(),
							            	Id = r.Journal.Id,
							            	Quantity = r.Journal.Quantity,
							            	Rate = r.Journal.Rate
							            });
							break;

						case (byte) JournalType.Transfer:
							var t = r.Posting.Amount >= 0
							        	? (TransferDTO) new IncomingTransferDTO
							        	                {
							        	                	Amount = r.Posting.Amount,
							        	                	Comment = r.Journal.Comment,
															Date = r.Posting.Date.IsUtc(),
							        	                	Id = r.Journal.Id,
							        	                	Quantity = r.Journal.Quantity,
							        	                	Rate = r.Journal.Rate,
							        	                	SecondAccountId = null // will be fetched with separate request to server if needed
							        	                }
							        	: new OutgoingTransferDTO
							        	  {
							        	  	Amount = r.Posting.Amount,
							        	  	Comment = r.Journal.Comment,
											Date = r.Posting.Date.IsUtc(),
							        	  	Id = r.Journal.Id,
							        	  	Quantity = r.Journal.Quantity,
							        	  	Rate = r.Journal.Rate,
							        	  	SecondAccountId = null // will be fetched with separate request to server if needed
							        	  };
							records.Add(t);

							break;

						default:
							throw new NotSupportedException(string.Format("Unknown journal type - {0}.", r.JournalType));
					}
				}
			}

			return records;
		}

	    /// <summary>
	    /// Gets all available asset types (i.e. "currency names") for user.
	    /// </summary>
	    /// <returns>Asset types presented by default or defined by the user.</returns>
	    public IList<AssetTypeDTO> GetAllAssetTypes()
		{
			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var assetTypeMapper = ObjectMapperManager.DefaultInstance.GetMapper<AssetType, AssetTypeDTO>();

				return mc.AssetTypes
					.OrderBy(c => c.Name)
					.ToList()
					.Select(assetTypeMapper.Map)
					.ToList();
			}
		}

		#endregion

		#region Transactions

	    /// <summary>
	    /// Create new deposit transaction for specific account.
	    /// The total amount of funds will be calculated as <paramref name="rate"/> * <paramref name="quantity"/>
	    /// and will be added to the <paramref name="accountId"/>.
	    /// </summary>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="date">Operation date.</param>
	    /// <param name="rate">Rate of the item.</param>
	    /// <param name="quantity">Quantity of the item.</param>
	    /// <param name="categoryId">The category ID.</param>
	    /// <param name="comment">Comment notes.</param>
	    /// <returns>Created deposit transaction ID.</returns>
	    public int Deposit(int accountId, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment)
		{
			return CreateTransaction(accountId, true, date, rate, quantity, categoryId, comment);
		}

	    /// <summary>
	    /// Create new withdrawal transaction for specific account.
	    /// The total amount of funds will be calculated as <paramref name="rate"/> * <paramref name="quantity"/>
	    /// and will be subtracted from the <paramref name="accountId"/>.
	    /// </summary>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="date">Operation date.</param>
	    /// <param name="rate">Rate of the item.</param>
	    /// <param name="quantity">Quantity of the item.</param>
	    /// <param name="categoryId">The category ID.</param>
	    /// <param name="comment">Comment notes.</param>
	    /// <returns>Created withdrawal transaction ID.</returns>
	    public int Withdrawal(int accountId, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment)
		{
			return CreateTransaction(accountId, false, date, rate, quantity, categoryId, comment);
		}

	    /// <summary>
	    /// Update specific deposit or withdrawal transaction.
	    /// </summary>
	    /// <remarks>
	    /// Transfer transaction are not updatable with this method.
	    /// To update transfer transaction use <see cref="UpdateTransfer"/> method instead.
	    /// </remarks>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="transactionId">Transaction ID.</param>
	    /// <param name="isDeposit">
	    /// <c>true</c> means that transaction is "Deposit";
	    /// <c>false</c> means that transaction is "Withdrawal".
	    /// </param>
	    /// <param name="date">Operation date.</param>
	    /// <param name="rate">Rate of the item.</param>
	    /// <param name="quantity">Quantity of the item.</param>
	    /// <param name="categoryId">The category Id.</param>
	    /// <param name="comment">Comment notes.</param>
	    /// <exception cref="NotSupportedException">
	    /// Only <see cref="JournalType.Deposit"/> and <see cref="JournalType.Withdrawal"/> journal types are supported.
	    /// </exception>
	    public void UpdateTransaction(int accountId, int transactionId, bool isDeposit, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment)
		{
			// Check comment max length
			if (comment != null && comment.Length > 256)
			{
				throw new Exception("Comment is too long. Maximum length is 256.");
			}

			// Check if rate is positive
			if (rate <= 0)
			{
				throw new Exception("Rate must not be less then or equal to 0.");
			}

			// Check if quantity  is positive
			if (quantity <= 0)
			{
				throw new Exception("Quantity must not be less then or equal to 0.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				ModelHelper.UpdateTransaction(mc, accountId, transactionId, isDeposit, date, rate, quantity, categoryId, comment);
			}
		}

		/// <summary>
		/// Create new deposit or withdrawal transaction for specific account.
		/// The total amount of funds will be calculated as <paramref name="rate"/> * <paramref name="quantity"/>
		/// and will be positive if the <paramref name="isDeposit"/> is <c>true</c>;
		/// otherwise the amount will be negative.
		/// </summary>
		/// <param name="accountId">Account ID.</param>
		/// <param name="isDeposit">
		/// 	<c>true</c> means that transaction is "Deposit";
		/// 	<c>false</c> means that transaction is "Withdrawal".
		/// </param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="categoryId">The category ID.</param>
		/// <param name="comment">Comment notes.</param>
		/// <returns>Created transaction ID.</returns>
		private int CreateTransaction(int accountId, bool isDeposit, DateTime date, decimal rate,
		                                     decimal quantity, int? categoryId, string comment)
		{
			// Check comment max length
			if (comment != null && comment.Length > 256)
			{
				throw new Exception("Comment is too long. Maximum length is 256.");
			}

			// Check rate positiveness
			if (rate <= 0)
			{
				throw new Exception("Rate must not be less then or equal to 0.");
			}

			// Check quantity positiveness
			if (quantity <= 0)
			{
				throw new Exception("Quantity must not be less then or equal to 0.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var journal = ModelHelper.CreateTransaction(mc, accountId, isDeposit ? JournalType.Deposit : JournalType.Withdrawal,
				                                            date, rate, quantity, categoryId, comment);
				mc.SaveChanges();
				return journal.Id;
			}
		}

		#endregion

		#region Transfers

	    /// <summary>
	    /// Transfer from <paramref name="fromAccountId"/> to <paramref name="toAccountId"/>
	    /// the <paramref name="rate"/> * <paramref name="quantity"/> amount of funds.
	    /// </summary>
	    /// <param name="fromAccountId">The account from which the funds will be written off.</param>
	    /// <param name="toAccountId">The account for which funds will be been credited.</param>
	    /// <param name="date">Operation date.</param>
	    /// <param name="rate">Rate of the item.</param>
	    /// <param name="quantity">Quantity of the item.</param>
	    /// <param name="comment">Comment notes.</param>
	    /// <returns>Created transfer ID.</returns>
	    /// <remarks> <paramref name="toAccountId"/> could be from another user.</remarks>
	    public int Transfer(int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment)
		{
			// Check accounts IDs difference
			if (fromAccountId == toAccountId)
			{
				throw new Exception("Account's IDs must be different.");
			}

			// Check comment max length
			if (comment != null && comment.Length > 256)
			{
				throw new Exception("Comment is too long. Maximum length is 256.");
			}

			// Check rate positiveness
			if (rate <= 0)
			{
				throw new Exception("Rate must not be less then or equal to 0.");
			}

			// Check quantity positiveness
			if (quantity <= 0)
			{
				throw new Exception("Quantity must not be less then or equal to 0.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				var journal = ModelHelper.CreateTransfer(mc, date, fromAccountId, toAccountId, rate, quantity, comment);
				mc.SaveChanges();
				return journal.Id;
			}
		}

	    /// <summary>
	    /// Update specific transfer details.
	    /// </summary>
	    /// <remarks>
	    /// Deposit or withdrawal transactions are not updatable with this method.
	    /// To update deposit or withdrawal transaction use <see cref="UpdateTransaction"/> method instead.
	    /// </remarks>
	    /// <param name="transactionId">Transfer transaction ID.</param>
	    /// <param name="fromAccountId">The account from which the funds will be written off.</param>
	    /// <param name="toAccountId">The account for which funds will be been credited.</param>
	    /// <param name="date">Operation date.</param>
	    /// <param name="rate">Rate of the item.</param>
	    /// <param name="quantity">Quantity of the item.</param>
	    /// <param name="comment">Comment notes.</param>
	    /// <exception cref="NotSupportedException">
	    /// Only <see cref="JournalType.Transfer"/> journal type supported.
	    /// </exception>
	    public void UpdateTransfer(int transactionId, int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment)
		{
			// Check accounts IDs difference
			if (fromAccountId == toAccountId)
			{
				throw new Exception("Account's IDs must be different.");
			}

			// Check comment max length
			if (comment != null && comment.Length > 256)
			{
				throw new Exception("Comment is too long. Maximum length is 256.");
			}

			// Check if rate is positive
			if (rate <= 0)
			{
				throw new Exception("Rate must not be less then or equal to 0.");
			}

			// Check if quantity is positive
			if (quantity <= 0)
			{
				throw new Exception("Quantity must not be less then or equal to 0.");
			}

			using (var mc = new ModelContainer(GetPersonalConnection(UserName)))
			{
				ModelHelper.UpdateTransfer(mc, transactionId, fromAccountId, toAccountId, date, rate, quantity, comment);
			}
		}

		#endregion

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets connection string to the user personal database based on user ID.
		/// </summary>
		/// <param name="login">User login.</param>
		/// <returns>Connection string to the user personal database.</returns>
		public string GetPersonalConnection(string login)
		{
			if (string.IsNullOrEmpty(login))
			{
				throw new ArgumentException("login");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);
			User user;

			using (var mc = new MasterEntities(masterConnection))
			{
				user = mc.Users.Single(u => u.Login == login);
			}

			return dbManager.GetPersonalConnection(user.DatabasePath);
		}

		#endregion
	}
}