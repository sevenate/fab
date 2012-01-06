//------------------------------------------------------------
// <copyright file="AdminService.svc.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using EmitMapper;
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
						  .ToList();
			}

			foreach (var adminUserDTO in records)
			{
				var absoluteFile = DatabaseManager.ResolveDataDirectory(adminUserDTO.DatabasePath);

				if (File.Exists(absoluteFile))
				{
					adminUserDTO.DatabaseSize = new FileInfo(absoluteFile).Length;
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
			var masterConnectioString = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnectioString))
			{
				var user = ModelHelper.GetUserById(mc, userId);
				mc.Users.DeleteObject(user);
				mc.SaveChanges();
			}
		}

		/// <summary>
		/// Update specific user data.
		/// </summary>
		/// <param name="userDto">User to update.</param>
		/// <returns>Latest "DisabledChanged" value.</returns>
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
				user.Password = userDto.Passoword.Hash();
				user.Email = string.IsNullOrWhiteSpace(userDto.Email)
								? null
								: userDto.Email.Trim();
				user.DatabasePath = userDto.DatabasePath;
				user.ServiceUrl = userDto.ServiceUrl;
				user.IsDisabled = userDto.IsDisabled;
				user.DisabledChanged = DateTime.UtcNow;

				mc.SaveChanges();

				// After this method call DisabledChanged will be always initialized
				return user.DisabledChanged.Value;
			}
		}

		#endregion
	}
}