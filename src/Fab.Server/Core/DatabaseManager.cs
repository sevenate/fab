﻿//------------------------------------------------------------
// <copyright file="DatabaseManager.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Fab.Server.Properties;

namespace Fab.Server.Core
{
	/// <summary>
	/// Create master and users personal databases and provide connection strings
	/// to them for EF's <see cref="System.Data.Objects.ObjectContext"/>.
	/// </summary>
	public class DatabaseManager
	{
		#region Constants

		/// <summary>
		/// Name of the default connection to the master database - "MasterEntities".
		/// </summary>
		private const string MasterConnectionName = "MasterEntities";

		/// <summary>
		/// Name of the default connection to the user personal database - "ModelContainer".
		/// </summary>
		private const string PersonalConnectionName = "ModelContainer";

		/// <summary>
		/// Connection string template to the SQL CE 4.0 database file - "Data Source='{0}'; Password='{1}'; max database size=4096"
		/// Note: default "max database size" value is "128" Mb, but we set it to physical maximum ~ 4Gb.
		/// </summary>
		private const string ConnectionStringTemplate = @"Data Source='{0}'; Password='{1}'; max database size=4091";

		/// <summary>
		/// Master database file name - "master.sdf".
		/// </summary>
		private const string MasterDatabaseName = "master.sdf";

		/// <summary>
		/// Default root folder for master and personal databases = "|DataDirectory|".
		/// </summary>
		private const string DefaultFolder = "|DataDirectory|";

		/// <summary>
		/// Default password that will be used for database encryption.
		/// </summary>
		private const string DefaultPassword = "_s#2PDaq[mL>4v%F";

		/// <summary>
		/// Personal setting key - "user-id".
		/// </summary>
		private const string SettingsUserIdKey = "user-id";

		/// <summary>
		/// Personal setting key - "registration-date".
		/// </summary>
		private const string SettingsRegistrationDateKey = "registration-date";

		/// <summary>
		/// String template for inserting setting value into "Settings" table in the user personal database.
		/// </summary>
		private const string SqlInsertSettingFormat = "INSERT INTO [Settings] ([Key], [Value]) VALUES ('{0}', '{1}')";

		#endregion

		#region Public Methods

		/// <summary>
		/// Ensure that master database file exist and return connection string to it.
		/// </summary>
		/// <param name="rootFolder">Root folder for database. Usually should be "|DataDirectory|".</param>
		/// <param name="password">Password for database encryption.</param>
		/// <returns>Connection string to the master database.</returns>
		public string GetMasterConnection(string rootFolder = DefaultFolder, string password = DefaultPassword)
		{
			var dataDirectory = ResolveDataDirectory(rootFolder);

			if (!Directory.Exists(dataDirectory))
			{
				Directory.CreateDirectory(dataDirectory);
			}

			var masterDatabasePath = GetMasterDatabasePath(rootFolder);
			var connectionString = GetConnectionString(masterDatabasePath, password);

			// Execute SQL scripts only once right after database file was created
			if (CreateDatabaseFile(connectionString))
			{
				ExecuteSqlScript(connectionString, Resources.master_001_setup);
			}

			return AddEntityMetadata(connectionString, MasterConnectionName);
		}

		/// <summary>
		/// Create user personal database file and return path to it.
		/// </summary>
		/// <param name="userId">User ID for whom the personal database should be created.</param>
		/// <param name="registrationDate">Date when the user has been registered.</param>
		/// <param name="rootFolder">Root folder where all users personal databases located. Usually should be "|DataDirectory|".</param>
		/// <param name="password">Password for database encryption.</param>
		/// <returns>Path to user personal database file.</returns>
		public string CreatePersonalDatabase(Guid userId, DateTime registrationDate, string rootFolder = DefaultFolder, string password = DefaultPassword)
		{
			var folderForPersonalDb = GetFolderForPersonalDb(userId, registrationDate);
			var relativeDatabasePath = Path.Combine(folderForPersonalDb, userId.ToString().ToLower() + ".sdf");
			var absoluteFolderPathTemplate = rootFolder + "\\" + folderForPersonalDb;
			var absoluteFilePathTemplate = rootFolder + "\\" + relativeDatabasePath;

			var dataDirectory = ResolveDataDirectory(absoluteFolderPathTemplate);

			if (!Directory.Exists(dataDirectory))
			{
				Directory.CreateDirectory(dataDirectory);
			}

			var absolutePathToDatabase = ResolveDataDirectory(absoluteFilePathTemplate);
			var connectionString = GetConnectionString(absolutePathToDatabase, password);

			// Execute SQL scripts only once right after database file was created
			if (CreateDatabaseFile(connectionString))
			{
				ExecuteSqlScript(connectionString, Resources.personal_001_setup);
				ExecuteSqlScript(connectionString, string.Format(SqlInsertSettingFormat, SettingsUserIdKey, userId));
				ExecuteSqlScript(connectionString, string.Format(SqlInsertSettingFormat, SettingsRegistrationDateKey, registrationDate));
			}

			return absoluteFilePathTemplate;
		}

