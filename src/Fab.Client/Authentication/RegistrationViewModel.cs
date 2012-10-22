//------------------------------------------------------------
// <copyright file="RegistrationViewModel.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Framework.Controls;
using Fab.Client.Framework.Filters;
using Fab.Client.Localization;
using Fab.Client.Resources;
using Fab.Client.Shell;
using Fab.Core;
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
		/// Delay (in milliseconds) from the last typed character in the <see cref="Username"/> field
		/// to the start of checking it for uniqueness on server.
		/// </summary>
		private const int TypingDelay = 500;

		/// <summary>
		/// Minimum allowed username length.
		/// </summary>
		private const int MinimumUsernameLength = 5;

		/// <summary>
		/// Minimum allowed password length.
		/// </summary>
#if DEBUG
		private const int MinimumPasswordLength = 5;
#else
		private const int MinimumPasswordLength = 8;
#endif

		#endregion

		#region Fields

		/// <summary>
		/// Timer for delayed username validation.
		/// </summary>
		private readonly Timer usernameValidationTimer;

		/// <summary
		/// Timer for delayed username validation.
		/// </summary>
		private readonly Timer passwordValidationTimer;

		/// <summary>
		/// User name.
		/// </summary>
		private string username;

		/// <summary>
		/// A value indicating whether username is unique.
		/// </summary>
		private bool? isUserNameUnique;

		/// <summary>
		/// <see cref="Username"/> validness state.
		/// </summary>
		private ValidationStates usernameState;

		/// <summary>
		/// User password.
		/// </summary>
		private string password;

		/// <summary>
		/// Specify if status message should be visible.
		/// </summary>
		private bool showCharacters;

		/// <summary>
		/// <see cref="Password"/> validness state.
		/// </summary>
		private ValidationStates passwordState;

		/// <summary>
		/// Status message.
		/// </summary>
		private string status;

		/// <summary>
		/// Specify if status message should be visible.
		/// </summary>
		private bool showStatus;

		/// <summary>
		/// Specify whether a <see cref="Username"/> field is focused.
		/// </summary>
		private bool usernameIsFocused;

		/// <summary>
		/// Gets or sets a value indicating weather a login view model has a long running operation in the background.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Validation error message for <see cref="Username"/>.
		/// </summary>
		private string usernameValidationResult;

		/// <summary>
		/// Validation error message for <see cref="Password"/>.
		/// </summary>
		private string passwordValidationResult;

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

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

		#region Properties

		/// <summary>
		/// Gets or sets user name.
		/// </summary>
		public string Username
		{
			get { return (username ?? string.Empty).Trim(); }
			set
			{
				if (Username != value)
				{
					username = value;
					NotifyOfPropertyChange(() => Username);

					// delay uniqueness checking for a moment for case if user will type another character
					usernameValidationTimer.Change(TimeSpan.FromMilliseconds(TypingDelay), TimeSpan.FromMilliseconds(-1));
				}
			}
		}

		public string UsernameValidationResult	
		{
			get { return usernameValidationResult; }
			set
			{
				if (value == usernameValidationResult) return;

				usernameValidationResult = value;
				NotifyOfPropertyChange(() => UsernameValidationResult);
			}
		}

		public string PasswordValidationResult
		{
			get { return passwordValidationResult; }
			set
			{
				if (value == passwordValidationResult) return;

				passwordValidationResult = value;
				NotifyOfPropertyChange(() => PasswordValidationResult);
			}
		}

		/// <summary>
		/// Gets or sets user password.
		/// </summary>
		public string Password
		{
			get { return password; }
			set
			{
				if (password != value)
				{
					password = value;

					// delay uniqueness checking for a moment for case if user will type another character
					passwordValidationTimer.Change(TimeSpan.FromMilliseconds(TypingDelay), TimeSpan.FromMilliseconds(-1));

					NotifyOfPropertyChange(() => Password);
				}
			}
		}

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

		/// <summary>
		/// Gets user name.
		/// </summary>
		public int MinPasswordLength
		{
			get { return MinimumPasswordLength; }
		}

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

		public ValidationStates UsernameState
		{
			get { return usernameState; }
			set
			{
				if (usernameState != value)
				{
					usernameState = value;
					NotifyOfPropertyChange(() => UsernameState);
				}
			}
		}

		public ValidationStates PasswordState
		{
			get { return passwordState; }
			set
			{
				if (passwordState != value)
				{
					passwordState = value;
					NotifyOfPropertyChange(() => PasswordState);
				}
			}
		}

		#endregion

		#region Implementation of ICanBeBusy

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
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public RegistrationViewModel()
		{
			ResetValidationState();

			usernameValidationTimer = new Timer(OnTimerCallback);
			passwordValidationTimer = new Timer(state => Execute.OnUIThread(ValidatePassword));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegistrationViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Events exchange entry point.</param>
		[ImportingConstructor]
		public RegistrationViewModel(IEventAggregator eventAggregator):this()
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);
			ShowCharacters = true;
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
		}

		/// <summary>
		/// Called when deactivating.
		/// </summary>
		/// <param name="close">Indicates whether this instance will be closed.</param>
		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
			IsolatedStorageSettings.ApplicationSettings["Login_ShowCharacters"] = ShowCharacters;
		}

		#endregion

		public void Generate()
		{
			Username = Guid.NewGuid().Hash36();
			ValidateUsername();
		}

		private void ResetValidationState()
		{
			UsernameState = ValidationStates.Undefined;
			UsernameValidationResult = string.Format(Strings.Registration_View_Error_Lenght, MinimumUsernameLength);

			PasswordState = ValidationStates.Undefined;
			PasswordValidationResult = string.Format(Strings.Registration_View_Error_Lenght, MinimumPasswordLength);
		}

		public void ValidateUsername()
		{
			if (string.IsNullOrWhiteSpace(Username) || Username.Length < MinimumUsernameLength)
			{
				UsernameState = ValidationStates.Error;
				UsernameValidationResult = string.Format(Strings.Registration_View_Error_Lenght, MinimumUsernameLength);
				return;
			}
			
			if (isUserNameUnique.HasValue && !isUserNameUnique.Value)
			{
				UsernameState = ValidationStates.Error;
				UsernameValidationResult = Strings.RegistrationView_Error_UsernameNotUnique;
				return;
			}

			if (!isUserNameUnique.HasValue)
			{
				UsernameState = ValidationStates.Checking;
				UsernameValidationResult = String.Empty;
				return;
			}

			UsernameState = ValidationStates.Ok;
			UsernameValidationResult = string.Empty;
		}

		public void ValidatePassword()
		{
			if (string.IsNullOrWhiteSpace(Password) || Password.Length < MinimumPasswordLength)
			{
				PasswordState = ValidationStates.Error;
				PasswordValidationResult = string.Format(Strings.Registration_View_Error_Lenght, MinimumPasswordLength);
				return;
			}

			PasswordState = ValidationStates.Ok;
			PasswordValidationResult = string.Empty;
		}

		private void OnTimerCallback(object state)
		{
			Execute.OnUIThread(() =>
				                   {
									   isUserNameUnique = null;
									   ValidateUsername();

									   if (!string.IsNullOrWhiteSpace(Username) && Username.Length >= MinimumUsernameLength)
									   {
										   var proxy = ServiceFactory.CreateRegistrationService();
										   proxy.IsLoginAvailableCompleted += (sender, args) =>
										   {
											   isUserNameUnique = args.Result;
											   Execute.OnUIThread(ValidateUsername);
										   };

										   proxy.IsLoginAvailableAsync(Username);
									   }
								   });
		}

		/// <summary>
		/// Authorize the user with specified credentials.
		/// </summary>
		/// <returns>Common co-routine results.</returns>
		[SetBusy]
		[Dependencies("Username", "Password", "PasswordConfirmation", "AgreeToTerms")]
		public IEnumerable<IResult> Register()
		{
			ShowStatus = true;

			if (!CanRegister())
			{
				ValidateUsername();
				ValidatePassword();
				yield break;
			}

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
			return UsernameState == ValidationStates.Ok && PasswordState == ValidationStates.Ok;
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
			termsViewModel.DisplayName = Strings.TermsView_Title;
			termsViewModel.Text = termsText;

			var windowManager = IoC.Get<IWindowManager>();
			// It is required to "manually" pass style here so that Caliburn could apply it to newly created ChildWindow
			windowManager.ShowDialog(termsViewModel, settings: new Dictionary<string, object>
			                                                   	{
			                                                   		{"Style", Application.Current.Resources["ChildWindowStyle"]}
			                                                   	});
		}

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
			ShowCharacters = true;
			ResetValidationState();
		}

		#endregion
	}
}