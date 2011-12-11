//------------------------------------------------------------
// <copyright file="UserService.svc.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Linq;
using System.ServiceModel;
using EmitMapper;
using Fab.Server.Core.Contracts;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Services
{
	/// <summary>
	/// User service.
	/// </summary>
	public class UserService : IUserService
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

		#region Current user

		/// <summary>
		/// Primary identity name.
		/// </summary>
		private string userName;

		/// <summary>
		/// Gets or sets primary identity name.
		/// </summary>
		public string UserName
		{
			get
			{
				return ServiceSecurityContext.Current == null
				       	? userName
				       	: ServiceSecurityContext.Current.PrimaryIdentity.Name;
			}
			set { userName = value; }
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="UserService"/> class.
		/// </summary>
		public UserService()
		{
			DefaultFolder = "|DataDirectory|";
			dbManager = new DatabaseManager();
		}

		#endregion

		#region Internal authentidation

		/// <summary>
		/// Authenticate user by validating his password.
		/// </summary>
		/// <param name="login">User unique login name.</param>
		/// <param name="password">User password.</param>
		/// <returns><c>true</c> if provided credentials exist in the master database.</returns>
		internal bool Authenticate(string login, string password)
		{
			if (string.IsNullOrWhiteSpace(login))
			{
				throw new ArgumentException("Login must not be empty.");
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException("Password must not be empty.");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var hash = password.Hash();
				var user = mc.Users.Where(u => u.Login == login.Trim() && u.Password == hash)
					.Select(u => u)
					.SingleOrDefault();

				if (user != null)
				{
					//TODO: consider do NOT write in master database every user operation;
					// for example, try to write in the user personal database instead.
					user.LastAccess = DateTime.UtcNow;
					mc.SaveChanges();
					return true;
				}
			}

			return false;
		}

		#endregion

		#region Implementation of IUserService

		/// <summary>
		/// Get user info based on authenticated username.
		/// </summary>
		/// <returns>User info.</returns>
		public UserDTO GetUser()
		{
			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var usersMapper = ObjectMapperManager.DefaultInstance.GetMapper<User, UserDTO>();

				var user = mc.Users.Where(u => u.Login == UserName)
					.Select(u => u)
					.Single();

				//TODO: consider do NOT write in master database on user "login" action;
				// for example, try to write in the user personal database instead.
				user.LastAccess = DateTime.UtcNow;
				mc.SaveChanges();
				return usersMapper.Map(user);
			}
		}

		/// <summary>
		/// Change user password or email to new values.
		/// </summary>
		/// <param name="oldPassword">User old password.</param>
		/// <param name="newPassword">User new password.</param>
		/// <param name="newEmail">User new email.</param>
		public void Update(string oldPassword, string newPassword, string newEmail)
		{
			if (string.IsNullOrWhiteSpace(oldPassword))
			{
				throw new ArgumentException("Old password must not be empty.");
			}

			if (string.IsNullOrWhiteSpace(newPassword))
			{
				throw new ArgumentException("New password must not be empty.");
			}

			// Check password min length
			if (newPassword.Length < 5)
			{
				throw new Exception("New password is too short. Minimum length is 5.");
			}

			// Check password max length
			if (newPassword.Length > 256)
			{
				throw new Exception("New password is too long. Maximum length is 256.");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				User user = ModelHelper.GetUserByLogin(mc, UserName);

				if (user.Password != oldPassword.Hash())
				{
					throw new Exception("Old password is incorrect.");
				}

				user.Password = newPassword.Hash();
				user.Email = string.IsNullOrWhiteSpace(newEmail)
								? null
								: newEmail.Trim();

				mc.SaveChanges();
			}
		}

		#endregion
	}
}
