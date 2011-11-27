//------------------------------------------------------------
// <copyright file="DatabaseManagerTests.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.IO;
using Fab.Server.Core;
using Xunit;

namespace Fab.Server.Tests
{
	/// <summary>
	/// Unit tests for <see cref="DatabaseManager" />.
	/// </summary>
	public class DatabaseManagerTests : IDisposable
	{
		/// <summary>
		/// Test user registration date - 5 Sept 2011.
		/// </summary>
		private readonly DateTime registrationDate = new DateTime(2011, 9, 5);

		/// <summary>
		/// Database password - 'testPassword'.
		/// </summary>
		private const string Password = "testPassword";

		/// <summary>
		/// Root folder for all test databases - 'DB_test'.
		/// </summary>
		private const string DefaultFolder = "db";

		/// <summary>
		/// Test user ID (random GUID).
		/// </summary>
		private Guid userId = Guid.NewGuid();

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseManagerTests"/> class.
		/// </summary>
		public DatabaseManagerTests()
		{
			Dispose();
		}

		#region Tests

		/// <summary>
		/// Test <see cref="DatabaseManager.GetPersonalConnection"/> method.
		/// </summary>
		[Fact]
		public void CreatePersonalDb()
		{
			var userGuid = userId.ToString();
			var expectedConnectoinString =
				string.Format(@"metadata=res://*/Core.Model.csdl|res://*/Core.Model.ssdl|res://*/Core.Model.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source='{0}\2011\09\05\{1}\{1}.sdf'; Password='{2}'""", DefaultFolder.ToLower(), userGuid.ToLower(), Password);
			
			var manager = new DatabaseManager();
			var connectionString = manager.GetPersonalConnection(userId, registrationDate, DefaultFolder, Password);

			Assert.True(connectionString == expectedConnectoinString);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.GetMasterConnection"/> method.
		/// </summary>
		[Fact]
		public void CreateMasterDb()
		{
			var expectedConnectoinString =
				string.Format(@"metadata=res://*/Core.Master.csdl|res://*/Core.Master.ssdl|res://*/Core.Master.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source='{0}\master.sdf'; Password='{1}'""", DefaultFolder.ToLower(), Password);

			var manager = new DatabaseManager();
			var connectionString = manager.GetMasterConnection(DefaultFolder, Password);

			Assert.True(connectionString == expectedConnectoinString);
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