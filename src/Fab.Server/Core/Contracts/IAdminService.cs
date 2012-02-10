//------------------------------------------------------------
// <copyright file="IAdminService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Filters;

namespace Fab.Server.Core.Contracts
{
	/// <summary>
	/// Administrative service contract.
	/// </summary>
	[ServiceContract]
	[ServiceKnownType(typeof(QueryFilter))]
	[ServiceKnownType(typeof(TextSearchFilter))]
	public interface IAdminService
	{
		#region Common

		/// <summary>
		/// Return count of users based on search filter.
		/// </summary>
		/// <param name="queryFilter">Filter conditions.</param>
		/// <returns>Count of filtered users.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		int GetUsersCount(IQueryFilter queryFilter);

		/// <summary>
		/// Return filtered list of registered users from the system.
		/// </summary>
		/// <param name="queryFilter">Filter conditions.</param>
		/// <returns>List of users.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<AdminUserDTO> GetUsers(IQueryFilter queryFilter);

		/// <summary>
		/// Delete specific user from master database records by his internal unique ID.
		/// Note: user personal database file will NOT be deleted since this is manual operation.
		/// </summary>
		/// <param name="userId">User ID to delete.</param>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void DeleteUser(Guid userId);

		/// <summary>
		/// Update specific user data.
		/// </summary>
		/// <param name="userDto">User to update.</param>
		/// <returns>Last updated date.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		DateTime UpdateUser(AdminUserDTO userDto);

		#endregion

		#region User

		/// <summary>
		/// Optimize user personal database file size.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>Database file after optimization.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		long OptimizeUserDatabase(Guid userId);

		/// <summary>
		/// Verify user personal database file integrity by comparing checksums.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>True if the checksums match and there is no database corruption; otherwise, false.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		bool VerifyUserDatabase(Guid userId);

		/// <summary>
		/// Try to repair a corrupted personal database file.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>Database file after repair.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		long RepairUserDatabase(Guid userId);

		#endregion

		#region Master

		/// <summary>
		/// Optimize master database file size.
		/// </summary>
		/// <returns>Database file after optimization.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		long OptimizeMasterDatabase();

		/// <summary>
		/// Verify master database file integrity by comparing checksums.
		/// </summary>
		/// <returns>True if the checksums match and there is no database corruption; otherwise, false.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		bool VerifyMasterDatabase();

		/// <summary>
		/// Try to repair a corrupted master database file.
		/// </summary>
		/// <returns>Database file after repair.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		long RepairMasterDatabase();

		#endregion

		/// <summary>
		/// Update (check/recalculate) cached values for user like accounts current balance, categories used statistic etc.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="checkOnly">If <c>false</c> - difference between cached and actual values will be fixed.</param>
		/// <returns>List of maintenance object (one per account) with info about actual and previous cached values.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<AccountMaintenanceDTO> UpdateCachedValuesForUserAccounts(Guid userId, bool checkOnly);
	}
}