		/// <summary>
		/// Return connection string to existed user database file.
		/// Note, that file should exist at the moment when service try to open returned connection or run-time exception will be thrown.
		/// </summary>
		/// <param name="pathToDatabase">Relative path to personal database file.</param>
		/// <param name="password">Database encryption password.</param>
		/// <returns>Connection string to the user personal database.</returns>
		public string GetPersonalConnection(string pathToDatabase = DefaultFolder, string password = DefaultPassword)
		{
			var absolutePathToDatabase = ResolveDataDirectory(pathToDatabase);
			var connectionString = GetConnectionString(absolutePathToDatabase, password);
			return AddEntityMetadata(connectionString, PersonalConnectionName);
		}

		#region Service

		/// <summary>
		/// Shrink database file.
		/// Note, that file should exist at the moment when service try to open returned connection or run-time exception will be thrown.
		/// </summary>
		/// <param name="pathToDatabase">Relative path to database file.</param>
		/// <param name="password">Database encryption password.</param>
		public void ShrinkDatabase(string pathToDatabase = DefaultFolder, string password = DefaultPassword)
		{
			var absolutePathToDatabase = ResolveDataDirectory(pathToDatabase);
			var connectionString = GetConnectionString(absolutePathToDatabase, password);
			ShrinkDatabaseFile(connectionString);
		}

		/// <summary>
		/// Compact database file. Free more space compared to <see cref="ShrinkDatabase"/>, but require re-create database file.
		/// Note, that file should exist at the moment when service try to open returned connection or run-time exception will be thrown.
		/// </summary>
		/// <param name="pathToDatabase">Relative path to database file.</param>
		/// <param name="password">Database encryption password.</param>
		public void CompactDatabase(string pathToDatabase = DefaultFolder, string password = DefaultPassword)
		{
			var absolutePathToDatabase = ResolveDataDirectory(pathToDatabase);
			var connectionString = GetConnectionString(absolutePathToDatabase, password);
			//TODO: consider use ability to change database password here when user change his own password.
			CompactDatabaseFile(connectionString);
		}

		/// <summary>
		/// Verify database file integrity by comparing checksums.
		/// Note, that file should exist at the moment when service try to open returned connection or run-time exception will be thrown.
		/// </summary>
		/// <param name="pathToDatabase">Relative path to database file.</param>
		/// <param name="password">Database encryption password.</param>
		/// <returns>True if the checksums match and there is no database corruption; otherwise, false.</returns>
		public bool VerifyDatabase(string pathToDatabase = DefaultFolder, string password = DefaultPassword)
		{
			var absolutePathToDatabase = ResolveDataDirectory(pathToDatabase);
			var connectionString = GetConnectionString(absolutePathToDatabase, password);
			//TODO: consider use ability to change database password here when user change his own password.
			return VerifyDatabaseFile(connectionString);
		}

		/// <summary>
		/// Try to repair a corrupted database file.
		/// Note, that file should exist at the moment when service try to open returned connection or run-time exception will be thrown.
		/// </summary>
		/// <param name="pathToDatabase">Relative path to database file.</param>
		/// <param name="password">Database encryption password.</param>
		public void RepairDatabase(string pathToDatabase = DefaultFolder, string password = DefaultPassword)
		{
			var absolutePathToDatabase = ResolveDataDirectory(pathToDatabase);
			var connectionString = GetConnectionString(absolutePathToDatabase, password);
			//TODO: consider use ability to change database password here when user change his own password.
			RepairDatabaseFile(connectionString);
		}

		#endregion

		/// <summary>
		/// Return path to master database file.
		/// Note: there is no need in similar method for user's personal database file,
		/// since it is saved in user record in master database.
		/// </summary>
		/// <param name="rootFolder">Root database folder.</param>
		/// <returns>Path to master database file.</returns>
		public static string GetMasterDatabasePath(string rootFolder = DefaultFolder)
		{
			var dataDirectory = ResolveDataDirectory(rootFolder);
			return Path.Combine(dataDirectory, MasterDatabaseName);
		}

		/// <summary>
		/// Replace the "|DataDirectory|" pattern in path to corresponding value from AppDomain like path to "App_Data" directory.
		/// </summary>
		/// <param name="path">Relative path with "|DataDirectory|".</param>
		/// <returns>Absolute path to the original directory.</returns>
		public static string ResolveDataDirectory(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentNullException("path");
			}

			//var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory");
			//AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);

			var dataDirectory = ConfigurationManager.AppSettings["DataDirectory"];
				
