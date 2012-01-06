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
		/// User registration service dependency.
		/// </summary>
		private readonly RegistrationService registrationService;

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
			registrationService = new RegistrationService
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
		/// Test <see cref="AdminService.GetUsers"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Get_All_Users()
// ReSharper restore InconsistentNaming
		{
			string login = "testUser" + Guid.NewGuid();
			var userDTO = registrationService.Register(login, "testPassword");

			login = "testUser2" + Guid.NewGuid();
			var userDTO2 = registrationService.Register(login, "testPassword");

			var users = adminService.GetUsers(new QueryFilter());

			Assert.True(users != null && users.Count == 2);
			Assert.True(users[0].Id == userDTO.Id);
			Assert.True(users[0].DatabaseSize != null);
			Assert.True(users[1].Id == userDTO2.Id);
			Assert.True(users[1].DatabaseSize != null);
		}

		/// <summary>
		/// Test <see cref="AdminService.DeleteUser"/> method.
		/// </summary>
		[Fact]
		public void DeleteUser()
		{
			var userDTO = registrationService.Register("testUser" + Guid.NewGuid(), "testPassword");

			var users = adminService.GetUsers(new QueryFilter());
			Assert.True(users != null && users.Count == 1);

			adminService.DeleteUser(userDTO.Id);

			users = adminService.GetUsers(new QueryFilter());
			Assert.True(users != null && users.Count == 0);
		}

		/// <summary>
		/// Test <see cref="AdminService.UpdateUser"/> method.
		/// </summary>
		[Fact]
		public void UpdateUser()
		{
			var userDTO = registrationService.Register("testUser" + Guid.NewGuid(), "testPassword");

			Assert.True(userDTO.Id != Guid.Empty);
			Assert.True(userDTO.Registered != new DateTime());
			Assert.True(userDTO.ServiceUrl == string.Empty);
			
			var adminUserDTO = new AdminUserDTO(userDTO);
			Assert.True(adminUserDTO.Id == userDTO.Id);
			Assert.True(adminUserDTO.Registered == userDTO.Registered);
			Assert.True(adminUserDTO.ServiceUrl == userDTO.ServiceUrl);
			Assert.True(adminUserDTO.ServiceUrl == string.Empty);
			Assert.True(adminUserDTO.DatabaseSize == null);
	
			adminUserDTO.Email = @"test@localhost";
			adminUserDTO.DatabasePath = @"d:\New Path here.sdf";
			adminUserDTO.IsDisabled = true;
			adminUserDTO.Login = @"new_login";
			adminUserDTO.Passoword = @"newPa$$w0rd!";
			adminUserDTO.ServiceUrl = @"\\some\service_here.svc";

			adminService.UpdateUser(adminUserDTO);
			var users = adminService.GetUsers(new QueryFilter());

			Assert.True(users[0].Id == adminUserDTO.Id);
			Assert.True(Math.Abs(users[0].Registered.Ticks - adminUserDTO.Registered.Ticks) < 100000);
			Assert.True(users[0].ServiceUrl == adminUserDTO.ServiceUrl);
			Assert.True(users[0].DatabasePath == adminUserDTO.DatabasePath);
			Assert.True(users[0].IsDisabled == adminUserDTO.IsDisabled);
			Assert.True(users[0].Login == adminUserDTO.Login);
			Assert.True(users[0].Passoword == null);
			Assert.True(users[0].ServiceUrl == adminUserDTO.ServiceUrl);
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