// <copyright file="MoneyServiceTests.cs" company="HD">
//  Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-02-04" />
// <summary>Unit tests for MoneyService.</summary>

using System;
using System.Linq;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Enums;
using Fab.Server.Core.Filters;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="MoneyService"/>.
	/// </summary>
	public class MoneyServiceTests
	{
		#region Accounts

		/// <summary>
		/// Test <see cref="MoneyService.CreateAccount"/> and <see cref="MoneyService.GetAccount"/> methods.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Create_and_Get_Account()
		// ReSharper restore InconsistentNaming
		{
			const string accountName = "Test Account";
			const int assetType = 1;

			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");

			var accountId = service.CreateAccount(userId, accountName, assetType);
			DateTime date = DateTime.UtcNow;
			var account = service.GetAccount(userId, accountId);

			Assert.Equal(assetType, account.AssetTypeId);
			Assert.Equal(accountName, account.Name);
			Assert.Equal(0, account.Balance);
			Assert.Equal(DateTimeKind.Utc, account.Created.Kind);
			// account creation date should not be too old
			Assert.True(account.Created.AddSeconds(30) > date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateAccount"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Update_Account()
// ReSharper restore InconsistentNaming
		{
			const string accountName = "Renamed Account";
			const int assetType = 2;

			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			DateTime date = DateTime.UtcNow;

			service.UpdateAccount(userId, accountId, accountName, assetType);
			var account = service.GetAccount(userId, accountId);

			Assert.Equal(accountName, account.Name);
			Assert.Equal(assetType, account.AssetTypeId);
			Assert.Equal(0, account.Balance);
			Assert.Equal(DateTimeKind.Utc, account.Created.Kind);
			// account creation date should not be too old
			Assert.True(account.Created.AddSeconds(30) > date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.DeleteAccount"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Delete_Account()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var accounts = service.GetAllAccounts(userId);
			Assert.Equal(1, accounts.Count);

			service.DeleteAccount(userId, accountId);

			accounts = service.GetAllAccounts(userId);
			Assert.Equal(0, accounts.Count);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAllAccounts"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_All_Accounts()
// ReSharper restore InconsistentNaming
		{
			const string accountName1 = "Test Account #1";
			const string accountName2 = "Test Account #2";
			const int assetType1 = 1;
			const int assetType2 = 2;

			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, accountName1, assetType1);
			DateTime date1 = DateTime.UtcNow;
			service.CreateAccount(userId, accountName2, assetType2);
			DateTime date2 = DateTime.UtcNow;

			var accounts = service.GetAllAccounts(userId);

			Assert.Equal(2, accounts.Count);
			// accounts should be ordered by creation date
			Assert.True(accounts[1].Created > accounts[0].Created);
			
			Assert.Equal(accountName1, accounts[0].Name);
			Assert.Equal(assetType1, accounts[0].AssetTypeId);
			Assert.Equal(0, accounts[0].Balance);
			Assert.Equal(DateTimeKind.Utc, accounts[0].Created.Kind);
			// account creation date should not be too old
			Assert.True(accounts[0].Created.AddSeconds(30) > date1);

			Assert.Equal(accountName2, accounts[1].Name);
			Assert.Equal(assetType2, accounts[1].AssetTypeId);
			Assert.Equal(0, accounts[1].Balance);
			Assert.Equal(DateTimeKind.Utc, accounts[1].Created.Kind);
			// account creation date should not be too old
			Assert.True(accounts[1].Created.AddSeconds(30) > date2);
		}

		#endregion

		#region Categories

		/// <summary>
		/// Test <see cref="MoneyService.CreateCategory"/> and <see cref="MoneyService.GetCategory"/> methods.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Create_and_Get_Category()
// ReSharper restore InconsistentNaming
		{
			const string categoryName = "Test Category";
			const CategoryType categoryType = CategoryType.Withdrawal;
			const int categoryPopularity = 0;

			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");

			var categoryId = service.CreateCategory(userId, categoryName, categoryType);

			var category = service.GetCategory(userId, categoryId);

			Assert.Equal(categoryName, category.Name);
			Assert.Equal(categoryType, category.CategoryType);
			Assert.Equal(categoryPopularity, category.Popularity);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateCategory"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Update_Category()
// ReSharper restore InconsistentNaming
		{
			const string categoryName = "Renamed Category";
			const CategoryType categoryType = CategoryType.Deposit;
			const int popularity = 0;

			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var categoryId = service.CreateCategory(userId, "Test Category", CategoryType.Withdrawal);

			service.UpdateCategory(userId, categoryId, categoryName, categoryType);
			var category = service.GetCategory(userId, categoryId);
			
			Assert.Equal(categoryName, category.Name);
			Assert.Equal(categoryType, category.CategoryType);
			Assert.Equal(popularity, category.Popularity);
		}

		/// <summary>
		/// Test <see cref="MoneyService.DeleteCategory"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Delete_Category()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var categoryId = service.CreateCategory(userId, "Test Category", CategoryType.Withdrawal);
			var categories = service.GetAllCategories(userId);
			Assert.Equal(1, categories.Count);

			service.DeleteCategory(userId, categoryId);

			categories = service.GetAllCategories(userId);
			Assert.Equal(0, categories.Count);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAllCategories"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_All_Categories()
// ReSharper restore InconsistentNaming
		{
			const string categoryName1 = "Withdrawal Category";
			const string categoryName2 = "Deposit Category";
			const CategoryType categoryType1 = CategoryType.Deposit;
			const CategoryType categoryType2 = CategoryType.Withdrawal;
			const int popularity = 0;

			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateCategory(userId, categoryName1, categoryType1);
			service.CreateCategory(userId, categoryName2, categoryType2);

			var categories = service.GetAllCategories(userId);

			Assert.Equal(2, categories.Count);

			// categories should be ordered by name

			Assert.Equal(categoryName2, categories[0].Name);
			Assert.Equal(categoryType2, categories[0].CategoryType);
			Assert.Equal(popularity, categories[0].Popularity);

			Assert.Equal(categoryName1, categories[1].Name);
			Assert.Equal(categoryType1, categories[1].CategoryType);
			Assert.Equal(popularity, categories[1].Popularity);
		}

		#endregion

		#region Journals, Transactions and Transfers

		/// <summary>
		/// Test <see cref="MoneyService.GetAllAssetTypes"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_All_AssetTypes()
// ReSharper restore InconsistentNaming
		{
			var service = new MoneyService();

			var assets = service.GetAllAssetTypes();

			// by default there are only 4 asset types: UAH, USD, EUR and RUR
			Assert.Equal(4, assets.Count);

			// and they should be sorted by name
			Assert.Equal(3, assets[0].Id);
			Assert.Equal("EUR", assets[0].Name);
			Assert.Equal(4, assets[1].Id);
			Assert.Equal("RUR", assets[1].Name);
			Assert.Equal(1, assets[2].Id);
			Assert.Equal("UAH", assets[2].Name);
			Assert.Equal(2, assets[3].Id);
			Assert.Equal("USD", assets[3].Name);
		}

		/// <summary>
		/// Test <see cref="MoneyService.Deposit"/>  and <see cref="MoneyService.DeleteJournal"/> methods.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Deposit_and_Delete()
// ReSharper restore InconsistentNaming
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var account = service.GetAccount(userId, accountId);
			Assert.Equal(0, account.Balance);

			var categoryId = service.CreateCategory(userId, "Test Category", CategoryType.Deposit);
			var category = service.GetCategory(userId, categoryId);
			Assert.Equal(0, category.Popularity);

			var journalId = service.Deposit(userId, accountId, DateTime.UtcNow, 25, 2, categoryId, "Some income");

			account = service.GetAccount(userId, accountId);
			Assert.Equal(50, account.Balance);

			category = service.GetCategory(userId, categoryId);
			Assert.Equal(1, category.Popularity);

			service.DeleteJournal(userId, accountId, journalId);

			account = service.GetAccount(userId, accountId);
			Assert.Equal(0, account.Balance);

			category = service.GetCategory(userId, categoryId);
			Assert.Equal(0, category.Popularity);
		}

		/// <summary>
		/// Test <see cref="MoneyService.Withdrawal"/> and <see cref="MoneyService.DeleteJournal"/> methods.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Withdrawal_and_Delete()
// ReSharper restore InconsistentNaming
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryId = service.CreateCategory(userId, "Test Category", CategoryType.Withdrawal);
			
			var journalId = service.Withdrawal(userId, accountId, DateTime.UtcNow, 13, 10, categoryId, "Some expense");

			var account = service.GetAccount(userId, accountId);
			Assert.Equal(-130, account.Balance);

			var category = service.GetCategory(userId, categoryId);
			Assert.Equal(1, category.Popularity);

			service.DeleteJournal(userId, accountId, journalId);

			account = service.GetAccount(userId, accountId);
			Assert.Equal(0, account.Balance);

			category = service.GetCategory(userId, categoryId);
			Assert.Equal(0, category.Popularity);
		}

		/// <summary>
		/// Test <see cref="MoneyService.Transfer"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Create_and_Delete_Transfer()
// ReSharper restore InconsistentNaming
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId1 = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var userId2 = userService.Register("testUser2" + Guid.NewGuid(), "testPassword");
			var accountId1 = service.CreateAccount(userId1, "Test Account 1", 1);
			var accountId2 = service.CreateAccount(userId2, "Test Account 2", 1);

			var transferId = service.Transfer(userId1, accountId1, accountId2, DateTime.UtcNow, 78, 10, "Some transfer comment");

			var account = service.GetAccount(userId1, accountId1);
			Assert.Equal(-780, account.Balance);

			account = service.GetAccount(userId2, accountId2);
			Assert.Equal(780, account.Balance);

			service.DeleteJournal(userId1, accountId1, transferId);

			account = service.GetAccount(userId1, accountId1);
			Assert.Equal(0, account.Balance);

			account = service.GetAccount(userId2, accountId2);
			Assert.Equal(0, account.Balance);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournal"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Deposit_Journal()
// ReSharper restore InconsistentNaming
		{
			const decimal rate = 25;
			const decimal quantity = 10;
			const decimal amount = rate * quantity;
			const string comment = "Some income";
			DateTime date = DateTime.UtcNow;

			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryId = service.CreateCategory(userId, "Test Category", CategoryType.Deposit);
			var depositId = service.Deposit(userId, accountId, date, rate, quantity, categoryId, comment);

			var deposit = service.GetJournal(userId, accountId, depositId) as DepositDTO;

			Assert.NotNull(deposit);
			Assert.Equal(amount, deposit.Amount);
			Assert.Equal(categoryId, deposit.CategoryId);
			Assert.Equal(comment, deposit.Comment);
			Assert.True(Math.Abs(date.Ticks - deposit.Date.Ticks) < 100000);
			Assert.Equal(quantity, deposit.Quantity);
			Assert.Equal(rate, deposit.Rate);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournal"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Get_Withdrawal_Journal()
		// ReSharper restore InconsistentNaming
		{
			const decimal rate = 75;
			const decimal quantity = 2;
			const decimal amount = rate * quantity;
			const string comment = "Some expense";
			DateTime date = DateTime.UtcNow;

			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryId = service.CreateCategory(userId, "Test Category", CategoryType.Withdrawal);
			var withdrawalId = service.Withdrawal(userId, accountId, date, rate, quantity, categoryId, comment);

			var withdrawal = service.GetJournal(userId, accountId, withdrawalId) as WithdrawalDTO;

			Assert.NotNull(withdrawal);
			Assert.Equal(-amount, withdrawal.Amount);
			Assert.Equal(categoryId, withdrawal.CategoryId);
			Assert.Equal(comment, withdrawal.Comment);
			Assert.True(Math.Abs(date.Ticks - withdrawal.Date.Ticks) < 100000);
			Assert.Equal(quantity, withdrawal.Quantity);
			Assert.Equal(rate, withdrawal.Rate);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournal"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Get_Incoming_and_Outgoing_Transfer_Journals()
		// ReSharper restore InconsistentNaming
		{
			const decimal rate = 25;
			const decimal quantity = 10;
			const decimal amount = rate*quantity;
			const string comment = "Some transfer";
			DateTime date = DateTime.UtcNow;

			var service = new MoneyService();
			var userService = new UserService();
			var userId1 = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var userId2 = userService.Register("testUser2" + Guid.NewGuid(), "testPassword");
			var accountId1 = service.CreateAccount(userId1, "Test Account 1", 1);
			var accountId2 = service.CreateAccount(userId2, "Test Account 2", 1);

			var transferId = service.Transfer(userId1, accountId1, accountId2, date, rate, quantity, comment);

			var outgoingTransferDTO = service.GetJournal(userId1, accountId1, transferId) as OutgoingTransferDTO;

			Assert.NotNull(outgoingTransferDTO);
			Assert.Equal(-amount, outgoingTransferDTO.Amount);
			Assert.Equal(comment, outgoingTransferDTO.Comment);
			Assert.True(Math.Abs(date.Ticks - outgoingTransferDTO.Date.Ticks) < 100000);
			Assert.Equal(quantity, outgoingTransferDTO.Quantity);
			Assert.Equal(rate, outgoingTransferDTO.Rate);
			Assert.Equal(accountId2, outgoingTransferDTO.SecondAccountId);

			var incomingTransferDTO = service.GetJournal(userId1, accountId2, transferId) as IncomingTransferDTO;

			Assert.NotNull(incomingTransferDTO);
			Assert.Equal(amount, incomingTransferDTO.Amount);
			Assert.Equal(comment, incomingTransferDTO.Comment);
			Assert.True(Math.Abs(date.Ticks - incomingTransferDTO.Date.Ticks) < 100000);
			Assert.Equal(quantity, incomingTransferDTO.Quantity);
			Assert.Equal(rate, incomingTransferDTO.Rate);
			Assert.Equal(accountId1, incomingTransferDTO.SecondAccountId);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAccountBalance"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Account_Balance()
// ReSharper restore InconsistentNaming
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId1 = service.CreateAccount(userId, "Test Account 1", 1);
			var accountId2 = service.CreateAccount(userId, "Test Account 2", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Test Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Test Category", CategoryType.Withdrawal);
			
			service.Deposit(userId, accountId1, DateTime.UtcNow, 25, 10, null, "Some income");

			var balance = service.GetAccountBalance(userId, accountId1, DateTime.UtcNow);
			Assert.Equal(250, balance);
			
			var categories = service.GetAllCategories(userId);
			Assert.Equal(2, categories.Count);
			Assert.Equal(0, categories.Single(c => c.CategoryType == CategoryType.Deposit).Popularity);
			Assert.Equal(0, categories.Single(c => c.CategoryType == CategoryType.Withdrawal).Popularity);

			service.Deposit(userId, accountId1, DateTime.UtcNow, 5, 2, categoryDepositId, "Small income");

			balance = service.GetAccountBalance(userId, accountId1, DateTime.UtcNow);
			Assert.Equal(260, balance);
			
			categories = service.GetAllCategories(userId);
			Assert.Equal(2, categories.Count);
			Assert.Equal(1, categories.Single(c => c.CategoryType == CategoryType.Deposit).Popularity);
			Assert.Equal(0, categories.Single(c => c.CategoryType == CategoryType.Withdrawal).Popularity);

			service.Withdrawal(userId, accountId1, DateTime.UtcNow, 10, 5, categoryWithdrawalId, "Some expense");

			balance = service.GetAccountBalance(userId, accountId1, DateTime.UtcNow);
			Assert.Equal(210, balance);

			categories = service.GetAllCategories(userId);
			Assert.Equal(2, categories.Count);
			Assert.Equal(1, categories.Single(c => c.CategoryType == CategoryType.Deposit).Popularity);
			Assert.Equal(1, categories.Single(c => c.CategoryType == CategoryType.Withdrawal).Popularity);

			balance = service.GetAccountBalance(userId, accountId2, DateTime.UtcNow);
			Assert.Equal(0, balance);

			service.Transfer(userId, accountId1, accountId2, DateTime.UtcNow, 30, 2, "Some transfer");

			balance = service.GetAccountBalance(userId, accountId1, DateTime.UtcNow);
			Assert.Equal(150, balance);

			balance = service.GetAccountBalance(userId, accountId2, DateTime.UtcNow);
			Assert.Equal(60, balance);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_All_Journals()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 7, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter();
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(4, journals.Count);
			Assert.True(journals[0] is DepositDTO);
			Assert.True(journals[1] is WithdrawalDTO);
			Assert.True(journals[2] is WithdrawalDTO);
			Assert.True(journals[3] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Not_Older_Then()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Old income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Not older then expense");
			service.Withdrawal(userId, accountId, date3, 6, 1, null, "New expense");
			service.Withdrawal(userId, accountId, date4, 7, 2, categoryWithdrawalId, "Newest expense");

			var filter = new QueryFilter
			             	{
			             		NotOlderThen = date2
			             	};
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(3, journals.Count);
			Assert.Equal("Not older then expense", journals[0].Comment);
			Assert.Equal(10, journals[0].Rate);
			Assert.Equal(5, journals[0].Quantity);
			Assert.Equal(-50, journals[0].Amount);
			Assert.Equal(date2, journals[0].Date);
			Assert.True(journals[0] is WithdrawalDTO);
			Assert.Equal("New expense", journals[1].Comment);
			Assert.Equal(6, journals[1].Rate);
			Assert.Equal(1, journals[1].Quantity);
			Assert.Equal(-6, journals[1].Amount);
			Assert.Equal(date3, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
			Assert.Equal("Newest expense", journals[2].Comment);
			Assert.Equal(7, journals[2].Rate);
			Assert.Equal(2, journals[2].Quantity);
			Assert.Equal(-14, journals[2].Amount);
			Assert.Equal(date4, journals[2].Date);
			Assert.True(journals[2] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_UpTo()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Old income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Up to expense");
			service.Withdrawal(userId, accountId, date3, 6, 1, null, "New expense");
			service.Withdrawal(userId, accountId, date4, 7, 2, categoryWithdrawalId, "Newest expense");

			var filter = new QueryFilter
			             	{
			             		Upto = date3
			             	};
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(2, journals.Count);
			Assert.Equal("Old income", journals[0].Comment);
			Assert.Equal(25, journals[0].Rate);
			Assert.Equal(10, journals[0].Quantity);
			Assert.Equal(250, journals[0].Amount);
			Assert.Equal(date1, journals[0].Date);
			Assert.True(journals[0] is DepositDTO);
			Assert.Equal("Up to expense", journals[1].Comment);
			Assert.Equal(10, journals[1].Rate);
			Assert.Equal(5, journals[1].Quantity);
			Assert.Equal(-50, journals[1].Amount);
			Assert.Equal(date2, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Text()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 1, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new TextSearchFilter
			             	{
								Contains = "exp"
			             	};
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(2, journals.Count);
			Assert.Equal("Some expense", journals[0].Comment);
			Assert.Equal(10, journals[0].Rate);
			Assert.Equal(5, journals[0].Quantity);
			Assert.Equal(-50, journals[0].Amount);
			Assert.Equal(date2, journals[0].Date);
			Assert.True(journals[0] is WithdrawalDTO);
			Assert.Equal("Another expense", journals[1].Comment);
			Assert.Equal(1, journals[1].Rate);
			Assert.Equal(5, journals[1].Quantity);
			Assert.Equal(-5, journals[1].Amount);
			Assert.Equal(date3, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Skiping()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 1, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
						 {
						 	Skip = 2
						 };
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(2, journals.Count);
			Assert.Equal("Another expense", journals[0].Comment);
			Assert.Equal(1, journals[0].Rate);
			Assert.Equal(5, journals[0].Quantity);
			Assert.Equal(-5, journals[0].Amount);
			Assert.Equal(date3, journals[0].Date);
			Assert.True(journals[1] is WithdrawalDTO);
			Assert.Equal("Yet another ...", journals[1].Comment);
			Assert.Equal(3, journals[1].Rate);
			Assert.Equal(2, journals[1].Quantity);
			Assert.Equal(-6, journals[1].Amount);
			Assert.Equal(date4, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Taking()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 1, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
			{
				Take = 2
			};
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(2, journals.Count);
			Assert.Equal("Some income", journals[0].Comment);
			Assert.Equal(25, journals[0].Rate);
			Assert.Equal(10, journals[0].Quantity);
			Assert.Equal(250, journals[0].Amount);
			Assert.Equal(date1, journals[0].Date);
			Assert.True(journals[0] is DepositDTO);
			Assert.Equal("Some expense", journals[1].Comment);
			Assert.Equal(10, journals[1].Rate);
			Assert.Equal(5, journals[1].Quantity);
			Assert.Equal(-50, journals[1].Amount);
			Assert.Equal(date2, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournals"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Skiping_Taking()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 7, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
			{
				Skip = 2,
				Take = 1
			};
			var journals = service.GetJournals(userId, accountId, filter);

			Assert.NotNull(journals);
			Assert.Equal(1, journals.Count);
			Assert.Equal("Another expense", journals[0].Comment);
			Assert.Equal(7, journals[0].Rate);
			Assert.Equal(5, journals[0].Quantity);
			Assert.Equal(-35, journals[0].Amount);
			Assert.Equal(date3, journals[0].Date);
			Assert.True(journals[0] is WithdrawalDTO);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournalsCount"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Count_All()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 7, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var journalsCount = service.GetJournalsCount(userId, accountId, new QueryFilter());

			Assert.Equal(4, journalsCount);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetJournalsCount"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_Journals_Count_DateRange()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Some income");
			service.Withdrawal(userId, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			service.Withdrawal(userId, accountId, date3, 7, 5, null, "Another expense");
			service.Withdrawal(userId, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
			{
				NotOlderThen = date1,
				Upto = date3
			};
			var journalsCount = service.GetJournalsCount(userId, accountId, filter);

			Assert.Equal(2, journalsCount);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateTransaction"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Update_Transaction()
// ReSharper restore InconsistentNaming
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var accountId = service.CreateAccount(userId, "Test Account", 1);
			var categoryDepositId = service.CreateCategory(userId, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = service.CreateCategory(userId, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();

			service.Deposit(userId, accountId, date1, 25, 10, categoryDepositId, "Income comment");
			service.Withdrawal(userId, accountId, date2, 10, 5, null, "Expense comment");

			var balance = service.GetAccountBalance(userId, accountId, DateTime.UtcNow);
			Assert.Equal(200, balance);
			
			var account = service.GetAccount(userId, accountId);
			Assert.Equal(200, account.Balance);

			var depositCategory = service.GetCategory(userId, categoryDepositId);
			var withdrawalCategory = service.GetCategory(userId, categoryWithdrawalId);

			Assert.Equal(1, depositCategory.Popularity);
			Assert.Equal(0, withdrawalCategory.Popularity);

			var journals = service.GetJournals(userId, accountId, new QueryFilter());

			Assert.NotNull(journals);
			Assert.Equal(2, journals.Count);
			Assert.Equal("Income comment", journals[0].Comment);
			Assert.Equal(25, journals[0].Rate);
			Assert.Equal(10, journals[0].Quantity);
			Assert.Equal(250, journals[0].Amount);
			Assert.Equal(date1, journals[0].Date);
			Assert.True(journals[0] is DepositDTO);
			Assert.Equal(categoryDepositId, ((DepositDTO) journals[0]).CategoryId);

			var journalId = journals[0].Id;

			service.UpdateTransaction(userId, accountId, journalId, false, date3, 32, 20, categoryWithdrawalId, "This is dept and not income!");

			balance = service.GetAccountBalance(userId, accountId, DateTime.UtcNow);
			Assert.Equal(-690, balance);

			account = service.GetAccount(userId, accountId);
			Assert.Equal(-690, account.Balance);

			depositCategory = service.GetCategory(userId, categoryDepositId);
			withdrawalCategory = service.GetCategory(userId, categoryWithdrawalId);

			Assert.Equal(0, depositCategory.Popularity);
			Assert.Equal(1, withdrawalCategory.Popularity);

			journals = service.GetJournals(userId, accountId, new QueryFilter());

			Assert.NotNull(journals);
			Assert.Equal(2, journals.Count);
			Assert.Equal("This is dept and not income!", journals[1].Comment);
			Assert.Equal(32, journals[1].Rate);
			Assert.Equal(20, journals[1].Quantity);
			Assert.Equal(-640, journals[1].Amount);
			Assert.Equal(date3, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
			Assert.Equal(categoryWithdrawalId, ((WithdrawalDTO) journals[1]).CategoryId);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateTransfer"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Update_Transfer()
		// ReSharper restore InconsistentNaming
		{
			const decimal rate = 75;
			const decimal quantity = 10;
			const decimal amount = rate * quantity;
			const string comment = "Outgoing transfer";

			const decimal rate2 = 3;
			const decimal quantity2 = 40;
			const decimal amount2 = rate2 * quantity2;
			const string comment2 = "Another outgoing transfer";

			var userService = new UserService();
			var service = new MoneyService();
			var userId1 = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var userId2 = userService.Register("testUser2" + Guid.NewGuid(), "testPassword");
			var accountId1 = service.CreateAccount(userId1, "Test Account 1-1", 1);
			var accountId2 = service.CreateAccount(userId2, "Test Account 2", 1);
			var accountId3 = service.CreateAccount(userId1, "Test Account 1-2", 1);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();

			var transferId = service.Transfer(userId1, accountId1, accountId2, date1, rate, quantity, comment);

			var account = service.GetAccount(userId1, accountId1);
			Assert.Equal(-amount, account.Balance);

			account = service.GetAccount(userId2, accountId2);
			Assert.Equal(amount, account.Balance);

			account = service.GetAccount(userId1, accountId3);
			Assert.Equal(0, account.Balance);

			Assert.Equal(-amount, service.GetAccountBalance(userId1, accountId1, DateTime.UtcNow));
			Assert.Equal(amount, service.GetAccountBalance(userId2, accountId2, DateTime.UtcNow));
			Assert.Equal(0, service.GetAccountBalance(userId1, accountId3, DateTime.UtcNow));

			var trasfer = service.GetJournal(userId1, accountId1, transferId);

			Assert.NotNull(trasfer);
			Assert.Equal(comment, trasfer.Comment);
			Assert.Equal(rate, trasfer.Rate);
			Assert.Equal(quantity, trasfer.Quantity);
			Assert.Equal(-amount, trasfer.Amount);
			Assert.Equal(date1, trasfer.Date);
			Assert.True(trasfer is OutgoingTransferDTO);
			Assert.Equal(accountId2, ((OutgoingTransferDTO) trasfer).SecondAccountId);

			service.UpdateTransfer(userId1, transferId, accountId1, accountId3, date2, rate2, quantity2, comment2);

			account = service.GetAccount(userId1, accountId1);
			Assert.Equal(-amount2, account.Balance);

			account = service.GetAccount(userId2, accountId2);
			Assert.Equal(0, account.Balance);

			account = service.GetAccount(userId1, accountId3);
			Assert.Equal(amount2, account.Balance);

			Assert.Equal(-amount2, service.GetAccountBalance(userId1, accountId1, DateTime.UtcNow));
			Assert.Equal(0, service.GetAccountBalance(userId2, accountId2, DateTime.UtcNow));
			Assert.Equal(amount2, service.GetAccountBalance(userId1, accountId3, DateTime.UtcNow));

			trasfer = service.GetJournal(userId1, accountId1, transferId);

			Assert.NotNull(trasfer);
			Assert.Equal(comment2, trasfer.Comment);
			Assert.Equal(rate2, trasfer.Rate);
			Assert.Equal(quantity2, trasfer.Quantity);
			Assert.Equal(-amount2, trasfer.Amount);
			Assert.Equal(date2, trasfer.Date);
			Assert.True(trasfer is OutgoingTransferDTO);
			Assert.Equal(accountId3, ((OutgoingTransferDTO)trasfer).SecondAccountId);
		}

		#endregion
	}
}