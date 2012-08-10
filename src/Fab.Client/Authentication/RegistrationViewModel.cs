//------------------------------------------------------------
// <copyright file="RegistrationViewModel.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Framework.Filters;
using Fab.Client.Localization;
using Fab.Core.Framework;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// View model for registration screen.
	/// </summary>
	[Export(typeof(RegistrationViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class RegistrationViewModel : LocalizableScreen, ICanBeBusy, IHandle<LoggedOutMessage>
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

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistrationViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Events exchange entry point.</param>
		[ImportingConstructor]
		public RegistrationViewModel(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return "Register"; }
		}

		#endregion

		#region Username

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
				NotifyOfPropertyChange(() => UsernameIsOk);
			}
		}

		#endregion

		#region Password

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
				NotifyOfPropertyChange(() => PasswordIsOk);
				NotifyOfPropertyChange(() => Password2IsOk);
			}
		}

		#endregion

		#region Password confirmation

		/// <summary>
		/// User password confirmation.
		/// </summary>
		private string passwordConfirmation;

		/// <summary>
		/// Gets or sets user password confirmation.
		/// </summary>
		public string PasswordConfirmation
		{
			get { return passwordConfirmation; }
			set
			{
				passwordConfirmation = value;
				NotifyOfPropertyChange(() => PasswordConfirmation);
				NotifyOfPropertyChange(() => PasswordIsOk);
				NotifyOfPropertyChange(() => Password2IsOk);
			}
		}

		#endregion

		#region Status

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

		#region Show status

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

		#region Agree to terms

		/// <summary>
		/// Specify if user is "agree to terms"
		/// </summary>
		private bool agreeToTerms;

		/// <summary>
		/// Gets a value indicating whether a user "agree to terms".
		/// </summary>
		public bool AgreeToTerms
		{
			get { return agreeToTerms; }
			set
			{
				agreeToTerms = value;
				NotifyOfPropertyChange(() => AgreeToTerms);
			}
		}

		#endregion

		#region Username is focused

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

		/// <summary>
		/// Authorize the user with specified credentials.
		/// </summary>
		/// <returns>Common co-routine results.</returns>
		[SetBusy]
		[Dependencies("Username", "Password", "PasswordConfirmation", "AgreeToTerms")]
		public IEnumerable<IResult> Register()
		{
			ShowStatus = true;

			var registerationResult = new RegisterationResult(Username, Password);
			yield return registerationResult;

			if (!registerationResult.Succeeded)
			{
				Status = registerationResult.Status;
				UsernameIsFocused = true;
				yield break;
			}

			//Registration ends with login
			UserCredentials.Current = registerationResult.Credentials;
			EventAggregator.Publish(new LoggedInMessage(UserCredentials.Current));
			ClearForm();
		}

		/// <summary>
		/// Check if the credentials meets the security requirements.
		/// </summary>
		/// <returns><c>true</c> if the username and password meets the security requirements.</returns>
		public bool CanRegister()
		{
			return AgreeToTerms
				   && UsernameIsOk
				   && PasswordIsOk
				   && Password2IsOk;
		}

		public bool UsernameIsOk
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Username)
					&& Username.Length >= MinimumUsernameLength;
			}
		}

		public bool PasswordIsOk
		{
			get
			{
				return !string.IsNullOrWhiteSpace(Password)
				       && Password.Length >= MinimumPasswordLength;
			}
		}

		public bool Password2IsOk
		{
			get
			{
				return !string.IsNullOrWhiteSpace(PasswordConfirmation)
					&& PasswordConfirmation.Length >= MinimumPasswordLength
					&& Password == PasswordConfirmation;
			}
		}

		/// <summary>
		/// Show window with "Terms of Use" agreement.
		/// </summary>
		public void Terms()
		{
			var assemblyName = Assembly.GetExecutingAssembly().GetAssemblyName(); // "SilverFAB"

			// Try to find culture specific Terms text first and fallback to default one in "not found" case
			var resource = Application.GetResourceStream(new Uri(string.Format(@"/{0};component/Resources/Terms.{1}.txt", assemblyName, Translator.CurrentCulture.TwoLetterISOLanguageName), UriKind.Relative)) ??
			               Application.GetResourceStream(new Uri(string.Format(@"/{0};component/Resources/Terms.txt", assemblyName), UriKind.Relative));

			var streamReader = new StreamReader(resource.Stream);
			string termsText = streamReader.ReadToEnd();

			var termsViewModel = IoC.Get<TermsViewModel>();
			termsViewModel.DisplayName = Resources.Strings.TermsView_Title;
			termsViewModel.Text = termsText;

			var windowManager = IoC.Get<IWindowManager>();
			// It is required to "manually" pass style here so that Caliburn could apply it to newly created ChildWindow
			windowManager.ShowDialog(termsViewModel, settings: new Dictionary<string, object>
			                                                   	{
			                                                   		{"Style", Application.Current.Resources["ChildWindowStyle"]}
			                                                   	});
		}

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
			AgreeToTerms = false;
			UsernameIsFocused = true;
			Username = string.Empty;
			Password = string.Empty;
			PasswordConfirmation = string.Empty;
		}

		#endregion
	}
}