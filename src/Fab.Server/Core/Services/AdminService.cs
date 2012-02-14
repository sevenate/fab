//------------------------------------------------------------
// <copyright file="AdminService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using EmitMapper;
using Fab.Core;
using Fab.Server.Core.Contracts;
using Fab.Server.Core.DTO;
using Fab.Server.Core.Filters;

namespace Fab.Server.Core.Services
{
	/// <summary>
	/// Administrative service.
	/// </summary>
	[ErrorHandlingBehavior]
	public class AdminService : IAdminService
	{
		#region Dependencies

		/// <summary>
		/// Database manager dependency.
		/// </summary>
		private readonly DatabaseManager dbManager;

		#endregion

		#region Default folder

		/// <summary>
		/// Gets or sets default root folder for master and personal databases = |DataDirectory|.
		/// </summary>
		public string DefaultFolder { get; set; }

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminService"/> class.
		/// </summary>
		public AdminService()
		{
			DefaultFolder = "|DataDirectory|";
			dbManager = new DatabaseManager();
		}

		#endregion

		#region Implementation of IAdminService

		/// <summary>
		/// Return count of users based on search filter.
		/// </summary>
		/// <param name="queryFilter">Filter conditions.</param>
		/// <returns>Count of filtered users.</returns>
		public int GetUsersCount(IQueryFilter queryFilter)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("GetUsersCount");

			if (queryFilter == null)
			{
				throw new ArgumentNullException("queryFilter");
			}

			int count;

