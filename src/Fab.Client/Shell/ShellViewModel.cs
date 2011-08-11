using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;

namespace Fab.Client.Shell
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IModule>.Collection.OneActive, IShell, IHandle<OpenDialogMessage>, IHandle<OpenMessageBoxMessage>, IHandle<ServiceErrorMessage>
	{
		#region Constants

		private const string CopyrightTemplate = "Copyright © 2009-{0} nReez Software. All rights reserved.";

		#endregion

		#region Fields

		/// <summary>
		/// Login view model.
		/// </summary>
		private readonly ILoginViewModel loginViewModel;

		#endregion

		#region Ctors

		[ImportingConstructor]
		public ShellViewModel(IDialogManager dialogs, [ImportMany]IEnumerable<IModule> modules, ILoginViewModel loginViewModel, IEventAggregator eventAggregator)
		{
			Dialogs = dialogs;
			Items.AddRange(modules.OrderBy(module => module.Name));
			this.loginViewModel = loginViewModel;
			CloseStrategy = new ApplicationCloseStrategy();
			eventAggregator.Subscribe(this);
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
		/// Gets personal corner view that allow user to logout.
		/// </summary>
		public ILoginViewModel PersonalCorner
		{
			get { return loginViewModel.IsAuthenticated ? loginViewModel : null; }
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Called when initializing.
		/// </summary>
		protected override void OnInitialize()
		{
			Dialogs.ShowDialog(loginViewModel);
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
			Dialogs.ShowDialog(null);
			NotifyOfPropertyChange(() => PersonalCorner);
			ActivateItem(Items.First());
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the <see cref="LoggedOutMessage"/>.
		/// </summary>
		/// <param name="message">The <see cref="LoggedOutMessage"/>.</param>
		public void Handle(LoggedOutMessage message)
		{
			ActivateItem(null);
			NotifyOfPropertyChange(() => PersonalCorner);
			Dialogs.ShowDialog(loginViewModel);
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
	}
}