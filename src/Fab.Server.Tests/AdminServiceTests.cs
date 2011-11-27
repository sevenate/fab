//------------------------------------------------------------
// <copyright file="AdminServiceTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="AdminService" />.
	/// </summary>
	public class AdminServiceTests : IDisposable
	{
		#region Dependencies

		/// <summary>
		/// Test folder with databases for unit tests - "db".
		/// </summary>
		private const string DefaultFolder = "db";

		/// <summary>
		/// User service dependency.
		/// </summary>
		private readonly UserService userService;

		/// <summary>
		/// Admin service dependency.
		/// </summary>
		private readonly AdminService adminService;

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminServiceTests"/> class.
		/// </summary>
		public AdminServiceTests()
		{
			Dispose();
			userService = new UserService
			          {
			          	DefaultFolder = DefaultFolder
			          };
			adminService = new AdminService
			               {
			               	DefaultFolder = DefaultFolder
			               };
		}

		#endregion

		#region Admin Service

		/// <summary>
		/// Test <see cref="AdminService.GetAllUsers"/> method.
		/// </summary>
		[Fact]
		public void GetAllUsers()
		{
			string login = "testUser" + Guid.NewGuid();
			const string password = "testPassword";
			var userDTO = userService.Register(login, password);

			var users = adminService.GetAllUsers();

			Assert.True(users != null && users.Count == 1);
			Assert.True(users[0].Id == userDTO.Id);
		}

		/// <summary>
		/// Test <see cref="AdminService.DisableUser"/> method.
		/// </summary>
		[Fact]
		public void DisableUser()
		{
			var userDTO = userService.Register("testUser" + Guid.NewGuid(), "testPassword");

			adminService.DisableUser(userDTO.Id);

			var users = adminService.GetAllUsers();

			Assert.True(users != null && users.Count == 1);
			Assert.True(users[0].Id == userDTO.Id);
			Assert.True(users[0].IsDisabled);
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