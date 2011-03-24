using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;

namespace Fab.Client.Shell
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IModule>.Collection.OneActive, IShell
	{
		#region Constants

		private const string CopyrightTemplate = "Copyright © 2009-{0} nReez Software. All rights reserved.";

		#endregion

		#region Fields

		private readonly ILoginViewModel loginViewModel;

		#endregion

		#region Ctors

		[ImportingConstructor]
		public ShellViewModel(IDialogManager dialogs, [ImportMany]IEnumerable<IModule> modules, ILoginViewModel loginViewModel, IEventAggregator eventAggregator)
		{
			Dialogs = dialogs;
			Items.AddRange(modules);
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
//			loginViewModel.Deactivated += (sender, args) =>
//			                              {
//				                              ActivateItem(Items.First());
//											  NotifyOfPropertyChange(() => PersonalCorner);
//			                              };
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
			//TODO: clear all collection with accounts, categories etc.
			ActivateItem(null);
			NotifyOfPropertyChange(() => PersonalCorner);
			Dialogs.ShowDialog(loginViewModel);
		}

		#endregion
	}
}