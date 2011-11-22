//------------------------------------------------------------
// <copyright file="UserService.svc.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using EmitMapper;
using Fab.Server.Core;
using Fab.Server.Core.DTO;

namespace Fab.Server
{
	/// <summary>
	/// User service.
	/// </summary>
	public class UserService : IUserService
	{
		#region Const

		/// <summary>
		/// Key in Web.Config "appSettings" section that define default service url for user.
		/// </summary>
		private const string DefaultServiceUrlKey = "DefaultServiceUrl";

		#endregion

		#region Dependencies

		/// <summary>
		/// Database manager dependency.
		/// </summary>
		private readonly DatabaseManager dbManager;

		#endregion

		#region Default folder

		/// <summary>
		/// Default root folder for master and personal databases = |DataDirectory|.
		/// </summary>
		private string defaultFolder = "|DataDirectory|";

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

		/// <summary>
		/// Gets or sets default root folder for master and personal databases = |DataDirectory|.
		/// </summary>
		public string DefaultFolder
		{
			[DebuggerStepThrough]
			get { return defaultFolder; }

			[DebuggerStepThrough]
			set { defaultFolder = value; }
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="UserService"/> class.
		/// </summary>
		public UserService()
		{
			dbManager = new DatabaseManager();
		}

		#endregion

		#region Implementation of IUserService

		/// <summary>
		/// Generate unique login name for new user.
		/// </summary>
		/// <returns>Unique login name.</returns>
		public string GenerateUniqueLogin()
		{
			// Todo: use more sophisticate algorithm for uniqueness login generation 
			return "a" + Guid.NewGuid().GetHashCode();
		}

		/// <summary>
		/// Check user login name for uniqueness.
		/// </summary>
		/// <param name="login">User login.</param>
		/// <returns><c>true</c> if user login name is unique.</returns>
		public bool IsLoginAvailable(string login)
		{
			if (string.IsNullOrWhiteSpace(login))
			{
				throw new ArgumentException("Login must not be empty.");
			}

			// Remove leading and closing spaces (user typo)
			string newLogin = login.Trim();

			// Check login min & max length
			if (newLogin.Length < 5 || newLogin.Length > 50)
			{
				return false;
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				return ModelHelper.IsLoginAvailable(mc, newLogin);
			}
		}

		/// <summary>
		/// Register new user with unique login name and password.
		/// </summary>
		/// <param name="login">User login name.</param>
		/// <param name="password">User password.</param>
		/// <returns>Created user object.</returns>
		public UserDTO Register(string login, string password)
		{
			if (string.IsNullOrWhiteSpace(login))
			{
				throw new ArgumentException("Login must not be empty.");
			}

			if (string.IsNullOrWhiteSpace(password))
			{
				throw new ArgumentException("Password must not be empty.");
			}

			// Remove leading and closing spaces (user typo)
			string newLogin = login.Trim();

			// Check login min length
			if (newLogin.Length < 5)
			{
				throw new Exception("Login name is too short. Minimum length is 5.");
			}

			// Check login max length
			if (newLogin.Length > 50)
			{
				throw new Exception("Login name is too long. Maximum length is 50.");
			}

			// Check password min length
			if (password.Length < 5)
			{
				throw new Exception("New password is too short. Minimum length is 5.");
			}

			// Check password max length
			if (password.Length > 255)
			{
				throw new Exception("New password is too long. Maximum length is 255.");
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var usersMapper = ObjectMapperManager.DefaultInstance.GetMapper<User, UserDTO>();

				// Check login uniqueness
				if (!ModelHelper.IsLoginAvailable(mc, newLogin))
				{
					throw new Exception(string.Format("Login name \"{0}\" is already used. Please use another one.", newLogin));
				}

				var user = new User
				{
					Id = Guid.NewGuid(),
					Login = newLogin,
					Password = password.Hash(),
					Registered = DateTime.UtcNow,
					IsDisabled = false,
					ServiceUrl = ConfigurationManager.AppSettings[DefaultServiceUrlKey]
				};

				// Create personal database for user and save path to it
				// TODO: use custom password here to encrypt database with
				user.DatabasePath = dbManager.GetPersonalConnection(user.Id, user.Registered, DefaultFolder /*, password*/);

				mc.Users.AddObject(user);
				mc.SaveChanges();

				return usersMapper.Map(user);
			}
		}

		/// <summary>
		/// Change user password or email to new values.
		/// </summary>
		/// <param name="userId">User unique ID.</param>
		/// <param name="oldPassword">User old password.</param>
		/// <param name="newPassword">User new password.</param>
		/// <param name="newEmail">User new email.</param>
		public void Update(Guid userId, string oldPassword, string newPassword, string newEmail)
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

		/// <summary>
		/// Get user by unique login and password.
		/// </summary>
		/// <param name="login">User unique login name.</param>
		/// <param name="password">User password.</param>
		/// <returns>User instance.</returns>
		public UserDTO GetUser(string login, string password)
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
				var usersMapper = ObjectMapperManager.DefaultInstance.GetMapper<User, UserDTO>();

				var hash = password.Hash();
				var user = mc.Users.Where(u => u.Login == login.Trim() && u.Password == hash)
						.Select(u => u)
						.SingleOrDefault();

				if (user != null)
				{
					user.LastAccess = DateTime.UtcNow;
					mc.SaveChanges();
				}
				else
				{
					throw new Exception("User name or password is incorrect.");
				}

				return usersMapper.Map(user);
			}
		}

		/// <summary>
		/// If user with specified login name have email and this email is match to specified email,
		/// then system will reset current password for this user to auto generated new one
		/// and sent it to the specified email.
		/// </summary>
		/// <param name="login">User login name.</param>
		/// <param name="email">User email.</param>
		public void ResetPassword(string login, string email)
		{
			if (string.IsNullOrWhiteSpace(login))
			{
				throw new ArgumentException("Login must not be empty.");
			}

			if (string.IsNullOrWhiteSpace(email))
			{
				throw new ArgumentException("Email must not be empty.");
			}

			throw new NotImplementedException();
		}

		#endregion
	}
}
