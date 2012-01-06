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
		/// <returns>Latest "DisabledChanged" value.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		DateTime UpdateUser(AdminUserDTO userDto);
	}
}