// <copyright file="ModelHelper.cs" company="HD">
//  Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@hd.com" date="2010-02-15" />

using System;
using System.Linq;
using Fab.Server.Core.Enums;

namespace Fab.Server.Core
{
	/// <summary>
	/// Helper for Entity Framework model container processing.
	/// </summary>
	internal static class ModelHelper
	{
		#region Users

		/// <summary>
		/// Check is user <paramref name="login"/> name is not used by some one else.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="login">User login name.</param>
		/// <returns><c>true</c> if user login name is unique.</returns>
		internal static bool IsLoginAvailable(MasterEntities mc, string login)
		{
			return mc.Users.SingleOrDefault(u => u.Login == login) == null;
		}

		/// <summary>
		/// Get <see cref="User"/> from model container by unique ID.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>User instance.</returns>
		internal static User GetUserById(MasterEntities mc, Guid userId)
		{
			User user = mc.Users.SingleOrDefault(u => u.Id == userId && !u.IsDisabled);

			if (user == null)
			{
				throw new Exception("User with ID " + userId + " not found.");
			}

			return user;
		}

		/// <summary>
		/// Get <see cref="User"/> from model container by unique ID.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="userName">Unique user name.</param>
		/// <returns>User instance.</returns>
		internal static User GetUserByLogin(MasterEntities mc, string userName)
		{
			User user = mc.Users.SingleOrDefault(u => u.Login == userName && !u.IsDisabled);

			if (user == null)
			{
				throw new Exception("User with name \"" + userName + "\" not found.");
			}

			return user;
		}

		#endregion

		#region Accounts

		/// <summary>
		/// Get <see cref="Account"/> from model container by unique user ID and account ID.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="accountId">Account ID.</param>
		/// <returns>Account instance</returns>
		internal static Account GetAccountById(ModelContainer mc, int accountId)
		{
			Account account = mc.Accounts.Include("AssetType")
								.Where(a => a.Id == accountId && !a.IsClosed)
								.SingleOrDefault();

			if (account == null)
			{
				throw new Exception("Account with ID = " + accountId + " not found.");
			}

			return account;
		}

		/// <summary>
		/// Get system "cash" account with specific <paramref name="assetTypeId"/> from model container.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="assetTypeId">The asset type ID.</param>
		/// <returns>System "cash" account instance.</returns>
		private static Account GetSystemAccount(ModelContainer mc, int assetTypeId)
		{
			Account account = mc.Accounts.Include("AssetType")
								.Where(a => a.AssetType.Id == assetTypeId && a.IsSystem)
								.SingleOrDefault();

			if (account == null)
			{
				throw new Exception("System account with asset type ID = " + assetTypeId + " not found.");
			}

			return account;
		}

		/// <summary>
		/// Check account's cached "first posting date" and "last posting date"
		/// to the date of new posting if is is out of current range.
		/// </summary>
		/// <param name="account">Account to check.</param>
		/// <param name="date">New posting date.</param>
		private static void UpdateAccountPeriodAfterNewPosting(Account account, DateTime date)
		{
			if (!account.FirstPostingDate.HasValue || account.FirstPostingDate > date)
			{
				account.FirstPostingDate = date;
			}

			if (!account.LastPostingDate.HasValue || account.LastPostingDate < date)
			{
				account.LastPostingDate = date;
			}
		}

		/// <summary>
		/// Update account's cached "first posting date" and "last posting date" after deleted posting.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="account">Account to check.</param>
		/// <param name="date">Deleted posting date.</param>
		private static void UpdateAccountPeriodAfterDeletePosting(ModelContainer mc, Account account, DateTime date)
		{
			if (account.FirstPostingDate.HasValue && account.FirstPostingDate == date)
			{
				// Find 2nd (by date) post from the start
				var firstDate = (from posting in mc.Postings
								 where posting.Account.Id == account.Id
								 orderby posting.Date
				                 select posting.Date)
								 .Skip(1)
								 .Take(1)
								 .ToArray();

				account.FirstPostingDate = firstDate.Length > 0
														? firstDate[0]
														: (DateTime?) null;
			}

			if (account.LastPostingDate.HasValue && account.LastPostingDate == date)
			{
				// Find 2nd (by date) post from the end
				var lastDate = (from posting in mc.Postings
								 where posting.Account.Id == account.Id
								 orderby posting.Date descending 
								 select posting.Date)
								.Skip(1)
								.Take(1)
								.ToArray();

				account.LastPostingDate = lastDate.Length > 0
														? lastDate[0]
														: (DateTime?)null;
			}
		}

