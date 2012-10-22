//------------------------------------------------------------
// <copyright file="RegistrationService.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ServiceModel;
using Common.Logging;
using EmitMapper;
using Fab.Core;
using Fab.Server.Core.Contracts;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Services
{
	/// <summary>
	/// User registration service.
	/// </summary>
	[ErrorHandlingBehavior]
	public class RegistrationService : IRegistrationService
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
		/// Initializes a new instance of the <see cref="RegistrationService"/> class.
		/// </summary>
		public RegistrationService()
		{
			DefaultFolder = "|DataDirectory|";
			dbManager = new DatabaseManager();
		}

		#endregion

		#region Implementation of IRegistrationService

		/// <summary>
		/// Generate unique login name for new user.
		/// Something like "8AB3-9D27" from Guid.New() most likely
		/// will be unique without additional DB hit to make sure that it is really unique.
		/// </summary>
		/// <returns>Unique login name.</returns>
		public string GenerateUniqueLogin()
		{
			return Guid.NewGuid().Hash36();
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
				throw new ArgumentException("Username must not be empty.");
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
				throw new Exception("Username is too short. Minimum length is 5.");
			}

			// Check login max length
			if (newLogin.Length > 50)
			{
				throw new Exception("Username is too long. Maximum length is 50.");
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

			if (Properties.Settings.Default.Registration_Disabled)
			{
				var log = LogManager.GetCurrentClassLogger();
				log.Warn("Registration failed. Attempt to use username: " + login);

				var faultDetail = new FaultDetail
				{
					ErrorCode = "ERR-REGS-0",
					ErrorMessage = "Registration failed.",
					Description = "Sorry, the subscription is temporarily suspended."
				};

				throw new FaultException<FaultDetail>(
					faultDetail,
					new FaultReason(faultDetail.Description),
					new FaultCode("Receiver"));
			}

			var masterConnection = dbManager.GetMasterConnection(DefaultFolder);

			using (var mc = new MasterEntities(masterConnection))
			{
				var usersMapper = ObjectMapperManager.DefaultInstance.GetMapper<User, UserDTO>();

				// Check login uniqueness
				if (!ModelHelper.IsLoginAvailable(mc, newLogin))
				{
					var faultDetail = new FaultDetail
					{
						ErrorCode = "ERR-REGS-1",
						ErrorMessage = "Registration failed.",
						Description = string.Format("Sorry, but username \"{0}\" is already in use. Please, try to pick another username.", newLogin)
					};

					throw new FaultException<FaultDetail>(
						faultDetail,
						new FaultReason(faultDetail.Description),
						new FaultCode("Receiver"));
				}

				var user = new User
				{
					Id = Guid.NewGuid(),
					Login = newLogin,
					Password = password.Hash(),
					Registered = DateTime.UtcNow,
					IsDisabled = false,
					ServiceUrl = string.Empty	// default service for all users (for now)
				};

				// Create personal database for user and save path to it
				// TODO: use custom password here to encrypt database with
				user.DatabasePath = dbManager.CreatePersonalDatabase(user.Id, user.Registered, DefaultFolder /*, password*/);

				mc.Users.AddObject(user);
				mc.SaveChanges();

				// Creating default $ account
				var moneyService = new MoneyService { UserName = user.Login };
				moneyService.CreateAccount("Cash", 2);

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