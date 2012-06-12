//------------------------------------------------------------
// <copyright file="AdminServiceTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Filters;
using Fab.Server.Core.Services;
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
		/// Admin service dependency.
		/// </summary>
		private readonly AdminService adminService;

		/// <summary>
		/// User registration service dependency.
		/// </summary>
		private readonly RegistrationService registrationService;

		/// <summary>
		/// Money service dependency.
		/// </summary>
		private readonly MoneyService moneyService;

		/// <summary>
		/// Current user.
		/// </summary>
		private UserDTO currentUser;

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminServiceTests"/> class.
		/// </summary>
		public AdminServiceTests()
		{
			Dispose();
			
			adminService = new AdminService
			               {
			               	DefaultFolder = DefaultFolder
			               };
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

		#region Admin Service

		/// <summary>
		/// Test <see cref="AdminService.GetUsers"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_All_Users()
// ReSharper restore InconsistentNaming
		{
			string login = "secondTestUser" + Guid.NewGuid();
			var userDTO = registrationService.Register(login, "testPassword");

			var users = adminService.GetUsers(new QueryFilter());

			Assert.True(users != null && users.Count == 2);
			Assert.True(users[0].Id == currentUser.Id);
			Assert.True(users[0].DatabaseSize != null);
			Assert.True(users[0].FreeDiskSpaceAvailable != null);
			Assert.True(users[1].Id == userDTO.Id);
			Assert.True(users[1].DatabaseSize != null);
			Assert.True(users[1].FreeDiskSpaceAvailable != null);
		}

		/// <summary>
		/// Test <see cref="AdminService.DeleteUser"/> method.
		/// </summary>
		[Fact]
		public void DeleteUser()
		{
			var users = adminService.GetUsers(new QueryFilter());
			Assert.True(users != null && users.Count == 1);

			adminService.DeleteUser(currentUser.Id);

			users = adminService.GetUsers(new QueryFilter());
			Assert.True(users != null && users.Count == 0);
		}

		/// <summary>
		/// Test <see cref="AdminService.UpdateUser"/> method.
		/// </summary>
		[Fact]
		public void UpdateUser()
		{
			Assert.True(currentUser.Id != Guid.Empty);
			Assert.True(currentUser.Registered != new DateTime());
			Assert.True(currentUser.ServiceUrl == string.Empty);

			var adminUserDTO = new AdminUserDTO(currentUser);
			Assert.True(adminUserDTO.Id == currentUser.Id);
			Assert.True(adminUserDTO.Registered == currentUser.Registered);
			Assert.True(adminUserDTO.ServiceUrl == currentUser.ServiceUrl);
			Assert.True(adminUserDTO.ServiceUrl == string.Empty);
			Assert.True(adminUserDTO.DatabaseSize == null);
	
			adminUserDTO.Email = @"test@localhost";
			adminUserDTO.DatabasePath = @"d:\New Path here.sdf";
			adminUserDTO.IsDisabled = true;
			adminUserDTO.Login = @"new_login";
			adminUserDTO.Password = @"newPa$$w0rd!";
			adminUserDTO.ServiceUrl = @"\\some\service_here.svc";

			adminService.UpdateUser(adminUserDTO);
			var users = adminService.GetUsers(new QueryFilter());

			Assert.True(users[0].Id == adminUserDTO.Id);
			Assert.True(Math.Abs(users[0].Registered.Ticks - adminUserDTO.Registered.Ticks) < 100000);
			Assert.True(users[0].ServiceUrl == adminUserDTO.ServiceUrl);
			Assert.True(users[0].DatabasePath == adminUserDTO.DatabasePath);
			Assert.True(users[0].IsDisabled == adminUserDTO.IsDisabled);
			Assert.True(users[0].Login == adminUserDTO.Login);
			Assert.True(users[0].Password == null);
			Assert.True(users[0].ServiceUrl == adminUserDTO.ServiceUrl);
		}

		[Fact]
		public void Check_Cached_Values_For_User_Accounts()
		{
			var account1 = moneyService.CreateAccount("Account 1", 1);
			var account2 = moneyService.CreateAccount("Account 2", 2);
			var account3 = moneyService.CreateAccount("Account 3", 3);

			moneyService.Deposit(account1, new DateTime(2012, 2, 11), 10, 3, null, null);
			moneyService.Deposit(account1, new DateTime(2012, 2, 10), 5, 4, null, null);
			moneyService.Withdrawal(account1, new DateTime(2012, 2, 13), 20, 2, null, null);

			moneyService.Deposit(account2, new DateTime(2012, 2, 5), 8, 1, null, null);

			var maintenanceAccounts = adminService.UpdateCachedValuesForUserAccounts(currentUser.Id, true);

			Assert.Equal(7, maintenanceAccounts.Count);
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