//------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Fab.Managment.Framework;

namespace Fab.Managment.Shell
{
	public class AppBootstrapper : Bootstrapper<ShellViewModel>
	{
		#region Overrides of Bootstrapper<ShellViewModel>

		/// <summary>
		/// Override this to add custom behavior to execute after the application starts.
		/// </summary>
		/// <param name="sender">The sender.</param><param name="e">The args.</param>
		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			base.OnStartup(sender, e);

			// Important to accept SSL certificate from server before making any call to WCF services
			SslValidation.SetCertificatePolicy();
		}

		#endregion

		#region Overrides of Bootstrapper

		/// <summary>
		/// Override this to add custom behavior for unhandled exceptions.
		/// </summary>
		/// <param name="sender">The sender.</param><param name="e">The event args.</param>
		protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			base.OnUnhandledException(sender, e);
#if !DEBUG
			e.Handled =
				MessageBox.Show(
					e.Exception.ToString(), "Unhandled exception occur. Continue?", MessageBoxButton.YesNo, MessageBoxImage.Error)
				== MessageBoxResult.Yes;
#endif
		}

		#endregion
	}
}