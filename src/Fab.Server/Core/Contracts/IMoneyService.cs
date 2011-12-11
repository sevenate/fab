//------------------------------------------------------------
// <copyright file="IMoneyService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Enums;
using Fab.Server.Core.Filters;

namespace Fab.Server.Core.Contracts
{
	/// <summary>
	/// Money service contract.
	/// </summary>
	[ServiceContract]
	[ServiceKnownType(typeof(DepositDTO))]
	[ServiceKnownType(typeof(WithdrawalDTO))]
	[ServiceKnownType(typeof(IncomingTransferDTO))]
	[ServiceKnownType(typeof(OutgoingTransferDTO))]
	[ServiceKnownType(typeof(TransactionDTO))]
	[ServiceKnownType(typeof(TransferDTO))]
	[ServiceKnownType(typeof(QueryFilter))]
	[ServiceKnownType(typeof(TextSearchFilter))]
	[ServiceKnownType(typeof(CategoryFilter))]
	public interface IMoneyService
	{
		#region Accounts

		/// <summary>
		/// Create new account.
		/// </summary>
		/// <param name="userId">User unique ID for which this account should be created.</param>
		/// <param name="name">Account name.</param>
		/// <param name="assetTypeId">The asset type ID.</param>
		/// <returns>Created account ID.</returns>
		[OperationContract]
		int CreateAccount(Guid userId, string name, int assetTypeId);

		/// <summary>
		/// Retrieve specific accounts by ID.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="accountId">Account ID to retrieve.</param>
		/// <returns>Account data transfer object.</returns>
		[OperationContract]
		AccountDTO GetAccount(Guid userId, int accountId);

		/// <summary>
		/// Update account details to new values.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="accountId">Account ID.</param>
		/// <param name="name">Account new name.</param>
		[OperationContract]
		void UpdateAccount(Guid userId, int accountId, string name);

		/// <summary>
		/// Mark account as "deleted".
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="accountId">Account ID to mark as deleted.</param>
		[OperationContract]
		void DeleteAccount(Guid userId, int accountId);

		/// <summary>
		/// Retrieve all accounts for user.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <returns>All accounts.</returns>
		[OperationContract]
		IList<AccountDTO> GetAllAccounts(Guid userId);

		/// <summary>
		/// Gets account balance before specific date.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="accountId">Account ID.</param>
		/// <param name="dateTime">Specific date.</param>
		/// <returns>Account balance at the specific date.</returns>
		[OperationContract]
		decimal GetAccountBalance(Guid userId, int accountId, DateTime dateTime);

		#endregion

		#region Categories

		/// <summary>
		/// Create new category.
		/// </summary>
		/// <param name="userId">User unique ID for which this category should be created.</param>
		/// <param name="name">Category name.</param>
		/// <param name="categoryType">Category type.</param>
		/// <returns>Created category ID.</returns>
		[OperationContract]
		int CreateCategory(Guid userId, string name, CategoryType categoryType);

		/// <summary>
		/// Retrieve specific category by ID.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="categoryId">Category ID to retrieve.</param>
		/// <returns>Category data transfer object.</returns>
		[OperationContract]
		CategoryDTO GetCategory(Guid userId, int categoryId);

		/// <summary>
		/// Update category details to new values.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="categoryId">Category ID.</param>
		/// <param name="name">Category new name.</param>
		/// <param name="categoryType">Category new type.</param>
		[OperationContract]
		void UpdateCategory(Guid userId, int categoryId, string name, CategoryType categoryType);

		/// <summary>
		/// Mark category as "deleted".
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="categoryId">Category ID to mark as deleted.</param>
		[OperationContract]
		void DeleteCategory(Guid userId, int categoryId);

		/// <summary>
		/// Retrieve all categories for user.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <returns>All categories.</returns>
		[OperationContract]
		IList<CategoryDTO> GetAllCategories(Guid userId);

		#endregion

		#region Journals

		/// <summary>
		/// Delete specific journal record.
		/// </summary>
		/// <param name="userId">The user unique ID.</param>
		/// <param name="accountId">The account ID.</param>
		/// <param name="journalId">Journal ID.</param>
		[OperationContract]
		void DeleteJournal(Guid userId, int accountId, int journalId);

		/// <summary>
		/// Return single journal record (either <see cref="TransactionDTO"/> or <see cref="TransferDTO"/>).
		/// </summary>
		/// <param name="userId">The user unique ID.</param>
		/// <param name="accountId">The account ID.</param>
		/// <param name="journalId">Journal ID.</param>
		/// <returns>Journal record details.</returns>
		[OperationContract]
		JournalDTO GetJournal(Guid userId, int accountId, int journalId);

