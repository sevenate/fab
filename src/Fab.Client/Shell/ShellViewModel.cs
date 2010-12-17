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
		private const string CopyrightTemplate = "Copyright © 2009-{0} nReez Software. All rights reserved.";

		private readonly ILoginViewModel loginViewModel;

		#region Ctors

		[ImportingConstructor]
		public ShellViewModel(IDialogManager dialogs, [ImportMany]IEnumerable<IModule> modules, ILoginViewModel loginViewModel)
		{
			Dialogs = dialogs;
			Items.AddRange(modules);
			this.loginViewModel = loginViewModel;
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
		/// Gets copyright information.
		/// </summary>
		public string Copyright
		{
			get { return string.Format(CopyrightTemplate, DateTime.Now.Year); }
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Called when initializing.
		/// </summary>
		protected override void OnInitialize()
		{
			loginViewModel.Deactivated += (sender, args) => ActivateItem(Items.First());
			Dialogs.ShowDialog(loginViewModel);
		}

		#endregion

		#region Implementation of IShell

		public IDialogManager Dialogs { get; private set; }

		#endregion
	}
}