		#endregion

		#region Categories

		/// <summary>
		/// Get <see cref="Category"/> from model container by unique user ID and category ID.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="categoryId">Category ID.</param>
		/// <returns>Category instance</returns>
		internal static Category GetCategoryById(ModelContainer mc, int categoryId)
		{
			Category category = mc.Categories.Where(c => c.Id == categoryId && c.Deleted == null)
											 .SingleOrDefault();

			if (category == null)
			{
				throw new Exception("Category with ID = " + categoryId + " not found.");
			}

			return category;
		}

		#endregion

		#region Asset Types

		/// <summary>
		/// Get <see cref="AssetType"/> from model container by unique ID.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="assetTypeId">The asset type ID.</param>
		/// <returns>Asset type instance</returns>
		internal static AssetType GetAssetTypeById(ModelContainer mc, int assetTypeId)
		{
			AssetType assetType = mc.AssetTypes.Where(at => at.Id == assetTypeId)
											   .SingleOrDefault();

			if (assetType == null)
			{
				throw new Exception("Asset type with ID = " + assetTypeId + " not found.");
			}

			return assetType;
		}

		#endregion

		#region Journals

		/// <summary>
		/// Get transaction from model container by unique ID.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="journalId">The journal ID.</param>
		/// <returns>Journal instance.</returns>
		internal static Journal GetJournalById(ModelContainer mc, int journalId)
		{
			// Todo: consider using not all includes here to increase performance and decrease traffic

			var journal = mc.Journals.Include("Postings")
									 .Include("Category")
									 .Where(j => j.Id == journalId)
									 .SingleOrDefault();

			if (journal == null)
			{
				throw new Exception("Journal with ID = " + journalId + " not found.");
			}

			// Todo: Consider do NOT include posting account information into result transaction
			// to increase performance and decrease traffic

			foreach (var posting in journal.Postings)
			{
				if (!posting.AccountReference.IsLoaded)
				{
					posting.AccountReference.Load();
				}
			}

			return journal;
		}

		// Todo: add user ID account ID to the GetJournalById() method call to
		// join them with transaction ID to prevent unauthorized delete 
		// Do this for all user-aware calls (i.e. Categories, Accounts etc.)

		/// <summary>
		/// Delete specific transaction.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="journalId">The journal ID.</param>
		internal static void DeleteJournal(ModelContainer mc, int journalId)
		{
			Journal journal = GetJournalById(mc, journalId);

			// decrement popularity of the journal if any
			if (journal.Category != null)
			{
				journal.Category.Popularity--;
			}

			// Create "deleted" journal
			var deletedJournal = new DeletedJournal
			                     	{
										Id = journal.Id,
										JournalType = journal.JournalType,
										Comment = journal.Comment,
										Rate = journal.Rate,
										Quantity = journal.Quantity,
										Category = journal.Category,
			                     	};

			// Delete operation date should be the same for all "deleted" postings
			var deletedDate = DateTime.UtcNow;

			// Load and copy original journal postings to the deleted journal
			foreach (var posting in journal.Postings.ToList())
			{
				if (!posting.AccountReference.IsLoaded)
				{
					posting.AccountReference.Load();
				}

				if (!posting.AssetTypeReference.IsLoaded)
				{
					posting.AssetTypeReference.Load();
				}

				// Copy postings from the original "postings" to the "deleted postings" table
				// and associate each of them with "deleted journal"
				mc.DeletedPostings.AddObject(new DeletedPosting
				                      	{
											Id = posting.Id,
											Date = posting.Date,
				                      		Amount = posting.Amount,
											Deleted = deletedDate,
											Account = posting.Account,
				                      		AssetType = posting.AssetType,
											DeletedJournal = deletedJournal
				                      	});

				// Update cached account balance
				posting.Account.Balance -= posting.Amount;

				// Update cached postings count
				posting.Account.PostingsCount--;

				UpdateAccountPeriodAfterDeletePosting(mc, posting.Account, posting.Date);

				// Delete original postings
				mc.Postings.DeleteObject(posting);
			}

			// Add "deleted" journal
			mc.DeletedJournals.AddObject(deletedJournal);

			// Delete original journal 
			mc.Journals.DeleteObject(journal);
		}

