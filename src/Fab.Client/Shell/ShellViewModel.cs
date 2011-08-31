using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.Shell.Async;

namespace Fab.Client.Shell
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IModule>.Collection.OneActive, IShell, IHandle<OpenDialogMessage>, IHandle<OpenMessageBoxMessage>, IHandle<ServiceErrorMessage>
	{
		#region Constants

		private const string CopyrightTemplate = "Copyright © 2009-{0} nReez Software. All rights reserved.";

		#endregion

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

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
			LoginScreen = modules.OfType<ILoginViewModel>().Single();

			// exclude login module from other modules
			Items.AddRange(modules.OrderBy(module => module.Name)
								  .Except(Enumerable.Repeat<IModule>(LoginScreen, 1)));
			PersonalCorner = corner;
			AsyncProgressIndicator = asyncProgressIndicator;
			CloseStrategy = new ApplicationCloseStrategy();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets current application version.
		/// </summary>
		public string Version
		{
			get { return AssemblyExtensions.AppVersion; }
		}

		/// <summary>
		/// Gets application main window size.
		/// </summary>
		public string WindowSize
		{
			get { return Application.Current.MainWindow.Width + " x " + Application.Current.MainWindow.Height; }
		}

		/// <summary>
		/// Gets copyright information.
		/// </summary>
		public string Copyright
		{
			get { return string.Format(CopyrightTemplate, DateTime.Now.Year); }
		}

		/// <summary>
		/// Gets start login screen model for not authenticate users.
		/// </summary>
		public ILoginViewModel LoginScreen { get; private set; }

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
			ActivateItem(LoginScreen);
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
			this.CloseItem(LoginScreen);
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
			ActivateItem(LoginScreen);
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
			//TODO: show desktop "toast" notification here (if possible)
			Dialogs.ShowMessageBox(message.Error.ToString(), "Service Error");
		}

		#endregion

		//		public override void CanClose(Action<bool> callback)
		//		{
		//			base.CanClose(callback);
		//			ActiveItem.CanClose(callback);
		//		}
	}
}