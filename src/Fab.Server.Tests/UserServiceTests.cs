//------------------------------------------------------------
// <copyright file="UserServiceTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Services;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="UserService"/>.
	/// </summary>
	public class UserServiceTests : IDisposable
	{
		#region Constants

		/// <summary>
		/// Test folder with databases for unit tests - "db".
		/// </summary>
		private const string DefaultFolder = "db";

		#endregion

		#region Dependencies

		/// <summary>
		/// User service dependency.
		/// </summary>
		private readonly UserService service;

		/// <summary>
		/// User registration service dependency.
		/// </summary>
		private readonly RegistrationService registrationService;

		/// <summary>
		/// Current user.
		/// </summary>
		private UserDTO currentUser;

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

			string login = "testUser" + Guid.NewGuid();
			service.UserName = login;

			registrationService = new RegistrationService
			{
				DefaultFolder = DefaultFolder
			};

			currentUser = registrationService.Register(login, "testPassword");
		}

		#endregion

		#region User Service

		/// <summary>
		/// Test <see cref="UserService.GetUser"/> method.
		/// </summary>
		[Fact]
		public void GetUserId()
		{
			var userDTO = service.GetUser();

			Assert.NotNull(userDTO);
			Assert.Equal(currentUser.Id, userDTO.Id);
			Assert.Equal(string.Empty, userDTO.ServiceUrl);
		}

		/// <summary>
		/// Test <see cref="UserService.Update"/> method.
		/// </summary>
		[Fact]
		public void UpdateUser()
		{
			service.Update("testPassword", "newTestPassword", "new@email");
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