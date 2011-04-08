// <copyright file="LoginViewModel.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-11-17" />
// <summary>View-model for <see cref="Login"/> dialog.</summary>

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
	[Export(typeof (ILoginViewModel))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class LoginViewModel : Screen, ILoginViewModel, ICanBeBusy
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
		private const string AuthorizationInProgress = "Authorizing...";

		/// <summary>
		/// Authorization success message.
		/// </summary>
		private const string AuthorizationSuccess = "Success!";

		/// <summary>
		/// Authorization failed message.
		/// </summary>
		private const string AuthorizationFailed = "The username or password provided is incorrect.";

		#endregion

		#region Fields

		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator;

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="LoginViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public LoginViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
			ShowCharacters = true;
			DisplayName = "Login";
			Username = "import";
			Password = "import";
			Status = AuthorizationInProgress;
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
		/// Gets or sets a value indicating whether a status message should be visible.
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
		[Async]
		[Dependencies("Username", "Password")]
		public IEnumerable<IResult> Login()
		{
			Status = AuthorizationInProgress;
			ShowStatus = true;

			var authenticateResult = new AuthenticateResult(Username, Password);
			yield return authenticateResult;

			IsAuthenticated = authenticateResult.Succeeded;

			if (!authenticateResult.Succeeded)
			{
				Status = AuthorizationFailed;
				UsernameIsFocused = true;
				//yield return Loader.Show(AuthorizationFailedMessage);
				yield break;
			}

			UserCredentials.Current = authenticateResult.Credentials;
			Status = AuthorizationSuccess;

			eventAggregator.Publish(new LoggedInMessage(UserCredentials.Current));

			//Parent.CloseItem(this);
		}

		/// <summary>
		/// Logout user from the system.
		/// </summary>
		[SetBusy]
		public void Logout()
		{
			//Username = string.Empty;
			//Password = string.Empty;
			ShowStatus = false;
			IsAuthenticated = false;
			UserCredentials.Current = null;
			eventAggregator.Publish(new LoggedOutMessage());
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
	}
}