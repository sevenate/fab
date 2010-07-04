// <copyright file="IUserService.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-01-28" />
// <summary>User service contract.</summary>

using System;
using System.ServiceModel;

namespace Fab.Server.Core
{
	/// <summary>
	/// User service contract.
	/// </summary>
	[ServiceContract]
	public interface IUserService
	{
		/// <summary>
		/// Generate unique login name for new user.
		/// </summary>
		/// <returns>Unique login name.</returns>
		[OperationContract]
		string GenerateUniqueLogin();

		/// <summary>
		/// Check new user login name for uniqueness.
		/// </summary>
		/// <param name="login">User login.</param>
		/// <returns><c>True</c> if user login name is unique.</returns>
		[OperationContract]
		bool IsLoginAvailable(string login);

		/// <summary>
		/// Register new user with unique login name and password.
		/// </summary>
		/// <param name="login">User login name.</param>
		/// <param name="password">User password.</param>
		/// <returns>Created user ID.</returns>
		[OperationContract]
		Guid Register(string login, string password);

		/// <summary>
		/// Change user password or email to new values.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="oldPassword">User old password.</param>
		/// <param name="newPassword">User new password.</param>
		/// <param name="newEmail">User new email.</param>
		[OperationContract]
		void Update(Guid userId, string oldPassword, string newPassword, string newEmail);

		/// <summary>
		/// Get user ID by unique login name.
		/// </summary>
		/// <param name="login">User unique login name.</param>
		/// <returns>User unique ID.</returns>
		[OperationContract]
		Guid GetUserId(string login);

		/// <summary>
		/// If user with specified login name have email and this email is match to specified email,
		/// then system will reset current password for this user to auto generated new one
		/// and sent it to the specified email.
		/// </summary>
		/// <param name="login">User login name.</param>
		/// <param name="email">User email.</param>
		[OperationContract]
		void ResetPassword(string login, string email);
	}
}