		#endregion

		#region Transactions

		/// <summary>
		/// Create deposit or withdrawal transaction with the
		/// <paramref name="rate"/> * <paramref name="quantity"/> amount of funds
		/// added to or written off from the <paramref name="accountId"/>.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="accountId">Account ID</param>
		/// <param name="journalType"><see cref="JournalType.Deposit"/> or <see cref="JournalType.Withdrawal"/> only.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="categoryId">Category ID.</param>
		/// <param name="comment">Comment notes.</param>
		/// <returns>Created journal instance.</returns>
		internal static Journal CreateTransaction(ModelContainer mc, int accountId, JournalType journalType, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment)
		{
			var targetAccount = GetAccountById(mc, accountId);
			var cashAccount = GetSystemAccount(mc, targetAccount.AssetType.Id);

			var journal = new Journal
			                  	{
			                  		JournalType = (byte)journalType,
			                  		Rate = rate,
			                  		Quantity = quantity,
			                  		Comment = comment
			                  	};

			if (categoryId.HasValue)
			{
				var category = GetCategoryById(mc, categoryId.Value);
				category.Popularity++;
				journal.Category = category;
			}

			var creditAccount = journalType == JournalType.Deposit
			                    	? targetAccount
			                    	: cashAccount;

			var debitAccount = journalType == JournalType.Deposit
			                   	? cashAccount
			                   	: targetAccount;

			var amount = rate * quantity;

			var creditPosting = new Posting
			                    	{
			                    		Account = creditAccount,
			                    		AssetType = creditAccount.AssetType,
			                    		Date = date,
										Amount = amount,
			                    		Journal = journal
			                    	};

			var debitPosting = new Posting
			                   	{
			                   		Account = debitAccount,
			                   		AssetType = debitAccount.AssetType,
			                   		Date = date,
									Amount = -amount,
			                   		Journal = journal
			                   	};

			mc.Journals.AddObject(journal);
			mc.Postings.AddObject(creditPosting);
			mc.Postings.AddObject(debitPosting);

			// Update cached account balance
			creditAccount.Balance += amount;
			debitAccount.Balance -= amount;

			// Update cached postings count
			creditAccount.PostingsCount++;
			debitAccount.PostingsCount++;

			UpdateAccountPeriodAfterNewPosting(creditAccount, date);
			UpdateAccountPeriodAfterNewPosting(debitAccount, date);

			return journal;
		}

