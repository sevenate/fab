//------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO.IsolatedStorage;
using Caliburn.Micro;
using Fab.Client.Framework.Filters;
using Fab.Client.Localization;
using Fab.Core.Framework;
using Fab.Client.Resources;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// View-model for <see cref="LoginView"/> dialog.
	/// </summary>
	[Export(typeof(LoginViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class LoginViewModel : LocalizableScreen, ICanBeBusy, IHandle<LoggedOutMessage>
	{
		#region Constants

		/// <summary>
		/// Minimum allowed username length.
		/// </summary>
		private const int MinimumUsernameLength = 5;

		/// <summary>
		/// Minimum allowed password length.
		/// </summary>
		private const int MinimumPasswordLength = 5;

		/// <summary>
		/// Authorization failed message.
		/// </summary>
		private const string AuthenticationFailed = "The username or password provided is incorrect.";

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Events exchange entry point.</param>
		[ImportingConstructor]
		public LoginViewModel(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);

			displayName = Strings.LoginView_Login;

#if DEBUG
			ShowCharacters = true;
#endif
		}

		#endregion

		public IEnumerable<CultureInfo> Cultures
		{
			get { return Translator.SupportedCultures; }
		}

		public CultureInfo CurrentCulture
		{
			get { return Translator.CurrentCulture; }
			set { Translator.CurrentCulture = value; }
		}

		#region Overrides of Screen

		private string displayName;

		/// <summary>
		/// Gets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return displayName; }
		}

		#endregion

		#region Implementation of ILoginViewModel

		#region Username DP

		/// <summary>
		/// User name.
		/// </summary>
		private string username;

		/// <summary>
		/// Gets or sets user name.
		/// </summary>
		public string Username
		{
			get { return username; }
			set
			{
				username = value;
				NotifyOfPropertyChange(() => Username);
			}
		}

		#endregion

		#region Password DP

		/// <summary>
		/// User password.
		/// </summary>
		private string password;

		/// <summary>
		/// Gets or sets user password.
		/// </summary>
		public string Password
		{
			get { return password; }
			set
			{
				password = value;
				NotifyOfPropertyChange(() => Password);
			}
		}

		#endregion

		#region Username is focused DP

		/// <summary>
		/// Specify whether a <see cref="Username"/> field is focused.
		/// </summary>
		private bool usernameIsFocused;

		/// <summary>
		/// Gets or sets a value indicating whether a <see cref="Username"/> field is focused.
		/// </summary>
		public bool UsernameIsFocused
		{
			get { return usernameIsFocused; }
			set
			{
				usernameIsFocused = value;
				NotifyOfPropertyChange(() => UsernameIsFocused);
			}
		}

		#endregion

		#region Password is focused DP

		/// <summary>
		/// Specify whether a <see cref="Password"/> field is focused.
		/// </summary>
		private bool passwordIsFocused;

		/// <summary>
		/// Gets or sets a value indicating whether a <see cref="Password"/> field is focused.
		/// </summary>
		public bool PasswordIsFocused
		{
			get { return passwordIsFocused; }
			set
			{
				passwordIsFocused = value;
				NotifyOfPropertyChange(() => PasswordIsFocused);
			}
		}

		#endregion

		#region Password text is focused DP

		/// <summary>
		/// Specify whether a <see cref="Password"/> field with clear text is focused.
		/// </summary>
		private bool passwordTextIsFocused;

		/// <summary>
		/// Gets or sets a value indicating whether a <see cref="Password"/> field clear text is focused.
		/// </summary>
		public bool PasswordTextIsFocused
		{
			get { return passwordTextIsFocused; }
			set
			{
				passwordTextIsFocused = value;
				NotifyOfPropertyChange(() => PasswordTextIsFocused);
			}
		}

		#endregion

		#region Status DP

		/// <summary>
		/// Status message.
		/// </summary>
		private string status;

		/// <summary>
		/// Gets or sets status message.
		/// </summary>
		public string Status
		{
			get { return status; }
			set
			{
				status = value;
				NotifyOfPropertyChange(() => Status);
			}
		}

		#endregion

		#region Show status DP

		/// <summary>
		/// Specify if status message should be visible.
		/// </summary>
		private bool showStatus;

		/// <summary>
		/// Gets a value indicating whether a status message should be visible.
		/// </summary>
		public bool ShowStatus
		{
			get { return showStatus; }
			private set
			{
				showStatus = value;
				NotifyOfPropertyChange(() => ShowStatus);
			}
		}

		#endregion

		#region Show characters DP

		/// <summary>
		/// Specify if status message should be visible.
		/// </summary>
		private bool showCharacters;

		/// <summary>
		/// Gets or sets a value indicating whether a password characters should be visible to user.
		/// </summary>
		public bool ShowCharacters
		{
			get { return showCharacters; }
			set
			{
				showCharacters = value;
				NotifyOfPropertyChange(() => ShowCharacters);
			}
		}

		#endregion

		#region Remember me DP

		/// <summary>
		/// Specify if user credentials will be stored in the local storage after successful login.
		/// </summary>
		private bool rememberMe;

		/// <summary>
		/// Gets or sets a value indicating whether user credentials should be stored in the local storage.
		/// </summary>
		public bool RememberMe
		{
			get { return rememberMe; }
			set
			{
				rememberMe = value;
				NotifyOfPropertyChange(() => RememberMe);
			}
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Called when activating.
		/// </summary>
		protected override void OnActivate()
		{
			base.OnActivate();

			if (IsolatedStorageSettings.ApplicationSettings.Contains("Login_ShowCharacters"))
			{
				ShowCharacters = (bool)IsolatedStorageSettings.ApplicationSettings["Login_ShowCharacters"];
			}

			if (IsolatedStorageSettings.ApplicationSettings.Contains("Login_RememberMe"))
			{
				RememberMe = (bool) IsolatedStorageSettings.ApplicationSettings["Login_RememberMe"];
				
				if (RememberMe)
				{
					Username = (string)IsolatedStorageSettings.ApplicationSettings["Login_Username"];
					
					if (ShowCharacters)
					{
						PasswordTextIsFocused = true;
					}
					else
					{
						PasswordIsFocused = true;
					}
				}
			}
		}

		/// <summary>
		/// Called when deactivating.
		/// </summary>
		/// <param name="close">Indicates whether this instance will be closed.</param>
		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
			IsolatedStorageSettings.ApplicationSettings["Login_ShowCharacters"] = ShowCharacters;
			IsolatedStorageSettings.ApplicationSettings["Login_RememberMe"] = RememberMe;

			if (RememberMe)
			{
				IsolatedStorageSettings.ApplicationSettings["Login_Username"] = Username;
			}
		}

		#endregion

		/// <summary>
		/// Authorize the user with specified credentials.
		/// </summary>
		/// <returns>Common co-routine results.</returns>
		[SetBusy]
		//[Async]
		[Dependencies("Username", "Password")]
		public IEnumerable<IResult> Login()
		{
			ShowStatus = true;

			var authenticateResult = new AuthenticateResult(Username, Password);
			yield return authenticateResult;

			if (!authenticateResult.Succeeded)
			{
				Status = authenticateResult.Status;// AuthenticationFailed;
				
				if (ShowCharacters)
				{
					PasswordTextIsFocused = true;
				}
				else
				{
					PasswordIsFocused = true;
				}

				yield break;
			}

			UserCredentials.Current = authenticateResult.Credentials;
			EventAggregator.Publish(new LoggedInMessage(UserCredentials.Current));
			ClearForm();
		}

		/// <summary>
		/// Check if the credentials meets the security requirements.
		/// </summary>
		/// <returns><c>true</c> if the username and password meets the security requirements.</returns>
		public bool CanLogin
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Username)
				       && !IsBusy;
				//			       && !string.IsNullOrWhiteSpace(Password);
				//			       && Username.Length >= MinimumUsernameLength
				//			       && Password.Length >= MinimumPasswordLength;
			}
		}

		#endregion

		#region Implementation of ICanBeBusy

		/// <summary>
		/// Indicate whether view-model is busy by some background operation.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether view-model is busy by some background operation.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
				NotifyOfPropertyChange(() => CanLogin);
			}
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(LoggedOutMessage message)
		{
			ClearForm();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Clear form data.
		/// </summary>
		private void ClearForm()
		{
			ShowStatus = false;
			UsernameIsFocused = true;
			Username = string.Empty;
			Password = string.Empty;
		}

		#endregion
	}
}