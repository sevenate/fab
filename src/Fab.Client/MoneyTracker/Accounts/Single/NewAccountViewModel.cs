// <copyright file="NewAccountViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-09-02" />

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Framework.Filters;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts.Single
{
	/// <summary>
	/// View model for new account dialog.
	/// </summary>
	[Export(typeof(NewAccountViewModel))]
	public class NewAccountViewModel : Screen, ICanBeBusy
	{
		#region Fields

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository repository = IoC.Get<IAccountsRepository>();

		#endregion

		#region Name

		/// <summary>
		/// New account name.
		/// </summary>
		private string name;

		/// <summary>
		/// Gets or sets new account name.
		/// </summary>
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
					NotifyOfPropertyChange(() => Name);
				}
			}
		}

		#endregion

		private readonly CollectionViewSource assetsSource = new CollectionViewSource();

		public ICollectionView Assets
		{
			get { return assetsSource.View; }
		}

		private BindableCollection<AssetTypeDTO> AssetsSource { get; set; }

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		/// <summary>
		/// Creates an instance of the screen.
		/// </summary>
		[ImportingConstructor]
		public NewAccountViewModel(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			AssetsSource = new BindableCollection<AssetTypeDTO>();
			assetsSource.Source = AssetsSource;
		}

		#region Overrides of Screen

		#region Overrides of Screen

		/// <summary>
		/// Gets the dialog title.
		/// </summary>
		public override string DisplayName
		{
			get { return "Create new account"; }
		}

		#endregion

		/// <summary>
		/// Called when initializing.
		/// </summary>
		protected override void OnInitialize()
		{
			Coroutine.BeginExecute(DownloadAssetTypes().GetEnumerator());
		}

		#endregion

		private IEnumerable<IResult> DownloadAssetTypes()
		{
			var getAssetTypesResult = new GetAssetTypesResult(EventAggregator);
			yield return getAssetTypesResult;
			AssetsSource.Clear();
			AssetsSource.AddRange(getAssetTypesResult.Assets);
			Assets.MoveCurrentToFirst();
			//NotifyOfPropertyChange(() => Assets);
		}

		/// <summary>
		/// Create new account on server.
		/// </summary>
		[SetBusy]
		[Dependencies("Name")]
		public IEnumerable<IResult> Create()
		{
			int assetTypeId = ((AssetTypeDTO) Assets.CurrentItem).Id;
			repository.Create(Name.Trim(), assetTypeId);
			Cancel();
			yield break;
		}

		/// <summary>
		/// Check new account name for empty string.
		/// </summary>
		/// <returns><c>true</c> if the name is not empty.</returns>
		public bool CanCreate()
		{
			return !string.IsNullOrWhiteSpace(Name);
			//[Dependencies] can't figure out changes of the Assets property
			//&& Assets.CurrentItem != null;
		}

		/// <summary>
		/// Cancel account creation.
		/// </summary>
		public void Cancel()
		{
			Name = string.Empty;
			(Parent as IConductor).CloseItem(this);
		}

		#region Implementation of ICanBeBusy

		/// <summary>
		/// Gets or sets a value indicating weather a login view model has a long running operation in the background.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether a view model has a long running operation in the background.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
			}
		}

		#endregion
	}
}