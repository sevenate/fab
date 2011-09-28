// <copyright file="AdminServiceTests.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-02-04" />
// <summary>Unit tests for AdminService.</summary>

using System;
using System.Linq;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="AdminService" />.
	/// </summary>
	public class AdminServiceTests
	{
		#region Admin Service

		/// <summary>
		/// Test <see cref="AdminService.GetAllUsers"/> method.
		/// </summary>
		[Fact]
		public void GetAllUsers()
		{
			var adminService = new AdminService();

			var users = adminService.GetAllUsers();

			Assert.True(users != null && users.Count > 0);
			Assert.True(users[0].IsDisabled);
		}

		/// <summary>
		/// Test <see cref="AdminService.DisableUser"/> method.
		/// </summary>
		[Fact]
		public void DisableUser()
		{
			var service = new UserService();
			Guid userId = service.Register("testUser" + Guid.NewGuid(), "testPassword");
			var adminService = new AdminService();

			adminService.DisableUser(userId);

			var users = adminService.GetAllUsers();

			Assert.True(users != null && users.Count > 1);

			var user = users.Where(u => u.Id == userId).Single();
			Assert.True(user.IsDisabled);
		}

		#endregion
	}
}