		/// <summary>
		/// Return filtered journal records count.
		/// </summary>
		/// <param name="userId">The user unique ID.</param>
		/// <param name="accountId">The account ID.</param>
		/// <param name="queryFilter">Specify conditions for filtering journal records.</param>
		/// <returns>Filtered journal records count.</returns>
		[OperationContract]
		int GetJournalsCount(Guid userId, int accountId, IQueryFilter queryFilter);

		/// <summary>
		/// Return filtered list of journal records for specific account.
		/// </summary>
		/// <param name="userId">The user unique ID.</param>
		/// <param name="accountId">The account ID.</param>
		/// <param name="queryFilter">Specify conditions for filtering journal records.</param>
		/// <returns>List of journal records.</returns>
		[OperationContract]
		IList<JournalDTO> GetJournals(Guid userId, int accountId, IQueryFilter queryFilter);

		/// <summary>
		/// Gets all available asset types (i.e. "currency names") for user.
		/// </summary>
		/// <param name="userId">The user unique ID.</param>
		/// <returns>Asset types presented by default or defined by the user.</returns>
		[OperationContract]
		IList<AssetTypeDTO> GetAllAssetTypes(Guid userId);

		#endregion

		#region Transactions

		/// <summary>
		/// Create new deposit transaction for specific account.
		/// The total amount of funds will be calculated as <paramref name="rate"/> * <paramref name="quantity"/>
		/// and will be added to the <paramref name="accountId"/>.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="accountId">Account ID.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="categoryId">The category ID.</param>
		/// <param name="comment">Comment notes.</param>
		/// <returns>Created deposit transaction ID.</returns>
		[OperationContract]
		int Deposit(Guid userId, int accountId, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment);
		
		/// <summary>
		/// Create new withdrawal transaction for specific account.
		/// The total amount of funds will be calculated as <paramref name="rate"/> * <paramref name="quantity"/>
		/// and will be subtracted from the <paramref name="accountId"/>.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="accountId">Account ID.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="categoryId">The category ID.</param>
		/// <param name="comment">Comment notes.</param>
		/// <returns>Created withdrawal transaction ID.</returns>
		[OperationContract]
		int Withdrawal(Guid userId, int accountId, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment);

		/// <summary>
		/// Update specific deposit or withdrawal transaction.
		/// </summary>
		/// <remarks>
		/// Transfer transaction are not updatable with this method.
		/// To update transfer transaction use <see cref="UpdateTransfer(System.Guid,int,int,int,System.DateTime,decimal,decimal,string)"/> method instead.
		/// </remarks>
		/// <param name="userId">User unique ID.</param>
		/// <param name="accountId">Account ID.</param>
		/// <param name="transactionId">Transaction ID.</param>
		/// <param name="isDeposit">
		/// 	<c>true</c> means that transaction is "Deposit";
		/// 	<c>false</c> means that transaction is "Withdrawal".
		/// </param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="categoryId">The category Id.</param>
		/// <param name="comment">Comment notes.</param>
		[OperationContract]
		void UpdateTransaction(Guid userId, int accountId, int transactionId, bool isDeposit, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment);

		#endregion

		#region Transfers

		/// <summary>
		/// Transfer from <paramref name="fromAccountId"/> to <paramref name="toAccountId"/>
		/// the <paramref name="rate"/> * <paramref name="quantity"/> amount of funds.
		/// </summary>
		/// <param name="userId">The user unique ID.</param>
		/// <param name="fromAccountId">The account from which the funds will be written off.</param>
		/// <param name="toAccountId">The account for which funds will be been credited.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="comment">Comment notes.</param>
		/// <returns>Created transfer ID.</returns>
		/// <remarks> <paramref name="toAccountId"/> could be from another user.</remarks>
		[OperationContract]
		int Transfer(Guid userId, int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment);

		/// <summary>
		/// Update specific transfer details.
		/// </summary>
		/// <remarks>
		/// Deposit or withdrawal transactions are not updatable with this method.
		/// To update deposit or withdrawal transaction use <see cref="UpdateTransaction"/> method instead.
		/// </remarks>
		/// <param name="userId">The user unique ID.</param>
		/// <param name="transactionId">Transfer transaction ID.</param>
		/// <param name="fromAccountId">The account from which the funds will be written off.</param>
		/// <param name="toAccountId">The account for which funds will be been credited.</param>
		/// <param name="date">Operation date.</param>
		/// <param name="rate">Rate of the item.</param>
		/// <param name="quantity">Quantity of the item.</param>
		/// <param name="comment">Comment notes.</param>
		[OperationContract]
		void UpdateTransfer(Guid userId, int transactionId, int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment);

		#endregion
	}
}