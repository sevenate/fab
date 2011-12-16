//------------------------------------------------------------
// <copyright file="MoneyServiceTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Enums;
using Fab.Server.Core.Filters;
using Fab.Server.Core.Services;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="MoneyService"/>.
	/// </summary>
	public class MoneyServiceTests : IDisposable
	{
		#region Constants

		/// <summary>
		/// Test folder with databases for unit tests - "db".
		/// </summary>
		private const string DefaultFolder = "db";

		#endregion

		#region Dependencies

		/// <summary>
		/// User registration service dependency.
		/// </summary>
		private readonly RegistrationService registrationService;

		/// <summary>
		/// Money moneyService dependency.
		/// </summary>
		private readonly MoneyService moneyService;

		/// <summary>
		/// Current user.
		/// </summary>
		private UserDTO currentUser;

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="MoneyServiceTests"/> class.
		/// </summary>
		public MoneyServiceTests()
		{
			Dispose();

			moneyService = new MoneyService
			              {
			              	DefaultFolder = DefaultFolder
			              };

			string login = "testUser" + Guid.NewGuid();
			moneyService.UserName = login;

			registrationService = new RegistrationService
			{
				DefaultFolder = DefaultFolder
			};

			currentUser = registrationService.Register(login, "testPassword");
		}

		#endregion

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

			var accountId = moneyService.CreateAccount(currentUser.Id, accountName, assetType);
			DateTime date = DateTime.UtcNow;
			var account = moneyService.GetAccount(currentUser.Id, accountId);

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

			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", assetType);
			DateTime date = DateTime.UtcNow;

			moneyService.UpdateAccount(currentUser.Id, accountId, accountName);
			var account = moneyService.GetAccount(currentUser.Id, accountId);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var accounts = moneyService.GetAllAccounts(currentUser.Id);
			Assert.Equal(1, accounts.Count);

			moneyService.DeleteAccount(currentUser.Id, accountId);

			accounts = moneyService.GetAllAccounts(currentUser.Id);
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

			moneyService.CreateAccount(currentUser.Id, accountName1, assetType1);
			DateTime date1 = DateTime.UtcNow;
			moneyService.CreateAccount(currentUser.Id, accountName2, assetType2);
			DateTime date2 = DateTime.UtcNow;

			var accounts = moneyService.GetAllAccounts(currentUser.Id);

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

			var categoryId = moneyService.CreateCategory(currentUser.Id, categoryName, categoryType);

			var category = moneyService.GetCategory(currentUser.Id, categoryId);

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

			var categoryId = moneyService.CreateCategory(currentUser.Id, "Test Category", CategoryType.Withdrawal);

			moneyService.UpdateCategory(currentUser.Id, categoryId, categoryName, categoryType);
			var category = moneyService.GetCategory(currentUser.Id, categoryId);
			
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
			var categoryId = moneyService.CreateCategory(currentUser.Id, "Test Category", CategoryType.Withdrawal);
			var categories = moneyService.GetAllCategories(currentUser.Id);
			Assert.Equal(1, categories.Count);

			moneyService.DeleteCategory(currentUser.Id, categoryId);

			categories = moneyService.GetAllCategories(currentUser.Id);
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

			moneyService.CreateCategory(currentUser.Id, categoryName1, categoryType1);
			moneyService.CreateCategory(currentUser.Id, categoryName2, categoryType2);

			var categories = moneyService.GetAllCategories(currentUser.Id);

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
			var assets = moneyService.GetAllAssetTypes(currentUser.Id);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(0, account.Balance);

			var categoryId = moneyService.CreateCategory(currentUser.Id, "Test Category", CategoryType.Deposit);
			var category = moneyService.GetCategory(currentUser.Id, categoryId);
			Assert.Equal(0, category.Popularity);

			var journalId = moneyService.Deposit(currentUser.Id, accountId, DateTime.UtcNow, 25, 2, categoryId, "Some income");

			account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(50, account.Balance);

			category = moneyService.GetCategory(currentUser.Id, categoryId);
			Assert.Equal(1, category.Popularity);

			moneyService.DeleteJournal(currentUser.Id, accountId, journalId);

			account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(0, account.Balance);

			category = moneyService.GetCategory(currentUser.Id, categoryId);
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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryId = moneyService.CreateCategory(currentUser.Id, "Test Category", CategoryType.Withdrawal);

			var journalId = moneyService.Withdrawal(currentUser.Id, accountId, DateTime.UtcNow, 13, 10, categoryId, "Some expense");

			var account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(-130, account.Balance);

			var category = moneyService.GetCategory(currentUser.Id, categoryId);
			Assert.Equal(1, category.Popularity);

			moneyService.DeleteJournal(currentUser.Id, accountId, journalId);

			account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(0, account.Balance);

			category = moneyService.GetCategory(currentUser.Id, categoryId);
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
			var accountId1 = moneyService.CreateAccount(currentUser.Id, "Test Account 1", 1);
			var accountId2 = moneyService.CreateAccount(currentUser.Id, "Test Account 2", 1);

			var transferId = moneyService.Transfer(currentUser.Id, accountId1, accountId2, DateTime.UtcNow, 78, 10, "Some transfer comment");

			var account = moneyService.GetAccount(currentUser.Id, accountId1);
			Assert.Equal(-780, account.Balance);

			account = moneyService.GetAccount(currentUser.Id, accountId2);
			Assert.Equal(780, account.Balance);

			moneyService.DeleteJournal(currentUser.Id, accountId1, transferId);

			account = moneyService.GetAccount(currentUser.Id, accountId1);
			Assert.Equal(0, account.Balance);

			account = moneyService.GetAccount(currentUser.Id, accountId2);
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
			
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryId = moneyService.CreateCategory(currentUser.Id, "Test Category", CategoryType.Deposit);
			var depositId = moneyService.Deposit(currentUser.Id, accountId, date, rate, quantity, categoryId, comment);

			var deposit = moneyService.GetJournal(currentUser.Id, accountId, depositId) as DepositDTO;

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
			
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryId = moneyService.CreateCategory(currentUser.Id, "Test Category", CategoryType.Withdrawal);
			var withdrawalId = moneyService.Withdrawal(currentUser.Id, accountId, date, rate, quantity, categoryId, comment);

			var withdrawal = moneyService.GetJournal(currentUser.Id, accountId, withdrawalId) as WithdrawalDTO;

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
			
			var accountId1 = moneyService.CreateAccount(currentUser.Id, "Test Account 1", 1);
			var accountId2 = moneyService.CreateAccount(currentUser.Id, "Test Account 2", 1);

			var transferId = moneyService.Transfer(currentUser.Id, accountId1, accountId2, date, rate, quantity, comment);

			var outgoingTransferDTO = moneyService.GetJournal(currentUser.Id, accountId1, transferId) as OutgoingTransferDTO;

			Assert.NotNull(outgoingTransferDTO);
			Assert.Equal(-amount, outgoingTransferDTO.Amount);
			Assert.Equal(comment, outgoingTransferDTO.Comment);
			Assert.True(Math.Abs(date.Ticks - outgoingTransferDTO.Date.Ticks) < 100000);
			Assert.Equal(quantity, outgoingTransferDTO.Quantity);
			Assert.Equal(rate, outgoingTransferDTO.Rate);
			Assert.Equal(accountId2, outgoingTransferDTO.SecondAccountId);

			var incomingTransferDTO = moneyService.GetJournal(currentUser.Id, accountId2, transferId) as IncomingTransferDTO;

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
			var accountId1 = moneyService.CreateAccount(currentUser.Id, "Test Account 1", 1);
			var accountId2 = moneyService.CreateAccount(currentUser.Id, "Test Account 2", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Test Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Test Category", CategoryType.Withdrawal);

			moneyService.Deposit(currentUser.Id, accountId1, DateTime.UtcNow, 25, 10, null, "Some income");

			var balance = moneyService.GetAccountBalance(currentUser.Id, accountId1, DateTime.UtcNow);
			Assert.Equal(250, balance);

			var categories = moneyService.GetAllCategories(currentUser.Id);
			Assert.Equal(2, categories.Count);
			Assert.Equal(0, categories.Single(c => c.CategoryType == CategoryType.Deposit).Popularity);
			Assert.Equal(0, categories.Single(c => c.CategoryType == CategoryType.Withdrawal).Popularity);

			moneyService.Deposit(currentUser.Id, accountId1, DateTime.UtcNow, 5, 2, categoryDepositId, "Small income");

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId1, DateTime.UtcNow);
			Assert.Equal(260, balance);

			categories = moneyService.GetAllCategories(currentUser.Id);
			Assert.Equal(2, categories.Count);
			Assert.Equal(1, categories.Single(c => c.CategoryType == CategoryType.Deposit).Popularity);
			Assert.Equal(0, categories.Single(c => c.CategoryType == CategoryType.Withdrawal).Popularity);

			moneyService.Withdrawal(currentUser.Id, accountId1, DateTime.UtcNow, 10, 5, categoryWithdrawalId, "Some expense");

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId1, DateTime.UtcNow);
			Assert.Equal(210, balance);

			categories = moneyService.GetAllCategories(currentUser.Id);
			Assert.Equal(2, categories.Count);
			Assert.Equal(1, categories.Single(c => c.CategoryType == CategoryType.Deposit).Popularity);
			Assert.Equal(1, categories.Single(c => c.CategoryType == CategoryType.Withdrawal).Popularity);

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId2, DateTime.UtcNow);
			Assert.Equal(0, balance);

			moneyService.Transfer(currentUser.Id, accountId1, accountId2, DateTime.UtcNow, 30, 2, "Some transfer");

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId1, DateTime.UtcNow);
			Assert.Equal(150, balance);

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId2, DateTime.UtcNow);
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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 7, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter();
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Old income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Not older then expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 6, 1, null, "New expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 7, 2, categoryWithdrawalId, "Newest expense");

			var filter = new QueryFilter
			             	{
			             		NotOlderThen = date2
			             	};
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Old income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Up to expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 6, 1, null, "New expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 7, 2, categoryWithdrawalId, "Newest expense");

			var filter = new QueryFilter
			             	{
			             		Upto = date3
			             	};
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 1, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new TextSearchFilter
			             	{
								Contains = "exp"
			             	};
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 1, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
						 {
						 	Skip = 2
						 };
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 1, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
			{
				Take = 2
			};
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 7, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
			{
				Skip = 2,
				Take = 1
			};
			var journals = moneyService.GetJournals(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 7, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var journalsCount = moneyService.GetJournalsCount(currentUser.Id, accountId, new QueryFilter());

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();
			var date4 = new DateTime(2010, 5, 17, 13, 49, 30).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Some income");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Some expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date3, 7, 5, null, "Another expense");
			moneyService.Withdrawal(currentUser.Id, accountId, date4, 3, 2, categoryWithdrawalId, "Yet another ...");

			var filter = new QueryFilter
			{
				NotOlderThen = date1,
				Upto = date3
			};
			var journalsCount = moneyService.GetJournalsCount(currentUser.Id, accountId, filter);

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
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Income comment");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, null, "Expense comment");

			var balance = moneyService.GetAccountBalance(currentUser.Id, accountId, DateTime.UtcNow);
			Assert.Equal(200, balance);
			
			var account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(200, account.Balance);

			var depositCategory = moneyService.GetCategory(currentUser.Id, categoryDepositId);
			var withdrawalCategory = moneyService.GetCategory(currentUser.Id, categoryWithdrawalId);

			Assert.Equal(1, depositCategory.Popularity);
			Assert.Equal(0, withdrawalCategory.Popularity);

			var journals = moneyService.GetJournals(currentUser.Id, accountId, new QueryFilter());

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

			moneyService.UpdateTransaction(currentUser.Id, accountId, journalId, false, date3, 32, 20, categoryWithdrawalId, "This is dept and not income!");

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId, DateTime.UtcNow);
			Assert.Equal(-690, balance);

			account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(-690, account.Balance);

			depositCategory = moneyService.GetCategory(currentUser.Id, categoryDepositId);
			withdrawalCategory = moneyService.GetCategory(currentUser.Id, categoryWithdrawalId);

			Assert.Equal(0, depositCategory.Popularity);
			Assert.Equal(1, withdrawalCategory.Popularity);

			journals = moneyService.GetJournals(currentUser.Id, accountId, new QueryFilter());

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
		/// Test <see cref="MoneyService.UpdateTransaction"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Update_Debit_Transaction()
		// ReSharper restore InconsistentNaming
		{
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Income comment");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Expense comment");
			moneyService.Deposit(currentUser.Id, accountId, date3, 35, 1, categoryDepositId, "Income 2 comment");

			var balance = moneyService.GetAccountBalance(currentUser.Id, accountId, DateTime.UtcNow);
			Assert.Equal(235, balance);

			var account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(235, account.Balance);

			var depositCategory = moneyService.GetCategory(currentUser.Id, categoryDepositId);
			var withdrawalCategory = moneyService.GetCategory(currentUser.Id, categoryWithdrawalId);

			Assert.Equal(2, depositCategory.Popularity);
			Assert.Equal(1, withdrawalCategory.Popularity);

			var journals = moneyService.GetJournals(currentUser.Id, accountId, new QueryFilter());

			Assert.NotNull(journals);
			Assert.Equal(3, journals.Count);
			Assert.Equal("Income comment", journals[0].Comment);
			Assert.Equal(25, journals[0].Rate);
			Assert.Equal(10, journals[0].Quantity);
			Assert.Equal(250, journals[0].Amount);
			Assert.Equal(date1, journals[0].Date);
			Assert.True(journals[0] is DepositDTO);
			Assert.Equal(categoryDepositId, ((DepositDTO)journals[0]).CategoryId);

			var journalId = journals[0].Id;

			moneyService.UpdateTransaction(currentUser.Id, accountId, journalId, true, date3, 10, 17, categoryDepositId, "This is updated income");

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId, DateTime.UtcNow);
			Assert.Equal(155, balance);

			account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(155, account.Balance);

			depositCategory = moneyService.GetCategory(currentUser.Id, categoryDepositId);
			withdrawalCategory = moneyService.GetCategory(currentUser.Id, categoryWithdrawalId);

			Assert.Equal(2, depositCategory.Popularity);
			Assert.Equal(1, withdrawalCategory.Popularity);

			journals = moneyService.GetJournals(currentUser.Id, accountId, new QueryFilter());

			Assert.NotNull(journals);
			Assert.Equal(3, journals.Count);
			Assert.Equal("This is updated income", journals[1].Comment);
			Assert.Equal(10, journals[1].Rate);
			Assert.Equal(17, journals[1].Quantity);
			Assert.Equal(170, journals[1].Amount);
			Assert.Equal(date3, journals[1].Date);
			Assert.True(journals[1] is DepositDTO);
			Assert.Equal(categoryDepositId, ((DepositDTO)journals[1]).CategoryId);
		}

		/// <summary>
		/// Test <see cref="MoneyService.UpdateTransaction"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Update_Withdrawal_Transaction()
		// ReSharper restore InconsistentNaming
		{
			var accountId = moneyService.CreateAccount(currentUser.Id, "Test Account", 1);
			var categoryDepositId = moneyService.CreateCategory(currentUser.Id, "Deposit Category", CategoryType.Deposit);
			var categoryWithdrawalId = moneyService.CreateCategory(currentUser.Id, "Withdrawal Category", CategoryType.Withdrawal);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();
			var date3 = new DateTime(2010, 5, 17, 13, 49, 29).ToUniversalTime();

			moneyService.Deposit(currentUser.Id, accountId, date1, 25, 10, categoryDepositId, "Income comment");
			moneyService.Withdrawal(currentUser.Id, accountId, date2, 10, 5, categoryWithdrawalId, "Expense comment");
			moneyService.Deposit(currentUser.Id, accountId, date3, 35, 1, categoryDepositId, "Income 2 comment");

			var balance = moneyService.GetAccountBalance(currentUser.Id, accountId, DateTime.UtcNow);
			Assert.Equal(235, balance);

			var account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(235, account.Balance);

			var depositCategory = moneyService.GetCategory(currentUser.Id, categoryDepositId);
			var withdrawalCategory = moneyService.GetCategory(currentUser.Id, categoryWithdrawalId);

			Assert.Equal(2, depositCategory.Popularity);
			Assert.Equal(1, withdrawalCategory.Popularity);

			var journals = moneyService.GetJournals(currentUser.Id, accountId, new QueryFilter());

			Assert.NotNull(journals);
			Assert.Equal(3, journals.Count);
			Assert.Equal("Expense comment", journals[1].Comment);
			Assert.Equal(10, journals[1].Rate);
			Assert.Equal(5, journals[1].Quantity);
			Assert.Equal(-50, journals[1].Amount);
			Assert.Equal(date2, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
			Assert.Equal(categoryWithdrawalId, ((WithdrawalDTO)journals[1]).CategoryId);

			var journalId = journals[1].Id;

			moneyService.UpdateTransaction(currentUser.Id, accountId, journalId, false, date3, 10, 17, categoryWithdrawalId, "This is updated expense");

			balance = moneyService.GetAccountBalance(currentUser.Id, accountId, DateTime.UtcNow);
			Assert.Equal(115, balance);

			account = moneyService.GetAccount(currentUser.Id, accountId);
			Assert.Equal(115, account.Balance);

			depositCategory = moneyService.GetCategory(currentUser.Id, categoryDepositId);
			withdrawalCategory = moneyService.GetCategory(currentUser.Id, categoryWithdrawalId);

			Assert.Equal(2, depositCategory.Popularity);
			Assert.Equal(1, withdrawalCategory.Popularity);

			journals = moneyService.GetJournals(currentUser.Id, accountId, new QueryFilter());

			Assert.NotNull(journals);
			Assert.Equal(3, journals.Count);
			Assert.Equal("This is updated expense", journals[1].Comment);
			Assert.Equal(10, journals[1].Rate);
			Assert.Equal(17, journals[1].Quantity);
			Assert.Equal(-170, journals[1].Amount);
			Assert.Equal(date3, journals[1].Date);
			Assert.True(journals[1] is WithdrawalDTO);
			Assert.Equal(categoryWithdrawalId, ((WithdrawalDTO)journals[1]).CategoryId);
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

			var accountId1 = moneyService.CreateAccount(currentUser.Id, "Test Account 1-1", 1);
			var accountId2 = moneyService.CreateAccount(currentUser.Id, "Test Account 2", 1);
			var accountId3 = moneyService.CreateAccount(currentUser.Id, "Test Account 1-2", 1);

			var date1 = new DateTime(2010, 5, 17, 13, 49, 27).ToUniversalTime();
			var date2 = new DateTime(2010, 5, 17, 13, 49, 28).ToUniversalTime();

			var transferId = moneyService.Transfer(currentUser.Id, accountId1, accountId2, date1, rate, quantity, comment);

			var account = moneyService.GetAccount(currentUser.Id, accountId1);
			Assert.Equal(-amount, account.Balance);

			account = moneyService.GetAccount(currentUser.Id, accountId2);
			Assert.Equal(amount, account.Balance);

			account = moneyService.GetAccount(currentUser.Id, accountId3);
			Assert.Equal(0, account.Balance);

			Assert.Equal(-amount, moneyService.GetAccountBalance(currentUser.Id, accountId1, DateTime.UtcNow));
			Assert.Equal(amount, moneyService.GetAccountBalance(currentUser.Id, accountId2, DateTime.UtcNow));
			Assert.Equal(0, moneyService.GetAccountBalance(currentUser.Id, accountId3, DateTime.UtcNow));

			var trasfer = moneyService.GetJournal(currentUser.Id, accountId1, transferId);

			Assert.NotNull(trasfer);
			Assert.Equal(comment, trasfer.Comment);
			Assert.Equal(rate, trasfer.Rate);
			Assert.Equal(quantity, trasfer.Quantity);
			Assert.Equal(-amount, trasfer.Amount);
			Assert.Equal(date1, trasfer.Date);
			Assert.True(trasfer is OutgoingTransferDTO);
			Assert.Equal(accountId2, ((OutgoingTransferDTO) trasfer).SecondAccountId);

			moneyService.UpdateTransfer(currentUser.Id, transferId, accountId1, accountId3, date2, rate2, quantity2, comment2);

			account = moneyService.GetAccount(currentUser.Id, accountId1);
			Assert.Equal(-amount2, account.Balance);

			account = moneyService.GetAccount(currentUser.Id, accountId2);
			Assert.Equal(0, account.Balance);

			account = moneyService.GetAccount(currentUser.Id, accountId3);
			Assert.Equal(amount2, account.Balance);

			Assert.Equal(-amount2, moneyService.GetAccountBalance(currentUser.Id, accountId1, DateTime.UtcNow));
			Assert.Equal(0, moneyService.GetAccountBalance(currentUser.Id, accountId2, DateTime.UtcNow));
			Assert.Equal(amount2, moneyService.GetAccountBalance(currentUser.Id, accountId3, DateTime.UtcNow));

			trasfer = moneyService.GetJournal(currentUser.Id, accountId1, transferId);

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


		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			if (Directory.Exists(DefaultFolder))
			{
				Directory.Delete(DefaultFolder, true);
			}
		}

		#endregion
	}
}