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
		/// Test <see cref="DatabaseManager.GetMasterConnection"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Create_Master_Database()
// ReSharper restore InconsistentNaming
		{
			var expectedConnectoinString =
				string.Format(@"metadata=res://*/Core.Master.csdl|res://*/Core.Master.ssdl|res://*/Core.Master.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source='{0}\master.sdf'; Password='{1}'""", DefaultFolder.ToLower(), Password);

			var manager = new DatabaseManager();
			var connectionString = manager.GetMasterConnection(DefaultFolder, Password);

			Assert.True(connectionString == expectedConnectoinString);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.GetPersonalConnection"/> method.
		/// </summary>
		[Fact]
// ReSharper disable InconsistentNaming
		public void Create_Personal_Database()
// ReSharper restore InconsistentNaming
		{
			var userGuid = userId.ToString();
			var expectedConnectoinString =
				string.Format(@"metadata=res://*/Core.Model.csdl|res://*/Core.Model.ssdl|res://*/Core.Model.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source='{0}\2011\09\05\{1}\{1}.sdf'; Password='{2}'""", DefaultFolder.ToLower(), userGuid.ToLower(), Password);

			var manager = new DatabaseManager();
			var databasePath = manager.CreatePersonalDatabase(userId, registrationDate, DefaultFolder, Password);
			var connectionString = manager.GetPersonalConnection(databasePath, Password);

			Assert.True(connectionString == expectedConnectoinString);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.ShrinkDatabase"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Shrink_Personal_Database()
		// ReSharper restore InconsistentNaming
		{
			var manager = new DatabaseManager();
			var databasePath = manager.CreatePersonalDatabase(userId, registrationDate, DefaultFolder, Password);
			var fileName = DatabaseManager.ResolveDataDirectory(databasePath);
			var initialFileSize = new FileInfo(fileName).Length;

			manager.ShrinkDatabase(databasePath, Password);

			var fileSizeAfterShrink = new FileInfo(fileName).Length;

			Assert.True(initialFileSize > fileSizeAfterShrink);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.CompactDatabase"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Compact_Personal_Database()
		// ReSharper restore InconsistentNaming
		{
			var manager = new DatabaseManager();
			var databasePath = manager.CreatePersonalDatabase(userId, registrationDate, DefaultFolder, Password);
			var fileName = DatabaseManager.ResolveDataDirectory(databasePath);
			var initialFileSize = new FileInfo(fileName).Length;

			manager.CompactDatabase(databasePath, Password);

			var fileSizeAfterCompact = new FileInfo(fileName).Length;

			Assert.True(initialFileSize > fileSizeAfterCompact);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.VerifyDatabase"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Verify_Personal_Database()
		// ReSharper restore InconsistentNaming
		{
			var manager = new DatabaseManager();
			var databasePath = manager.CreatePersonalDatabase(userId, registrationDate, DefaultFolder, Password);

			var isOk = manager.VerifyDatabase(databasePath, Password);

			Assert.True(isOk);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.RepairDatabase"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Repair_Personal_Database()
		// ReSharper restore InconsistentNaming
		{
			var manager = new DatabaseManager();
			var databasePath = manager.CreatePersonalDatabase(userId, registrationDate, DefaultFolder, Password);

			manager.RepairDatabase(databasePath, Password);
		}

		/// <summary>
		/// Test <see cref="DatabaseManager.GetMasterDatabasePath"/> method.
		/// </summary>
		[Fact]
		// ReSharper disable InconsistentNaming
		public void Shrink_Master_Database()
		// ReSharper restore InconsistentNaming
		{
			var manager = new DatabaseManager();
			
			//Creating master database first time
			manager.GetMasterConnection(DefaultFolder, Password);
			
			var databasePath = DatabaseManager.GetMasterDatabasePath(DefaultFolder);

			var fileName = DatabaseManager.ResolveDataDirectory(databasePath);
			var initialFileSize = new FileInfo(fileName).Length;

			manager.ShrinkDatabase(databasePath, Password);

			var fileSizeAfterShrink = new FileInfo(fileName).Length;

			Assert.True(initialFileSize == fileSizeAfterShrink);
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