		/// <summary>
		/// Update deposit or withdrawal transaction with the
		/// <paramref name="rate"/> * <paramref name="quantity"/> amount of funds.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="accountId">Account ID</param>
		/// <param name="transactionId">Original deposit or withdrawal transaction ID.</param>
		/// <param name="isDeposit">Specify is the updating transaction was "deposit" (<c>true</c> value) or "withdrawal" (<c>false</c> value).</param>
		/// <param name="date">New operation date.</param>
		/// <param name="rate">New rate of the item.</param>
		/// <param name="quantity">New quantity of the item.</param>
		/// <param name="categoryId">New category ID.</param>
		/// <param name="comment">New comment notes.</param>
		internal static void UpdateTransaction(ModelContainer mc, int accountId, int transactionId, bool isDeposit, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment)
		{
			// Todo: add user ID account ID to the GetJournalById() method call to
			// join them with transaction ID to prevent unauthorized delete 
			// Do this for all user-aware calls (i.e. Categories, Accounts etc.)
			var journal = GetJournalById(mc, transactionId);

			if (journal.JournalType != (byte) JournalType.Deposit
			    && journal.JournalType != (byte) JournalType.Withdrawal)
			{
				throw new NotSupportedException(String.Format("Only {0} and {1} journal types supported.", JournalType.Deposit, JournalType.Withdrawal));
			}

			var targetAccount = GetAccountById(mc, accountId);
			var cashAccount = GetSystemAccount(mc, targetAccount.AssetType.Id);

			var targetAccountPosting = journal.Postings.Where(p => p.Account.Id == targetAccount.Id).Single();
			var cashAccountPosting = journal.Postings.Where(p => p.Account.Id == cashAccount.Id).Single();

			// Decrement cached account balance by the previous transaction amount
			if (journal.JournalType == (byte) JournalType.Deposit)
			{
				targetAccount.Balance -= targetAccountPosting.Amount;
				cashAccount.Balance += cashAccountPosting.Amount;
			}
			else
			{
				// JournalType.Withdrawal
				targetAccount.Balance += targetAccountPosting.Amount;
				cashAccount.Balance -= cashAccountPosting.Amount;
			}

			// Note: cached postings count will be the same after update transaction operation

			var amount = rate * quantity;

			if (isDeposit)
			{
				journal.JournalType = (byte)JournalType.Deposit;
				targetAccountPosting.Amount = amount;
				targetAccount.Balance += amount;
				cashAccountPosting.Amount = -amount;
				cashAccount.Balance -= amount;
			}
			else
			{
				journal.JournalType = (byte)JournalType.Withdrawal;
				targetAccountPosting.Amount = -amount;
				targetAccount.Balance -= amount;
				cashAccountPosting.Amount = amount;
				cashAccount.Balance += amount;
			}

			// Update account cached "first" and "last" posting dates
			UpdateAccountPeriodAfterDeletePosting(mc, targetAccount, targetAccountPosting.Date);
			UpdateAccountPeriodAfterDeletePosting(mc, cashAccount, cashAccountPosting.Date);
			UpdateAccountPeriodAfterNewPosting(targetAccount, date);
			UpdateAccountPeriodAfterNewPosting(cashAccount, date);

			targetAccountPosting.Date = date;
			cashAccountPosting.Date = date;

			journal.Rate = rate;
			journal.Quantity = quantity;
			journal.Comment = comment;

			if (journal.Category != null && (!categoryId.HasValue || journal.Category.Id != categoryId))
			{
				journal.Category.Popularity--;
			}

			if (categoryId.HasValue && (journal.Category == null || journal.Category.Id != categoryId))
			{
				var category = GetCategoryById(mc, categoryId.Value);
				category.Popularity++;
				journal.Category = category;
			}

			mc.SaveChanges();
		}

		#endregion

		#region Transfers

		/// <summary>
		/// Create transfer transaction: the <paramref name="rate"/> * <paramref name="quantity"/> of funds
		/// are moved from <paramref name="fromAccountId"/> account to <paramref name="toAccountId"/> account.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="fromAccountId">The account from which the funds will be written off.</param>
		/// <param name="toAccountId">The account for which funds will be been credited.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="comment">Comment notes.</param>
		/// <returns>Created journal instance.</returns>
		internal static Journal CreateTransfer(ModelContainer mc, DateTime date, int fromAccountId, int toAccountId, decimal rate, decimal quantity, string comment)
		{
			var sourceAccount = GetAccountById(mc, fromAccountId);
			var targetAccount = GetAccountById(mc, toAccountId);

			var journal = new Journal
			                  	{
			                  		JournalType = (byte)JournalType.Transfer,
			                  		Rate = rate,
									Quantity = quantity,
			                  		Comment = comment,
									Category = null	// Not used for transfers
			                  	};

			var amount = rate * quantity;

			var creditPosting = new Posting
			                    	{
			                    		Date = date,
										Amount = amount,
			                    		Journal = journal,
			                    		Account = targetAccount,
			                    		AssetType = targetAccount.AssetType
			                    	};

			var debitPosting = new Posting
			                   	{
			                   		Date = date,
									Amount = -rate * quantity,
			                   		Journal = journal,
			                   		Account = sourceAccount,
			                   		AssetType = sourceAccount.AssetType
			                   	};

			mc.Journals.AddObject(journal);
			mc.Postings.AddObject(creditPosting);
			mc.Postings.AddObject(debitPosting);

			// Update cached account balance
			targetAccount.Balance += amount;
			sourceAccount.Balance -= amount;

			// Update cached postings count
			targetAccount.PostingsCount++;
			sourceAccount.PostingsCount++;
			
			UpdateAccountPeriodAfterNewPosting(sourceAccount, date);
			UpdateAccountPeriodAfterNewPosting(targetAccount, date);

			return journal;
		}

