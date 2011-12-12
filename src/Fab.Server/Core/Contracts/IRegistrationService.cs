//------------------------------------------------------------
// <copyright file="IRegistrationService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.ServiceModel;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Contracts
{
	/// <summary>
	/// User registration contract.
	/// </summary>
	[ServiceContract]
	public interface IRegistrationService
	{
		/// <summary>
		/// Generate unique login name for new user.
		/// </summary>
		/// <returns>Unique login name.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]

		string GenerateUniqueLogin();

		/// <summary>
		/// Check new user login name for uniqueness.
		/// </summary>
		/// <param name="login">User login.</param>
		/// <returns><c>True</c> if user login name is unique.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		bool IsLoginAvailable(string login);

		/// <summary>
		/// Register new user with unique login name and password.
		/// </summary>
		/// <param name="login">User login name.</param>
		/// <param name="password">User password.</param>
		/// <returns>Created user object.</returns>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		UserDTO Register(string login, string password);

		/// <summary>
		/// If user with specified login name have email and this email is match to specified email,
		/// then system will reset current password for this user to auto generated new one
		/// and sent it to the specified email.
		/// </summary>
		/// <param name="login">User login name.</param>
		/// <param name="email">User email.</param>
		[OperationContract]
		[FaultContract(typeof(FaultDetail))]
		void ResetPassword(string login, string email);
	}
}