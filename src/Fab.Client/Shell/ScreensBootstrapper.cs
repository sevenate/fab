//------------------------------------------------------------
// <copyright file="ScreensBootstrapper.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;

namespace Fab.Client.Shell
{
	public class ScreensBootstrapper : Bootstrapper<IShell>
	{
		private bool actuallyClosing;
		private CompositionContainer container;
		private Window mainWindow;

		protected override void Configure()
		{
			var composablePartCatalogs = AssemblySource.Instance.Select(x => new AssemblyCatalog(x))
				.OfType<ComposablePartCatalog>();

			var aggregateCatalog = new AggregateCatalog(composablePartCatalogs);

			container = CompositionHost.Initialize(aggregateCatalog);

			var batch = new CompositionBatch();

			batch.AddExportedValue<IWindowManager>(new WindowManager());
			batch.AddExportedValue<IEventAggregator>(new EventAggregator());
			batch.AddExportedValue<Func<IMessageBox>>(() => container.GetExportedValue<IMessageBox>());
			batch.AddExportedValue<Func<IModule>>(() => container.GetExportedValue<IModule>());
			batch.AddExportedValue<Func<IAccountsRepository>>(() => container.GetExportedValue<IAccountsRepository>());
			batch.AddExportedValue<Func<ICategoriesRepository>>(() => container.GetExportedValue<ICategoriesRepository>());
			batch.AddExportedValue(container);

			container.Compose(batch);

			FilterFramework.Configure();
		}

		protected override object GetInstance(Type serviceType, string key)
		{
			var contractName = string.IsNullOrEmpty(key)
								? AttributedModelServices.GetContractName(serviceType)
								: key;

			var exportedValues = container.GetExportedValues<object>(contractName);

			if (exportedValues.Count() > 0)
			{
				return exportedValues.First();
			}

			throw new Exception(string.Format("Could not locate any instances of contract {0}.", contractName));
		}

		protected override IEnumerable<object> GetAllInstances(Type serviceType)
		{
			var contractName = AttributedModelServices.GetContractName(serviceType);
			return container.GetExportedValues<object>(contractName);
		}

		protected override void BuildUp(object instance)
		{
			container.SatisfyImportsOnce(instance);
		}

		#region Overrides of Bootstrapper<IShell>

		/// <summary>
		/// Override this to add custom behavior to execute after the application starts.
		/// </summary>
		/// <param name="sender">The sender.</param><param name="e">The args.</param>
		protected override void OnStartup(object sender, StartupEventArgs e)
		{
			base.OnStartup(sender, e);
			if (Application.IsRunningOutOfBrowser)
			{
				mainWindow = Application.MainWindow;
				mainWindow.Closing += OnMainWindowClosing;
				Application.CheckAndDownloadUpdateCompleted += OnCheckAndDownloadUpdateCompleted;
				Application.CheckAndDownloadUpdateAsync();
			}
			else
			{
				// set focus to the Silverlight plug-in when launched in browser
				System.Windows.Browser.HtmlPage.Plugin.Focus();
			}
		}

		/// <summary>
		/// Override this to add custom behavior for unhandled exceptions.
		/// </summary>
		/// <param name="sender">The sender.</param><param name="e">The event args.</param>
		protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			base.OnUnhandledException(sender, e);
			var eventAggregator = IoC.Get<IEventAggregator>();

			eventAggregator.Publish(new ApplicationErrorMessage
			{
				Error = e.ExceptionObject
			});

			//TODO: consider "Cancel to reload page" behavior instead of just "handled"
			e.Handled = true;
		}

		#endregion

		private void OnMainWindowClosing(object sender, ClosingEventArgs e)
		{
			if (actuallyClosing)
			{
				return;
			}

			e.Cancel = true;

			Execute.OnUIThread(() =>
								{
									var shell = IoC.Get<IShell>();

									shell.CanClose(result =>
													{
														if (result)
														{
															actuallyClosing = true;

															//NOTE: to manually close application the elevated permissions required.
															if (Application.HasElevatedPermissions)
															{
																mainWindow.Close();
															}
															else
															{
																shell.Dialogs.ShowMessageBox(
																	Resources.Strings.ScreensBootstrapper_Close_Notification_Message,
																	Resources.Strings.ScreensBootstrapper_Close_Notification_Title);
																e.Cancel = false;
															}
														}
													});
								});
		}

		private void OnCheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs args)
		{
			var shell = IoC.Get<IShell>();

			// http://nerddawg.blogspot.com/2009/07/silverlight-out-of-browser-apps-how.html
			if (args.UpdateAvailable)
			{
				shell.Dialogs.ShowMessageBox(Resources.Strings.ScreensBootstrapper_Update_Notification_Message,
											 Resources.Strings.ScreensBootstrapper_Update_Notification_Title);
				//MessageBox.Show("Application updated, please restart to apply changes.");
			}
			else if (args.Error != null && args.Error is PlatformNotSupportedException)
			{
				shell.Dialogs.ShowMessageBox(
					Resources.Strings.ScreensBootstrapper_Update_Notification_Error);
			}


			//else - there is no update available.
		}
	}
}