		/// <summary>
		/// Update transfer transaction: the <paramref name="rate"/> * <paramref name="quantity"/> of funds
		/// are moved from <paramref name="fromAccountId"/> account to <paramref name="toAccountId"/> account.
		/// </summary>
		/// <param name="mc">Entity Framework model container.</param>
		/// <param name="transactionId">Original transfer transaction ID.</param>
		/// <param name="fromAccountId">The account from which the funds will be written off.</param>
		/// <param name="toAccountId">The account for which funds will be been credited.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="comment">Comment notes.</param>
		internal static void UpdateTransfer(ModelContainer mc, int transactionId, int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment)
		{
			// Todo: add user ID account ID to the GetJournalById() method call to
			// join them with transaction ID to prevent unauthorized delete 
			// Do this for all user-aware calls (i.e. Categories, Accounts etc.)
			var journal = GetJournalById(mc, transactionId);

			if (journal.JournalType != (byte)JournalType.Transfer)
			{
				throw new NotSupportedException(String.Format("Only {0} journal type supported.", JournalType.Transfer));
			}

			var sourceAccountPosting = journal.Postings.Where(p => p.Amount < 0).Single();
			var targetAccountPosting = journal.Postings.Where(p => p.Amount >= 0).Single();

			if (sourceAccountPosting.AccountReference.IsLoaded)
			{
				sourceAccountPosting.AccountReference.Load();
			}

			if (targetAccountPosting.AccountReference.IsLoaded)
			{
				targetAccountPosting.AccountReference.Load();
			}

			// Decrement cached account balance by the previous transfer amount
			sourceAccountPosting.Account.Balance -= sourceAccountPosting.Amount;
			targetAccountPosting.Account.Balance -= targetAccountPosting.Amount;

			// Update cached postings count for accounts involved into previous transfer operation
			sourceAccountPosting.Account.PostingsCount--;
			targetAccountPosting.Account.PostingsCount--;

			var sourceAccount = GetAccountById(mc, fromAccountId);
			var targetAccount = GetAccountById(mc, toAccountId);

			var amount = rate*quantity;

			sourceAccountPosting.Amount = -amount;
			sourceAccount.Balance -= amount;
			targetAccountPosting.Amount = amount;
			targetAccount.Balance += amount;

			// Update cached postings count for new accounts involved into updated transfer operation
			sourceAccount.PostingsCount++;
			targetAccount.PostingsCount++;

			sourceAccountPosting.Account = sourceAccount;
			targetAccountPosting.Account = targetAccount;

			journal.Rate = rate;
			journal.Quantity = quantity;
			journal.Comment = comment;

			// Update account cached "first" and "last" posting dates
			UpdateAccountPeriodAfterDeletePosting(mc, targetAccount, targetAccountPosting.Date);
			UpdateAccountPeriodAfterDeletePosting(mc, sourceAccount, sourceAccountPosting.Date);
			UpdateAccountPeriodAfterNewPosting(targetAccount, date);
			UpdateAccountPeriodAfterNewPosting(sourceAccount, date);

			sourceAccountPosting.Date = date;
			targetAccountPosting.Date = date;

			mc.SaveChanges();
		}

		#endregion
	}
}