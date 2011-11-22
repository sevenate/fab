// <copyright file="LoginViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2010-11-17" />

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Framework.Filters;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// View-model for <see cref="LoginView"/> dialog.
	/// </summary>
	[Export(typeof (IModule))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class LoginViewModel : Screen, ILoginViewModel, ICanBeBusy, IHandle<LoggedOutMessage>
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
		/// Authorization in progress message.
		/// </summary>
		private const string AuthenticationInProgress = "Authenticating...";

		/// <summary>
		/// Authorization success message.
		/// </summary>
		private const string AuthenticationSuccess = "Success!";

		/// <summary>
		/// Authorization failed message.
		/// </summary>
		private const string AuthenticationFailed = "The username or password provided is incorrect.";

		/// <summary>
		/// "Long" copyright message.
		/// </summary>
		private const string LongCopyrightTemplate = "Copyright © 2009-{0} nReez Software. All rights reserved.";

		/// <summary>
		/// "Short" copyright message.
		/// </summary>
		private const string ShortCopyrightTemplate = "© {0} nReez";

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// Gets copyright information.
		/// </summary>
		public string Copyright
		{
			get { return string.Format(ShortCopyrightTemplate, DateTime.Now.Year); }
		}

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
#if DEBUG
			ShowCharacters = true;
#endif

			Status = AuthenticationInProgress;
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return "Login"; }
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

		#region Show characters DP

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

		#region Is authenticated DP

		/// <summary>
		/// Specify whether a user is logged is authenticated.
		/// </summary>
		private bool isAuthenticated;

		/// <summary>
		/// Gets a value indicating whether a user is authenticated.
		/// </summary>
		public bool IsAuthenticated
		{
			get { return isAuthenticated; }
			private set
			{
				isAuthenticated = value;
				NotifyOfPropertyChange(() => IsAuthenticated);
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
			Status = AuthenticationInProgress;
			ShowStatus = true;

			var authenticateResult = new AuthenticateResult(Username, Password);
			yield return authenticateResult;

			IsAuthenticated = authenticateResult.Succeeded;

			if (!authenticateResult.Succeeded)
			{
				Status = AuthenticationFailed;
				UsernameIsFocused = true;
				yield break;
			}

			UserCredentials.Current = authenticateResult.Credentials;
			Status = AuthenticationSuccess;

			EventAggregator.Publish(new LoggedInMessage(UserCredentials.Current));
		}

		/// <summary>
		/// Check if the credentials meets the security requirements.
		/// </summary>
		/// <returns><c>true</c> if the username and password meets the security requirements.</returns>
		public bool CanLogin()
		{
			return !string.IsNullOrWhiteSpace(Username)
			       && !string.IsNullOrWhiteSpace(Password)
			       && Username.Length >= MinimumUsernameLength
			       && Password.Length >= MinimumPasswordLength;
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

		#region Implementation of ICanBeBusy

		/// <summary>
		/// Gets or sets a value indicating weather a login view model has a long running operation in the background.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether a view model has a long running operation in the background.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
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
			ShowStatus = false;
			IsAuthenticated = false;
		}

		#endregion

		#region Implementation of IModule

		public string Name
		{
			get { return "Login"; }
		}

		public void Show()
		{
			if (Parent is IHaveActiveItem && ((IHaveActiveItem)Parent).ActiveItem == this)
			{
				DisplayName = Name;
			}
			else
			{
				((IConductor)Parent).ActivateItem(this);
			}
		}

		#endregion
	}
}