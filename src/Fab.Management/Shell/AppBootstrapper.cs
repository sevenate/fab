//------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Fab.Managment.Framework;

namespace Fab.Managment.Shell
{
	public class AppBootstrapper : Bootstrapper<IShell>
	{
		#region Overrides of Bootstrapper<IShell>

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

		private CompositionContainer container;

		protected override void Configure()
		{
			container =
				new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)))
					);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue(container);

			container.Compose(batch);
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
			IEnumerable<object> exports = container.GetExportedValues<object>(contract);

			if (exports.Any())
				return exports.First();

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
		}

		protected override void BuildUp(object instance)
		{
			container.SatisfyImportsOnce(instance);
		}

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