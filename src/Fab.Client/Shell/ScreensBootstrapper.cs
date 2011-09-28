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
using Fab.Client.Framework.Controls;
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

			ConventionManager.AddElementConvention<WatermarkedTextbox>(WatermarkedTextbox.TextProperty, "Text", "TextChanged");

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
			}
			else
			{
				// set focus to the Silverlight plug-in when launched in browser
				System.Windows.Browser.HtmlPage.Plugin.Focus();
			}
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
																	"You have to close application window manually, since the application does not have the Elevated Permissions (required to close its main window by himself).",
																	"Notification");
																e.Cancel = false;
															}
			                   		               		}
			                   		               	});
			                   	});
		}
	}
}