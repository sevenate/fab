using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Controls;
using Fab.Client.Framework;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;
using Fab.Client.MoneyTracker.TransactionDetails;
using Fab.Client.MoneyTracker.Transactions;
using Fab.Client.MoneyTracker.Transfers;

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
			batch.AddExportedValue<Func<IAccountsViewModel>>(() => container.GetExportedValue<IAccountsViewModel>());
			batch.AddExportedValue<Func<ICategoriesViewModel>>(() => container.GetExportedValue<ICategoriesViewModel>());
			batch.AddExportedValue<Func<ITransactionDetailsViewModel>>(() => container.GetExportedValue<ITransactionDetailsViewModel>());
			batch.AddExportedValue<Func<ITransactionsViewModel>>(() => container.GetExportedValue<ITransactionsViewModel>());
			batch.AddExportedValue<Func<ITransferViewModel>>(() => container.GetExportedValue<ITransferViewModel>());
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

		protected override void DisplayRootView()
		{
			base.DisplayRootView();
			
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
																//TODO: display "close application" dialog here.
																e.Cancel = false;
															}
			                   		               		}
			                   		               	});
			                   	});
		}
	}
}