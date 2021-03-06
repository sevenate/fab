﻿//------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyTracker.Accounts.AssetTypes;
using Fab.Client.Shell.Async;

namespace Fab.Client.Shell
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IModule>.Collection.OneActive,
								  IShell,
								  IHandle<OpenDialogMessage>,
								  IHandle<OpenMessageBoxMessage>,
								  IHandle<ServiceErrorMessage>,
								  IHandle<ApplicationErrorMessage>
	{
		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		/// <summary>
		/// Assets repository.
		/// </summary>
		private readonly IAssetTypesRepository assetTypes = IoC.Get<IAssetTypesRepository>();

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="ShellViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Events exchange entry point.</param>
		[ImportingConstructor]
		public ShellViewModel(
			IEventAggregator eventAggregator,
			IDialogManager dialogs,
			[ImportMany]IEnumerable<IModule> modules,
			PersonalCornerViewModel corner,
			AsyncProgressIndicatorViewModel asyncProgressIndicator)
		{
			if (eventAggregator == null)
			{
				throw new ArgumentNullException("eventAggregator");
			}

			if (dialogs == null)
			{
				throw new ArgumentNullException("dialogs");
			}

			if (modules == null)
			{
				throw new ArgumentNullException("modules");
			}

			if (corner == null)
			{
				throw new ArgumentNullException("corner");
			}

			if (asyncProgressIndicator == null)
			{
				throw new ArgumentNullException("asyncProgressIndicator");
			}

			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);

			Dialogs = dialogs;

			// find login module
			StartScreen = modules.OfType<IStartViewModel>().Single();

			// exclude login module from other modules
			Items.AddRange(modules.OrderBy(module => module.Order)
								  .Except(Enumerable.Repeat<IModule>(StartScreen, 1)));
			PersonalCorner = corner;
			AsyncProgressIndicator = asyncProgressIndicator;
			CloseStrategy = new ApplicationCloseStrategy();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets start screen view model for not authenticate users.
		/// </summary>
		public IStartViewModel StartScreen { get; private set; }

		/// <summary>
		/// Gets personal corner view model that allow user to logout.
		/// </summary>
		public PersonalCornerViewModel PersonalCorner { get; private set; }

		/// <summary>
		/// Gets async progress indicator view model.
		/// </summary>
		public AsyncProgressIndicatorViewModel AsyncProgressIndicator { get; private set; }

		/// <summary>
		/// Indicating whether user has been authenticated in the system.
		/// </summary>
		private bool isAuthenticated;

		/// <summary>
		/// Gets a value indicating whether user has been authenticated in the system.
		/// </summary>
		public bool IsAuthenticated
		{
			get { return isAuthenticated; }
			private set
			{
				if (isAuthenticated != value)
				{
					isAuthenticated = value;
					NotifyOfPropertyChange(() => IsAuthenticated);
				}
			}
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Called when initializing.
		/// </summary>
		protected override void OnInitialize()
		{
			ActivateItem(StartScreen);
		}

		#endregion

		#region Implementation of IShell

		public IDialogManager Dialogs { get; private set; }

		#endregion

		#region Implementation of IHandle<in LoggedInMessage>

		/// <summary>
		/// Handles the <see cref="LoggedInMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="LoggedInMessage"/>.</param>
		public void Handle(LoggedInMessage message)
		{
			this.CloseItem(StartScreen);
			ActivateItem(Items.First());
			IsAuthenticated = true;
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the <see cref="LoggedOutMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="LoggedOutMessage"/>.</param>
		public void Handle(LoggedOutMessage message)
		{
			IsAuthenticated = false;
			ActivateItem(StartScreen);
		}

		#endregion

		#region Implementation of IHandle<in OpenDialogMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(OpenDialogMessage message)
		{
			Dialogs.ShowDialog(message.Dialog);
		}

		#endregion

		#region Implementation of IHandle<in OpenMessageBoxMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(OpenMessageBoxMessage message)
		{
			Dialogs.ShowMessageBox(message.Message, message.Title, message.Options, message.Callback);
		}

		#endregion

		#region Implementation of IHandle<in ServiceErrorMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(ServiceErrorMessage message)
		{
			var errorDialog = IoC.Get<ErrorDialogViewModel>();
			errorDialog.Error = message.Error.ToString();
			errorDialog.DisplayName = "Service Error";

			//TODO: show desktop "toast" notification here (if possible)
			Dialogs.ShowDialog(errorDialog);
		}

		#endregion

		#region Implementation of IHandle<ApplicationErrorMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(ApplicationErrorMessage message)
		{
			var errorViewModel = IoC.Get<ErrorDialogViewModel>();
			errorViewModel.DisplayName = Resources.Strings.ErrorDialogView_Title;
			errorViewModel.Error = message.Error.ToString();

			//TODO: show "toast" notification for muted exceptions
			// when in out-of-browser mode
			var windowManager = IoC.Get<IWindowManager>();
			// It is required to "manually" pass style here so that Caliburn could apply it to newly created ChildWindow
			windowManager.ShowDialog(errorViewModel, settings: new Dictionary<string, object>
			                                                   	{
			                                                   		{"Style", Application.Current.Resources["ErrorChildWindowStyle"]}
			                                                   	});
		}

		#endregion
	}
}