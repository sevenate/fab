//------------------------------------------------------------
// <copyright file="IAdminService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Contracts
{
	/// <summary>
	/// Administrative service contract.
	/// </summary>
	[ServiceContract]
	public interface IAdminService
	{
		/// <summary>
		/// Retrieve all registered users from the system.
		/// </summary>
		/// <returns>All users.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		IList<AdminUserDTO> GetAllUsers();

		/// <summary>
		/// Disable login for specific user by his internal unique ID.
		/// </summary>
		/// <param name="userId">User ID to disable.</param>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void DisableUser(Guid userId);
	}
}