//------------------------------------------------------------
// <copyright file="UserServiceTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="UserService"/>.
	/// </summary>
	public class UserServiceTests : IDisposable
	{
		#region Dependencies

		/// <summary>
		/// Test folder with databases for unit tests - "db".
		/// </summary>
		private const string DefaultFolder = "db";

		/// <summary>
		/// User service dependency.
		/// </summary>
		private readonly UserService service;

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="UserServiceTests"/> class.
		/// </summary>
		public UserServiceTests()
		{
			Dispose();

			service = new UserService
			{
				DefaultFolder = DefaultFolder
			};
		}

		#endregion

		#region User Service

		/// <summary>
		/// Test <see cref="UserService.Register"/> method.
		/// </summary>
		[Fact]
		public void RegisterNewUser()
		{
			var userId = service.Register("testUser" + Guid.NewGuid(), "testPassword");
			Assert.NotNull(userId);
		}

		/// <summary>
		/// Test <see cref="UserService.IsLoginAvailable"/> method.
		/// </summary>
		[Fact]
		public void CheckIsLoginAvailable()
		{
			bool isAvailable = service.IsLoginAvailable("testUser" + Guid.NewGuid());
			Assert.True(isAvailable);
		}

		/// <summary>
		/// Test <see cref="UserService.IsLoginAvailable"/> method.
		/// </summary>
		[Fact]
		public void CheckIsLoginNotAvailable()
		{
			string login = "testUser" + Guid.NewGuid();
			service.Register(login, "testPassword");

			bool isAvailable = service.IsLoginAvailable(login);

			Assert.False(isAvailable);
		}

		/// <summary>
		/// Test <see cref="UserService.GenerateUniqueLogin"/> method.
		/// </summary>
		[Fact]
		public void GenerateUniqueUserLogin()
		{
			string uniqueLogin = service.GenerateUniqueLogin();

			bool isAvailable = service.IsLoginAvailable(uniqueLogin);
			Assert.True(isAvailable);
		}

		/// <summary>
		/// Test <see cref="UserService.Update"/> method.
		/// </summary>
		[Fact]
		public void UpdateUser()
		{
			var userId = service.Register("testUser" + Guid.NewGuid(), "testPassword");
			service.Update(userId.Id, "testPassword", "newTestPassword", "new@email");
		}

		/// <summary>
		/// Test <see cref="UserService.GetUser"/> method.
		/// </summary>
		[Fact]
		public void GetUserId()
		{
			string login = "testUser" + Guid.NewGuid();
			const string password = "testPassword";
			var userId = service.Register(login, password);

			var userDTO = service.GetUser(login, password);

			Assert.NotNull(userDTO);
			Assert.Equal(userId.Id, userDTO.Id);
		}

		/// <summary>
		/// Test <see cref="UserService.ResetPassword"/> method.
		/// </summary>
		[Fact(Skip = "not implemented")]
		public void ResetPassword()
		{
			string login = "testUser" + Guid.NewGuid();
			var userId = service.Register(login, "testPassword");
			service.Update(userId.Id, "testPassword", "newTestPassword", "new@email");

			service.ResetPassword(login, "new@email");
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