			if (!Path.IsPathRooted(dataDirectory))
			{
				var root = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath ?? AppDomain.CurrentDomain.BaseDirectory;
				dataDirectory = Path.Combine(root, dataDirectory ?? string.Empty);
			}

			return path.Replace("|DataDirectory|", dataDirectory);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Construct connection string to the SQL CE database.
		/// </summary>
		/// <param name="file">Path to the database file.</param>
		/// <param name="password">Password for the database file.</param>
		/// <returns>Connection string to the SQL CE database.</returns>
		private static string GetConnectionString(string file, string password)
		{
			return string.Format(ConnectionStringTemplate, file, password);
		}

		/// <summary>
		/// Adds entity model specific metadata to the provided connection string.
		/// </summary>
		/// <param name="connectionString">Connection string to update.</param>
		/// <param name="connectionName">Name of the default connection string with entity metadata in the configuration file.</param>
		/// <returns>Connection string that could be used to create entity model context/container.</returns>
		private static string AddEntityMetadata(string connectionString, string connectionName)
		{
			string defaultConnectionString = WebConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
			return Regex.Replace(defaultConnectionString, "Data Source=.*\\;", match => connectionString);
		}

		/// <summary>
		/// Create "YYYY/MM/DD/GUID"-like folder structure to save user personal database file into.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="registrationDate">User registration date.</param>
		/// <returns>Path to the user personal database file (without file name itself).</returns>
		private static string GetFolderForPersonalDb(Guid userId, DateTime registrationDate)
		{
			var folderPath = Path.Combine(registrationDate.ToString("yyyy"),
										  registrationDate.ToString("MM"),
										  registrationDate.ToString("dd"),
										  userId.ToString().ToLower());
			return folderPath;
		}

		/// <summary>
		/// Create the database file (only if it is not already created!) by connection string to it.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file that should be created.</param>
		/// <returns>True if the database file was created.</returns>
		private static bool CreateDatabaseFile(string connectionString)
		{
			using (var connection = new SqlCeConnection(connectionString))
			{ 
				if (!File.Exists(connection.Database))
				{
					using (var sqlCeEngine = new SqlCeEngine(connectionString))
					{
						sqlCeEngine.CreateDatabase();
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Shrink the database file by connection string to it.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file that should be shrink.</param>
		private static void ShrinkDatabaseFile(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}

			using (var sqlCeEngine = new SqlCeEngine(connectionString))
			{
				sqlCeEngine.Shrink();
			}
		}

		/// <summary>
		/// Compact the database file by connection string to it.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file that should be compact.</param>
		/// <param name="newConnectionString">Connection string to the new destination database file that will be created.</param>
		private static void CompactDatabaseFile(string connectionString, string newConnectionString = null)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}

			using (var sqlCeEngine = new SqlCeEngine(connectionString))
			{
				// Specify null destination connection string for in-place compaction
				sqlCeEngine.Compact(null);
				
				if (!string.IsNullOrWhiteSpace(newConnectionString))
				{
					// Specify connection string for new database options; The following 
					// tokens are valid:
					//      - Password
					//      - LCID
					//      - Encrypt
					// 
					// All other SqlCeConnection.ConnectionString tokens are ignored
					sqlCeEngine.Compact(newConnectionString);
				}
			}
		}

		/// <summary>
		/// Recalculates the checksums for each page in the database and compares the new checksums to the expected values. 
		/// </summary>
		/// <param name="connectionString">Connection string to the database file that should be verified.</param>
		/// <returns>True if the checksums match and there is no database corruption; otherwise, false.</returns>
		private static bool VerifyDatabaseFile(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}

			using (var sqlCeEngine = new SqlCeEngine(connectionString))
			{
				return sqlCeEngine.Verify();
			}
		}

		/// <summary>
		/// Try to repair a corrupted database file by connection string to it.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file that should be repaired.</param>
		private static void RepairDatabaseFile(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new ArgumentNullException("connectionString");
			}

			using (var sqlCeEngine = new SqlCeEngine(connectionString))
			{
				//TODO: consider using different repair param
				sqlCeEngine.Repair(connectionString, RepairOption.RecoverAllOrFail);
			}
		}

		/// <summary>
		/// Execute SQL CE script in specific database file context.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file.</param>
		/// <param name="sqlScriptText">SQL CE script for execution. Every SQL command should be confirmed by following "GO" command.</param>
		private static void ExecuteSqlScript(string connectionString, string sqlScriptText)
		{
			using (var connection = new SqlCeConnection(connectionString))
			{
				string[] commands = sqlScriptText.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

				var cmd = new SqlCeCommand
				{
					Connection = connection
				};

				connection.Open();

				foreach (var text in commands)
				{
					cmd.CommandText = text;
					cmd.ExecuteNonQuery();
				}
			}
		}

		#endregion
	}
}