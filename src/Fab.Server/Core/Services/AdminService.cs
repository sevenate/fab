//------------------------------------------------------------
// <copyright file="AdminService.svc.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using EmitMapper;
using Fab.Server.Core.Contracts;
using Fab.Server.Core.DTO;

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
		/// Retrieve all registered users from the system.
		/// </summary>
		/// <returns>All users.</returns>
		public IList<AdminUserDTO> GetAllUsers()
		{
			LogManager.GetCurrentClassLogger().LogClientIP("GetAllUsers");
			var masterConnectioString = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnectioString))
			{
				var userMaper = ObjectMapperManager.DefaultInstance.GetMapper<User, AdminUserDTO>();

				return mc.Users.OrderBy(u => u.Registered)
									.ToList()
									.Select(userMaper.Map)
									.ToList();
			}
		}

		/// <summary>
		/// Disable login for specific user by his internal unique ID.
		/// </summary>
		/// <param name="userId">User ID to disable.</param>
		public void DisableUser(Guid userId)
		{
			LogManager.GetCurrentClassLogger().LogClientIP("DisableUser");
			var masterConnectioString = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnectioString))
			{
				var user = ModelHelper.GetUserById(mc, userId);
				user.IsDisabled = true;
				user.DisabledChanged = DateTime.UtcNow;
				mc.SaveChanges();
			}
		}

		#endregion
	}
}