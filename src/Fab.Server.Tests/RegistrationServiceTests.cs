//------------------------------------------------------------
// <copyright file="RegistrationServiceTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using Fab.Server.Core.Services;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="RegistrationService"/>.
	/// </summary>
	public class RegistrationServiceTests : IDisposable
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

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistrationServiceTests"/> class.
		/// </summary>
		public RegistrationServiceTests()
		{
			Dispose();

			registrationService = new RegistrationService
			{
				DefaultFolder = DefaultFolder
			};
		}

		#endregion

		/// <summary>
		/// Test <see cref="RegistrationService.Register"/> method.
		/// </summary>
		[Fact]
		public void RegisterNewUser()
		{
			string expectedUsername = "testUser" + Guid.NewGuid();
			var userId = registrationService.Register(expectedUsername, "testPassword");

			Assert.NotNull(userId);
			Assert.NotEqual(Guid.Empty, userId.Id);
			Assert.NotEqual(new DateTime(), userId.Registered);
			Assert.Equal(string.Empty, userId.ServiceUrl);
		}

		/// <summary>
		/// Test <see cref="RegistrationService.IsLoginAvailable"/> method.
		/// </summary>
		[Fact]
		public void CheckIsLoginAvailable()
		{
			bool isAvailable = registrationService.IsLoginAvailable("testUser" + Guid.NewGuid());
			Assert.True(isAvailable);
		}

		/// <summary>
		/// Test <see cref="RegistrationService.IsLoginAvailable"/> method.
		/// </summary>
		[Fact]
		public void CheckIsLoginNotAvailable()
		{
			string login = "testUser" + Guid.NewGuid();
			registrationService.Register(login, "testPassword");

			bool isAvailable = registrationService.IsLoginAvailable(login);

			Assert.False(isAvailable);
		}

		/// <summary>
		/// Test <see cref="RegistrationService.GenerateUniqueLogin"/> method.
		/// </summary>
		[Fact]
		public void GenerateUniqueUserLogin()
		{
			string uniqueLogin = registrationService.GenerateUniqueLogin();

			bool isAvailable = registrationService.IsLoginAvailable(uniqueLogin);
			Assert.True(isAvailable);
		}

		/// <summary>
		/// Test <see cref="RegistrationService.ResetPassword"/> method.
		/// </summary>
		[Fact(Skip = "not implemented")]
		public void ResetPassword()
		{
			string login = "testUser" + Guid.NewGuid();
			registrationService.Register(login, "testPassword");

			registrationService.ResetPassword(login, "new@email");
		}

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