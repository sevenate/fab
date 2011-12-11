//------------------------------------------------------------
// <copyright file="IUserService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ServiceModel;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Contracts
{
	/// <summary>
	/// User service contract.
	/// </summary>
	[ServiceContract]
	public interface IUserService
	{
		/// <summary>
		/// Get user info based on authenticated username.
		/// </summary>
		/// <returns>User info.</returns>
		[OperationContract]
		UserDTO GetUser();

		/// <summary>
		/// Change user password or email to new values.
		/// </summary>
		/// <param name="oldPassword">User old password.</param>
		/// <param name="newPassword">User new password.</param>
		/// <param name="newEmail">User new email.</param>
		[OperationContract]
		void Update(string oldPassword, string newPassword, string newEmail);
	}
}