//------------------------------------------------------------
// <copyright file="DatabaseManager.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using Fab.Server.Properties;

namespace Fab.Server.Core
{
	/// <summary>
	/// Create and delete users database in the file system.
	/// </summary>
	public class DatabaseManager
	{
		#region Constants

		/// <summary>
		/// Name of the default connection to the master database - 'MasterEntities'.
		/// </summary>
		private const string MasterConnectionName = "MasterEntities";

		/// <summary>
		/// Name of the default connection to the user personal database - 'ModelContainer'.
		/// </summary>
		private const string PersonalConnectionName = "ModelContainer";

		/// <summary>
		/// Connection string template to the SQL CE 4.0 database file: Data Source='{0}\{1}.sdf'; Password='{2}'
		/// </summary>
		private const string ConnectionStringTemplate = @"Data Source='{0}\{1}.sdf'; Password='{2}'";

		/// <summary>
		/// Connection string template to the SQL CE 4.0 database file: Data Source='{0}.sdf'; Password='{1}'
		/// </summary>
		private const string ConnectionStringSimpleTemplate = @"Data Source='{0}'; Password='{1}'";

		/// <summary>
		/// Master database file name.
		/// </summary>
		private const string MasterDatabaseName = "master";

		/// <summary>
		/// Default root folder for master and personal databases = |DataDirectory|.
		/// </summary>
		private const string DefaultFolder = "|DataDirectory|";

		/// <summary>
		/// Default password that will be used for database encryption.
		/// </summary>
		private const string DefaultPassword = "_s#2PDaq[mL>4v%F";

		/// <summary>
		/// Personal setting key: user-id.
		/// </summary>
		private const string UserIdKey = "user-id";

		/// <summary>
		/// Personal setting key: registration-date.
		/// </summary>
		private const string RegistrationDateKey = "registration-date";

		/// <summary>
		/// Insert setting value into "Settings" table in the user personal database.
		/// </summary>
		private const string SqlInsertSettingFormat = "INSERT INTO [Settings] ([Key], [Value]) VALUES ('{0}', '{1}')";

		#endregion

		#region Public Methods

		/// <summary>
		/// Ensure that master database file exist and return connection string to it.
		/// </summary>
		/// <param name="folder">Root folder for database. Usually should be "|DataDirectory|".</param>
		/// <param name="password">Password for database encryption.</param>
		/// <returns>Connection string to the master database.</returns>
		public string GetMasterConnection(string folder = DefaultFolder, string password = DefaultPassword)
		{
			var dataDirectory = ResolveDataDirectory(folder);
			var connectionString = GetMasterDbConnectionString(dataDirectory, password);

			if (!Directory.Exists(dataDirectory))
			{
				Directory.CreateDirectory(dataDirectory);
			}

			// Execute SQL scripts only once right after database file was created
			if (CreateDatabaseFile(connectionString))
			{
				ExecuteSqlScript(connectionString, Resources.master_001_setup);
			}

			return AddEntityMetadata(connectionString, MasterConnectionName);
		}

		/// <summary>
		/// Ensure that user personal database file exist and return connection string to it.
		/// </summary>
		/// <param name="userId">User ID for whom the personal database should be created.</param>
		/// <param name="registrationDate">Date when the user has been registered.</param>
		/// <param name="rootFolder">Root folder where all users personal databases located. Usually should be "|DataDirectory|".</param>
		/// <param name="password">Password for database encryption.</param>
		/// <returns>Connection string to the user personal database.</returns>
		public string GetPersonalConnection(Guid userId, DateTime registrationDate, string rootFolder = DefaultFolder, string password = DefaultPassword)
		{
			var dataDirectory = ResolveDataDirectory(rootFolder);
			var folder = GetFolderForPersonalDb(userId, registrationDate, dataDirectory);
			var connectionString = GetPersonalDbConnectionString(folder, userId, password);

			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
			}

			// Execute SQL scripts only once right after database file was created
			if (CreateDatabaseFile(connectionString))
			{
				ExecuteSqlScript(connectionString, Resources.personal_001_setup);
				ExecuteSqlScript(connectionString, string.Format(SqlInsertSettingFormat, UserIdKey, userId));
				ExecuteSqlScript(connectionString, string.Format(SqlInsertSettingFormat, RegistrationDateKey, registrationDate));
			}

			return AddEntityMetadata(connectionString, PersonalConnectionName);
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
			var connectionString = GetPersonalDbConnectionString(absolutePathToDatabase, password);
			
			return AddEntityMetadata(connectionString, PersonalConnectionName);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Construct connection string to the master database.
		/// </summary>
		/// <param name="folder">Path to the master database file.</param>
		/// <param name="password">Password for the master database file.</param>
		/// <returns>Connection string to the master database.</returns>
		private static string GetMasterDbConnectionString(string folder, string password)
		{
			return string.Format(ConnectionStringTemplate, folder, MasterDatabaseName, password);
		}

		/// <summary>
		/// Construct connection string to the user personal database.
		/// </summary>
		/// <param name="folder">Path to the user personal database file.</param>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="password">Password for the user personal database file.</param>
		/// <returns>Connection string to the user personal database.</returns>
		private static string GetPersonalDbConnectionString(string folder, Guid userId, string password)
		{
			return string.Format(ConnectionStringTemplate, folder, userId.ToString().ToLower(), password);
		}

		/// <summary>
		/// Construct connection string to the user personal database.
		/// </summary>
		/// <param name="file">Path to the user personal database file.</param>
		/// <param name="password">Password for the user personal database file.</param>
		/// <returns>Connection string to the user personal database.</returns>
		private static string GetPersonalDbConnectionString(string file, string password)
		{
			return string.Format(ConnectionStringSimpleTemplate, file, password);
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
		/// <param name="rootFolder">Root folder for all users personal databases.</param>
		/// <returns>Path to the user personal database file (without file name itself).</returns>
		private static string GetFolderForPersonalDb(Guid userId, DateTime registrationDate, string rootFolder)
		{
			var folderPath = Path.Combine(rootFolder,
										  registrationDate.ToString("yyyy"),
										  registrationDate.ToString("MM"),
										  registrationDate.ToString("dd"),
										  userId.ToString().ToLower());
			return folderPath;
		}

		/// <summary>
		/// Replace the "|DataDirectory|" pattern in path to corresponding value from AppDomain like path to "App_Data" directory.
		/// </summary>
		/// <param name="path">Relative path with "|DataDirectory|".</param>
		/// <returns>Absolute path to the original directory.</returns>
		private static string ResolveDataDirectory(string path)
		{
			return path.Replace("|DataDirectory|", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
		}

		/// <summary>
		/// Create the database file (only if it is not already created!) by connection string to it.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file that should be created.</param>
		/// <returns>True if the database file was created.</returns>
		private bool CreateDatabaseFile(string connectionString)
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
		/// Execute SQL CE script in specific database file context.
		/// </summary>
		/// <param name="connectionString">Connection string to the database file.</param>
		/// <param name="sqlScriptText">SQL CE script for execution. Every SQL command should be confirmed by following "GO" command.</param>
		private void ExecuteSqlScript(string connectionString, string sqlScriptText)
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