			var masterConnectioString = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnectioString))
			{
				var query = from u in mc.Users
							select u;

				if (queryFilter is TextSearchFilter)
				{
					var textSearchFilter = queryFilter as TextSearchFilter;

					if (!string.IsNullOrEmpty(textSearchFilter.Contains))
					{
						query = from user in query
								where user.Login.Contains(textSearchFilter.Contains)
								   || user.Email.Contains(textSearchFilter.Contains)
								   || user.DatabasePath.Contains(textSearchFilter.Contains)
								   || user.ServiceUrl.Contains(textSearchFilter.Contains)
								select user;
					}
				}

				if (queryFilter.NotOlderThen.HasValue)
				{
					query = from user in query
							where user.LastAccess >= queryFilter.NotOlderThen.Value
							select user;
				}

				if (queryFilter.Upto.HasValue)
				{
					query = from user in query
							where user.LastAccess < queryFilter.Upto.Value
							select user;
				}

				query = query.OrderBy(user => user.Registered);

				if (queryFilter.Skip.HasValue)
				{
					query = query.Skip(queryFilter.Skip.Value);
				}

				if (queryFilter.Take.HasValue)
				{
					query = query.Take(queryFilter.Take.Value);
				}

				count = query.Count();
			}

			return count;
		}

		/// <summary>
		/// Return filtered list of registered users from the system.
		/// </summary>
		/// <param name="queryFilter">Filter conditions.</param>
		/// <returns>List of users.</returns>
		public IList<AdminUserDTO> GetUsers(IQueryFilter queryFilter)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("GetUsers");

			if (queryFilter == null)
			{
				throw new ArgumentNullException("queryFilter");
			}

			var records = new List<AdminUserDTO>();

			var masterConnectioString = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnectioString))
			{
				//TODO: remove duplicated code with GetJournalsCounts() method
				var query = from u in mc.Users
							select u;

				if (queryFilter is TextSearchFilter)
				{
					var textSearchFilter = queryFilter as TextSearchFilter;

					if (!string.IsNullOrEmpty(textSearchFilter.Contains))
					{
						query = from user in query
								where user.Login.Contains(textSearchFilter.Contains)
								   || user.Email.Contains(textSearchFilter.Contains)
								   || user.DatabasePath.Contains(textSearchFilter.Contains)
								   || user.ServiceUrl.Contains(textSearchFilter.Contains)
								select user;
					}
				}

				if (queryFilter.NotOlderThen.HasValue)
				{
					query = from user in query
							where user.LastAccess >= queryFilter.NotOlderThen.Value
							select user;
				}

				if (queryFilter.Upto.HasValue)
				{
					query = from user in query
							where user.LastAccess < queryFilter.Upto.Value
							select user;
				}

				query = query.OrderBy(user => user.Registered);

				if (queryFilter.Skip.HasValue)
				{
					query = query.Skip(queryFilter.Skip.Value);
				}

				if (queryFilter.Take.HasValue)
				{
					query = query.Take(queryFilter.Take.Value);
				}
				// End of duplicated code

				var res = query.ToList();

				// No users take place yet, so nothing to return
				if (res.Count == 0)
				{
					return records;
				}

				var userMaper = ObjectMapperManager.DefaultInstance.GetMapper<User, AdminUserDTO>();

				records = res.Select(userMaper.Map)
							 .Select(dto =>
							 {
								 // Do not return password hash to client
								 // by security considerations
								 dto.Password = null;
								 return dto;
							 })
						  .ToList();
			}

			var drivesCache = new Dictionary<string, bool>();

			foreach (var adminUser in records)
			{
				var resolvedFile = DatabaseManager.ResolveDataDirectory(adminUser.DatabasePath);

				if (File.Exists(resolvedFile))
				{
					var file = new FileInfo(resolvedFile);
					adminUser.DatabaseSize = file.Length;

					// C:\ or D:\ etc.
					var driveName = Path.GetPathRoot(file.FullName);

					if (!string.IsNullOrEmpty(driveName))
					{
						if (!drivesCache.ContainsKey(driveName))
						{
							try
							{
								// Check drive free space info availability
								if (new DriveInfo(driveName).AvailableFreeSpace > 0)
								{
									drivesCache[driveName] = true;
								}
							}
							catch (UnauthorizedAccessException)
							{
								LogManager.GetCurrentClassLogger().Error("DriveInfo.AvailableFreeSpace for " + driveName + " is denied.");
								drivesCache[driveName] = false;
							}
						}

						if (drivesCache[driveName])
						{
							// Free space available for IIS AppPool user account, not the entire disk
							long freeSpace = new DriveInfo(driveName).AvailableFreeSpace;
							adminUser.FreeDiskSpaceAvailable = freeSpace;
						}
					}
				}
			}

			return records;
		}

		/// <summary>
		/// Delete specific user from master database records by his internal unique ID.
		/// Note: user personal database file will NOT be deleted since this is manual operation.
		/// </summary>
		/// <param name="userId">User ID to delete.</param>
		public void DeleteUser(Guid userId)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("DisableUser");

			if (userId == Guid.Empty)
			{
				throw new ArgumentException("userId");
			}

			var masterConnectioString = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnectioString))
			{
				var user = ModelHelper.GetUserById(mc, userId);
				mc.Users.DeleteObject(user);
				mc.SaveChanges();

				var absolutePath = DatabaseManager.ResolveDataDirectory(user.DatabasePath);
				var userFolder = new FileInfo(absolutePath).Directory;

				if (userFolder != null)
				{
					var deletedFolderName = "DELETED_" + userFolder.Name;
					var parentUserFolder = userFolder.Parent;

					if (parentUserFolder != null)
					{
						var targetFolder = new DirectoryInfo(Path.Combine(parentUserFolder.FullName, deletedFolderName));
						Directory.Move(userFolder.FullName, targetFolder.FullName);
					}
				}
			}
		}

		/// <summary>
		/// Update specific user data.
		/// </summary>
		/// <param name="userDto">User to update.</param>
		/// <returns>Last updated date.</returns>
		public DateTime UpdateUser(AdminUserDTO userDto)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("UpdateUser");

			if (userDto == null)
			{
				throw new ArgumentNullException("userDto");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				User user = ModelHelper.GetUserById(mc, userDto.Id);

				user.Login = userDto.Login;

				if (!string.IsNullOrEmpty(userDto.Password))
				{
					user.Password = userDto.Password.Hash();
				}

				user.Email = string.IsNullOrWhiteSpace(userDto.Email)
								? null
								: userDto.Email.Trim();
				user.DatabasePath = userDto.DatabasePath;
				user.ServiceUrl = userDto.ServiceUrl;
				user.IsDisabled = userDto.IsDisabled;
				user.DisabledChanged = DateTime.UtcNow;

				mc.SaveChanges();

				// After this method call "user.DisabledChanged" will be always initialized
				return user.DisabledChanged.Value;
			}
		}

		#region User

		/// <summary>
		/// Optimize user personal database file size.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>Database file after optimization.</returns>
		public long OptimizeUserDatabase(Guid userId)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("OptimizeUserDatabase");

			if (userId == Guid.Empty)
			{
				throw new ArgumentException("userId");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var user = ModelHelper.GetUserById(mc, userId);
				var absolutePath = DatabaseManager.ResolveDataDirectory(user.DatabasePath);
				dbManager.ShrinkDatabase(absolutePath);
				return new FileInfo(absolutePath).Length;
			}
		}

		/// <summary>
		/// Verify user personal database file integrity by comparing checksums.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>True if the checksums match and there is no database corruption; otherwise, false.</returns>
		public bool VerifyUserDatabase(Guid userId)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("VerifyUserDatabase");

			if (userId == Guid.Empty)
			{
				throw new ArgumentException("userId");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var user = ModelHelper.GetUserById(mc, userId);
				var absolutePath = DatabaseManager.ResolveDataDirectory(user.DatabasePath);
				return dbManager.VerifyDatabase(absolutePath);
			}
		}

		/// <summary>
		/// Try to repair a corrupted personal database file.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>Database file after repair.</returns>
		public long RepairUserDatabase(Guid userId)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("RepairUserDatabase");

			if (userId == Guid.Empty)
			{
				throw new ArgumentException("userId");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var user = ModelHelper.GetUserById(mc, userId);
				var absolutePath = DatabaseManager.ResolveDataDirectory(user.DatabasePath);
				dbManager.RepairDatabase(absolutePath);
				return new FileInfo(absolutePath).Length;
			}
		}

		#endregion

		#region Master

		/// <summary>
		/// Optimize master database file size.
		/// </summary>
		/// <returns>Database file after optimization.</returns>
		public long OptimizeMasterDatabase()
		{
			LogManager.GetCurrentClassLogger().LogClientIP("OptimizeMasterDatabase");

			// Assume that master database already exist
			var absolutePath = DatabaseManager.GetMasterDatabasePath(DefaultFolder);
			dbManager.ShrinkDatabase(absolutePath);
			return new FileInfo(absolutePath).Length;
		}

		/// <summary>
		/// Verify master database file integrity by comparing checksums.
		/// </summary>
		/// <returns>True if the checksums match and there is no database corruption; otherwise, false.</returns>
		public bool VerifyMasterDatabase()
		{
			LogManager.GetCurrentClassLogger().LogClientIP("VerifyMasterDatabase");

			// Assume that master database already exist
			var absolutePath = DatabaseManager.GetMasterDatabasePath(DefaultFolder);
			return dbManager.VerifyDatabase(absolutePath);
		}

		/// <summary>
		/// Try to repair a corrupted master database file.
		/// </summary>
		/// <returns>Database file after repair.</returns>
		public long RepairMasterDatabase()
		{
			LogManager.GetCurrentClassLogger().LogClientIP("RepairMasterDatabase");

			// Assume that master database already exist
			var absolutePath = DatabaseManager.GetMasterDatabasePath(DefaultFolder);
			dbManager.RepairDatabase(absolutePath);
			return new FileInfo(absolutePath).Length;
		}

		#endregion

		/// <summary>
		/// Update (check/recalculate) cached values for user like accounts current balance, categories used statistic etc.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <param name="checkOnly">If <c>false</c> - difference between cached and actual values will be fixed.</param>
		/// <returns>List of maintenance object (one per account) with info about actual and previous cached values.</returns>
		public IList<AccountMaintenanceDTO> UpdateCachedValuesForUserAccounts(Guid userId, bool checkOnly)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("UpdateCachedValues");

			if (userId == Guid.Empty)
			{
				throw new ArgumentException("userId");
			}

			var result = new List<AccountMaintenanceDTO>();

			using (var mc = new ModelContainer(GetPersonalConnection(userId)))
			{
				var accountMapper = ObjectMapperManager.DefaultInstance.GetMapper<Account, AccountMaintenanceDTO>(AccountMapper.AccountMappingConfigurator);

				var accounts = mc.Accounts.Include("AssetType")
					//.Where(a => !a.IsSystem && !a.IsClosed)
					.ToList();

				foreach (var account in accounts)
				{
					var maintenanceDto = accountMapper.Map(account);
					maintenanceDto.AssetName = account.AssetType.Name;

					// Update balance
					if (mc.Postings.Any(posting => posting.Account.Id == account.Id))
					{
						var actualBalance = mc.Postings.Where(posting => posting.Account.Id == account.Id)
												  .Sum(p => p.Amount);

						maintenanceDto.ActualBalance = actualBalance;

						if (!checkOnly)
						{
							account.Balance = actualBalance;
						}
					}

					// Update postings count
					var actualPostingsCount = mc.Postings.Count(posting => posting.Account.Id == account.Id);

					maintenanceDto.ActualPostingsCount = actualPostingsCount;

					if (!checkOnly)
					{
						account.PostingsCount = actualPostingsCount;
					}

					// Update first posting date
					var actualFirstPosting = mc.Postings.Where(posting => posting.Account.Id == account.Id)
																.OrderBy(posting => posting.Date)
																.FirstOrDefault();

					if (actualFirstPosting != null)
					{
						maintenanceDto.ActualFirstPostingDate = actualFirstPosting.Date.IsUtc();
					}

					if (!checkOnly)
					{
						account.FirstPostingDate = maintenanceDto.ActualFirstPostingDate;
					}

					// Update last posting date
					var actualLastPosting = mc.Postings.Where(posting => posting.Account.Id == account.Id)
																.OrderByDescending(posting => posting.Date)
																.FirstOrDefault();

					if (actualLastPosting != null)
					{
						maintenanceDto.ActualLastPostingDate = actualLastPosting.Date.IsUtc();
					}

					if (!checkOnly)
					{
						account.LastPostingDate = maintenanceDto.ActualLastPostingDate;
					}

					result.Add(maintenanceDto);
				}

				// Updated cached values if required
				if (!checkOnly)
				{
					mc.SaveChanges();
				}
			}

			return result;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets connection string to the user personal database based on user ID.
		/// </summary>
		/// <param name="userId">Unique user ID.</param>
		/// <returns>Connection string to the user personal database.</returns>
		private string GetPersonalConnection(Guid userId)
		{
			if (userId == Guid.Empty)
			{
				throw new ArgumentException("userId");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);
			User user;

			using (var mc = new MasterEntities(masterConnection))
			{
				user = mc.Users.Single(u => u.Id == userId);
			}

			return dbManager.GetPersonalConnection(user.DatabasePath);
		}

		#endregion
	}
}