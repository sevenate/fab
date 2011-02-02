// <copyright file="MoneyServiceTests.cs" company="HD">
//  Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-02-04" />
// <summary>Unit tests for MoneyService.</summary>

using System;
using Fab.Server.Core;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="MoneyService"/>.
	/// </summary>
	public class MoneyServiceTests
	{
		#region Account Service

		/// <summary>
		/// Test <see cref="MoneyService.CreateAccount"/> method.
		/// </summary>
		[Fact]
		public void CreateAccount()
		{
			const string expectedAccountName = "Test Account";
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");

			int accountId = service.CreateAccount(userId, expectedAccountName, 1);

			var accounts = service.GetAllAccounts(userId);
			Assert.Equal(1, accounts.Count);
			Assert.Equal(expectedAccountName, accounts[0].Name);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateAccount"/> method.
		/// </summary>
		[Fact]
		public void UpdateAccount()
		{
			const string accountName = "Test Account";
			const string expectedNewAccountName = "Renamed Account";
			const int assetType = 1;
			const int expectedNewAssetType = 2;
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, accountName, assetType);
			var accounts = service.GetAllAccounts(userId);

			service.UpdateAccount(userId, accounts[0].Id, expectedNewAccountName, expectedNewAssetType);

			accounts = service.GetAllAccounts(userId);
			Assert.Equal(1, accounts.Count);
			Assert.Equal(expectedNewAccountName, accounts[0].Name);
			Assert.Equal(expectedNewAssetType, accounts[0].AssetType.Id);
		}

		/// <summary>
		/// Test <see cref="MoneyService.DeleteAccount"/> method.
		/// </summary>
		[Fact]
		public void DeleteAccount()
		{
			const string accountName = "Test Account";
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, accountName, 1);
			var accounts = service.GetAllAccounts(userId);

			service.DeleteAccount(userId, accounts[0].Id);

			accounts = service.GetAllAccounts(userId);
			Assert.Equal(0, accounts.Count);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAllAccounts"/> method.
		/// </summary>
		[Fact]
		public void GetAllAccounts()
		{
			const string expectedAccountName = "Test Account";
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, expectedAccountName, 1);

			var accounts = service.GetAllAccounts(userId);

			Assert.Equal(1, accounts.Count);
			Assert.Equal(expectedAccountName, accounts[0].Name);
			Assert.NotNull(accounts[0].AssetType);
			Assert.Equal(1, accounts[0].AssetType.Id);
		}

		#endregion

		#region Category Service

		/// <summary>
		/// Test <see cref="MoneyService.CreateCategory"/> method.
		/// </summary>
		[Fact]
		public void CreateCategory()
		{
			const string expectedCategoryName = "Test Category";
			const byte categoryType = 1;
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");

			int categoryId = service.CreateCategory(userId, expectedCategoryName, categoryType);

			var categories = service.GetAllCategories(userId);
			Assert.Equal(1, categories.Count);
			Assert.Equal(expectedCategoryName, categories[0].Name);
			Assert.Equal(categoryType, categories[0].CategoryType);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateCategory"/> method.
		/// </summary>
		[Fact]
		public void UpdateCategory()
		{
			const string categoryName = "Test Category";
			const string expectedNewCategoryName = "Renamed Category";
			const byte categoryType = 1;
			const byte expectedNewCategoryType = 2;
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateCategory(userId, categoryName, categoryType);
			var categories = service.GetAllCategories(userId);

			service.UpdateCategory(userId, categories[0].Id, expectedNewCategoryName, expectedNewCategoryType);

			categories = service.GetAllCategories(userId);
			Assert.Equal(1, categories.Count);
			Assert.Equal(expectedNewCategoryName, categories[0].Name);
			Assert.Equal(expectedNewCategoryType, categories[0].CategoryType);
		}

		/// <summary>
		/// Test <see cref="MoneyService.DeleteCategory"/> method.
		/// </summary>
		[Fact]
		public void DeleteCategory()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateCategory(userId, "Test Category", 1);
			var categories = service.GetAllCategories(userId);

			service.DeleteCategory(userId, categories[0].Id);

			categories = service.GetAllCategories(userId);
			Assert.Equal(0, categories.Count);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAllCategories"/> method.
		/// </summary>
		[Fact]
		public void GetAllCategories()
		{
			const string expectedCategoryName = "Test Category";
			const byte expectedNewCategoryType = 1;
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateCategory(userId, expectedCategoryName, expectedNewCategoryType);

			var categories = service.GetAllCategories(userId);

			Assert.Equal(1, categories.Count);
			Assert.Equal(expectedCategoryName, categories[0].Name);
			Assert.Equal(expectedNewCategoryType, categories[0].CategoryType);
		}

		#endregion

		#region Transaction Service

		/// <summary>
		/// Test <see cref="MoneyService.GetAllAssetTypes"/> method.
		/// </summary>
		[Fact]
		public void GetAllAssetTypes()
		{
			var service = new MoneyService();

			var assets = service.GetAllAssetTypes();

			Assert.Equal(4, assets.Count);
		}

		/// <summary>
		/// Test <see cref="MoneyService.Deposit"/> method.
		/// </summary>
		[Fact]
		public void Deposit()
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category", 1);
			var categories = service.GetAllCategories(userId);

			service.Deposit(userId, accounts[0].Id, DateTime.Now, 25, 2, "Some income comment", categories[0].Id);
		}

		/// <summary>
		/// Test <see cref="MoneyService.Withdrawal"/> method.
		/// </summary>
		[Fact]
		public void Withdrawal()
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId = userService.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category", 1);
			var categories = service.GetAllCategories(userId);

			service.Withdrawal(userId, accounts[0].Id, DateTime.Now, 13, 10, "Some expense comment", categories[0].Id);
		}

		/// <summary>
		/// Test <see cref="MoneyService.Transfer"/> method.
		/// </summary>
		[Fact]
		public void Transfer()
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId1 = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var userId2 = userService.Register("testUser2" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId1, "Test Account 1", 1);
			service.CreateAccount(userId2, "Test Account 2", 1);
			var accounts1 = service.GetAllAccounts(userId1);
			var accounts2 = service.GetAllAccounts(userId2);

			service.Transfer(userId1, accounts1[0].Id, userId2, accounts2[0].Id, DateTime.Now, 78, "Some transfer comment");
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetTransaction"/> method.
		/// </summary>
		[Fact]
		public void GetTransaction()
		{
			const string accountName = "Test Account 1";
			const string categoryName = "Test Category 1";
			const byte categoryType = 1;
			var service = new MoneyService();
			var userService = new UserService();
			var userId1 = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId1, accountName, 1);
			var accounts1 = service.GetAllAccounts(userId1);
			service.CreateCategory(userId1, categoryName, categoryType);
			var categories = service.GetAllCategories(userId1);
			service.Deposit(userId1, accounts1[0].Id, DateTime.Now, 25, 10, "Some income comment", categories[0].Id);

			var transactions = service.GetAllTransactions(userId1, accounts1[0].Id);
			
			Assert.Equal(1, transactions.Count);

			var transaction = service.GetTransaction(userId1, accounts1[0].Id, transactions[0].TransactionId);

			Assert.NotNull(transaction);
			Assert.Equal(2, transaction.Postings.Count);
			
			Assert.NotNull(transaction.Category);
			Assert.Equal(categoryName, transaction.Category.Name);
			Assert.Equal(categoryType, transaction.Category.CategoryType);
			
			Assert.NotNull(transaction.Postings[0].Account);
			Assert.Equal(accountName, transaction.Postings[0].Account.Name);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAccountBalance"/> method.
		/// </summary>
		[Fact]
		public void GetAccountBalance()
		{
			var service = new MoneyService();
			var userService = new UserService();
			var userId1 = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			var userId2 = userService.Register("testUser2" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId1, "Test Account 1", 1);
			service.CreateAccount(userId2, "Test Account 2", 1);
			var accounts1 = service.GetAllAccounts(userId1);
			var accounts2 = service.GetAllAccounts(userId2);
			service.CreateCategory(userId1, "Test Category 1", 1);
			var categories1 = service.GetAllCategories(userId1);

			service.Deposit(userId1, accounts1[0].Id, DateTime.Now, 25, 10, "Some income comment", null);

			var balance = service.GetAccountBalance(userId1, accounts1[0].Id);
			Assert.Equal(250, balance);

			service.Withdrawal(userId1, accounts1[0].Id, DateTime.Now, 10, 5, "Some expense comment", categories1[0].Id);

			balance = service.GetAccountBalance(userId1, accounts1[0].Id);
			Assert.Equal(200, balance);

			balance = service.GetAccountBalance(userId2, accounts2[0].Id);
			Assert.Equal(0, balance);

			service.Transfer(userId1, accounts1[0].Id, userId2, accounts2[0].Id, DateTime.Now, 75, "Some transfer comment");

			balance = service.GetAccountBalance(userId1, accounts1[0].Id);
			Assert.Equal(125, balance);

			balance = service.GetAccountBalance(userId2, accounts2[0].Id);
			Assert.Equal(75, balance);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetAllTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetAllTransactions()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			service.Deposit(userId, accounts[0].Id, DateTime.Now, 25, 10, "Some income comment", null);
			service.Withdrawal(userId, accounts[0].Id, DateTime.Now, 10, 5, "Some expense comment", categories[0].Id);

			var transactionRecords = service.GetAllTransactions(userId, accounts[0].Id);

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetTransactionsNotOlderThenDate()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accounts[0].Id, date1, 25, 10, "Old income", null);
			service.Withdrawal(userId, accounts[0].Id, date2, 10, 5, "Not older then expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date3, 6, 1, "New expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date4, 3, 2, "Newest expense", categories[0].Id);

			var filter = new QueryFilter
			             	{
			             		NotOlderThen = date2
			             	};
			var transactionRecords = service.GetTransactions(userId, accounts[0].Id, filter);

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);
			Assert.Equal("New expense", transactionRecords[0].Comment);
			Assert.Equal(date3, transactionRecords[0].Date);
			Assert.Equal("Newest expense", transactionRecords[1].Comment);
			Assert.Equal(date4, transactionRecords[1].Date);
		}


		/// <summary>
		/// Test <see cref="MoneyService.GetTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetTransactionsUpToDate()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();

			service.Deposit(userId, accounts[0].Id, date1, 25, 10, "Old income", null);
			service.Withdrawal(userId, accounts[0].Id, date2, 10, 5, "Up to expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date3, 6, 1, "New expense", categories[0].Id);

			var filter = new QueryFilter
			             	{
			             		Upto = date2
			             	};
			var transactionRecords = service.GetTransactions(userId, accounts[0].Id, filter);

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);
			Assert.Equal("Old income", transactionRecords[0].Comment);
			Assert.Equal(date1, transactionRecords[0].Date);
			Assert.Equal("Up to expense", transactionRecords[1].Comment);
			Assert.Equal(date2, transactionRecords[1].Date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetTransactionsByText()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accounts[0].Id, date1, 25, 10, "Some income", null);
			service.Withdrawal(userId, accounts[0].Id, date2, 10, 5, "Some expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date3, 1, 5, "Another expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date4, 3, 2, "Yet another ...", categories[0].Id);

			var filter = new QueryFilter
			             	{
								Contains = "exp"
			             	};
			var transactionRecords = service.GetTransactions(userId, accounts[0].Id, filter);

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);
			Assert.Equal("Some expense", transactionRecords[0].Comment);
			Assert.Equal(date2, transactionRecords[0].Date);
			Assert.Equal("Another expense", transactionRecords[1].Comment);
			Assert.Equal(date3, transactionRecords[1].Date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetTransactionsWithSkiping()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accounts[0].Id, date1, 25, 10, "Some income", null);
			service.Withdrawal(userId, accounts[0].Id, date2, 10, 5, "Some expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date3, 1, 5, "Another expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date4, 3, 2, "Yet another ...", categories[0].Id);

			var filter = new QueryFilter
						 {
						 	Skip = 2
						 };
			var transactionRecords = service.GetTransactions(userId, accounts[0].Id, filter);

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);
			Assert.Equal("Another expense", transactionRecords[0].Comment);
			Assert.Equal(date3, transactionRecords[0].Date);
			Assert.Equal("Yet another ...", transactionRecords[1].Comment);
			Assert.Equal(date4, transactionRecords[1].Date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetTransactionsWithTaking()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accounts[0].Id, date1, 25, 10, "Some income", null);
			service.Withdrawal(userId, accounts[0].Id, date2, 10, 5, "Some expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date3, 1, 5, "Another expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date4, 3, 2, "Yet another ...", categories[0].Id);

			var filter = new QueryFilter
			{
				Take = 2
			};
			var transactionRecords = service.GetTransactions(userId, accounts[0].Id, filter);

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);
			Assert.Equal("Some income", transactionRecords[0].Comment);
			Assert.Equal(date1, transactionRecords[0].Date);
			Assert.Equal("Some expense", transactionRecords[1].Comment);
			Assert.Equal(date2, transactionRecords[1].Date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.GetTransactions"/> method.
		/// </summary>
		[Fact]
		public void GetTransactionsWithSkipingAndTaking()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			service.Deposit(userId, accounts[0].Id, date1, 25, 10, "Some income", null);
			service.Withdrawal(userId, accounts[0].Id, date2, 10, 5, "Some expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date3, 1, 5, "Another expense", categories[0].Id);
			service.Withdrawal(userId, accounts[0].Id, date4, 3, 2, "Yet another ...", categories[0].Id);

			var filter = new QueryFilter
			{
				Skip = 2,
				Take = 1
			};
			var transactionRecords = service.GetTransactions(userId, accounts[0].Id, filter);

			Assert.NotNull(transactionRecords);
			Assert.Equal(1, transactionRecords.Count);
			Assert.Equal("Another expense", transactionRecords[0].Comment);
			Assert.Equal(date3, transactionRecords[0].Date);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateTransaction"/> method.
		/// </summary>
		[Fact]
		public void UpdateTransaction()
		{
			var userService = new UserService();
			var service = new MoneyService();
			var userId = userService.Register("testUser1" + Guid.NewGuid(), "testPassword");
			service.CreateAccount(userId, "Test Account 1", 1);
			var accounts = service.GetAllAccounts(userId);
			service.CreateCategory(userId, "Test Category 1", 1);
			var categories = service.GetAllCategories(userId);

			service.Deposit(userId, accounts[0].Id, DateTime.Now, 25, 10, "Some income comment", null);
			service.Withdrawal(userId, accounts[0].Id, DateTime.Now, 10, 5, "Some expense comment", categories[0].Id);
			
			var balance = service.GetAccountBalance(userId, accounts[0].Id);
			Assert.Equal(200, balance);

			var transactionRecords = service.GetAllTransactions(userId, accounts[0].Id);
			var transactionId = transactionRecords[0].TransactionId;

			Assert.NotNull(transactionRecords);
			Assert.Equal(2, transactionRecords.Count);

			service.UpdateTransaction(transactionId, userId, accounts[0].Id, DateTime.Now, 32, 20, "Updated income", categories[0].Id, false);

			balance = service.GetAccountBalance(userId, accounts[0].Id);
			Assert.Equal(-690, balance);
		}

		#endregion
	}
}