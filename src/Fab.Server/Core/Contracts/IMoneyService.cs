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
	    /// <param name="name">Account name.</param>
	    /// <param name="assetTypeId">The asset type ID.</param>
	    /// <returns>Created account ID.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int CreateAccount(string name, int assetTypeId);

	    /// <summary>
	    /// Retrieve specific accounts by ID.
	    /// </summary>
	    /// <param name="accountId">Account ID to retrieve.</param>
	    /// <returns>Account data transfer object.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		AccountDTO GetAccount(int accountId);

	    /// <summary>
	    /// Update account details to new values.
	    /// </summary>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="name">Account new name.</param>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void UpdateAccount(int accountId, string name);

	    /// <summary>
	    /// Mark account as "deleted".
	    /// </summary>
	    /// <param name="accountId">Account ID to mark as deleted.</param>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void DeleteAccount(int accountId);

		/// <summary>
		/// Retrieve all accounts for user.
		/// </summary>
		/// <returns>All accounts.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<AccountDTO> GetAllAccounts();

	    /// <summary>
	    /// Gets account balance before specific date.
	    /// </summary>
	    /// <param name="accountId">Account ID.</param>
	    /// <param name="dateTime">Specific date.</param>
	    /// <returns>Account balance at the specific date.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		decimal GetAccountBalance(int accountId, DateTime dateTime);

		#endregion

		#region Categories

	    /// <summary>
	    /// Create new category.
	    /// </summary>
	    /// <param name="name">Category name.</param>
	    /// <param name="categoryType">Category type.</param>
	    /// <returns>Created category ID.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int CreateCategory(string name, CategoryType categoryType);

	    /// <summary>
	    /// Retrieve specific category by ID.
	    /// </summary>
	    /// <param name="categoryId">Category ID to retrieve.</param>
	    /// <returns>Category data transfer object.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		CategoryDTO GetCategory(int categoryId);

	    /// <summary>
	    /// Update category details to new values.
	    /// </summary>
	    /// <param name="categoryId">Category ID.</param>
	    /// <param name="name">Category new name.</param>
	    /// <param name="categoryType">Category new type.</param>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void UpdateCategory(int categoryId, string name, CategoryType categoryType);

	    /// <summary>
	    /// Mark category as "deleted".
	    /// </summary>
	    /// <param name="categoryId">Category ID to mark as deleted.</param>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void DeleteCategory(int categoryId);

	    /// <summary>
	    /// Retrieve all categories for user.
	    /// </summary>
	    /// <returns>All categories.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<CategoryDTO> GetAllCategories();

		#endregion

		#region Journals

	    /// <summary>
	    /// Delete specific journal record.
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="journalId">Journal ID.</param>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void DeleteJournal(int accountId, int journalId);

	    /// <summary>
	    /// Return single journal record (either <see cref="TransactionDTO"/> or <see cref="TransferDTO"/>).
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="journalId">Journal ID.</param>
	    /// <returns>Journal record details.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		JournalDTO GetJournal(int accountId, int journalId);

	    /// <summary>
	    /// Return filtered journal records count.
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="queryFilter">Specify conditions for filtering journal records.</param>
	    /// <returns>Filtered journal records count.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int GetJournalsCount(int accountId, IQueryFilter queryFilter);

	    /// <summary>
	    /// Return filtered list of journal records for specific account.
	    /// </summary>
	    /// <param name="accountId">The account ID.</param>
	    /// <param name="queryFilter">Specify conditions for filtering journal records.</param>
	    /// <returns>List of journal records.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<JournalDTO> GetJournals(int accountId, IQueryFilter queryFilter);

	    /// <summary>
	    /// Gets all available asset types (i.e. "currency names") for user.
	    /// </summary>
	    /// <returns>Asset types presented by default or defined by the user.</returns>
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<AssetTypeDTO> GetAllAssetTypes();

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
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int Deposit(int accountId, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment);

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
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int Withdrawal(int accountId, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment);

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
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void UpdateTransaction(int accountId, int transactionId, bool isDeposit, DateTime date, decimal rate, decimal quantity, int? categoryId, string comment);

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
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int Transfer(int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment);

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
	    [OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void UpdateTransfer(int transactionId, int fromAccountId, int toAccountId, DateTime date, decimal rate, decimal quantity, string comment);

		#endregion
	}
}