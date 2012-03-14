//------------------------------------------------------------
// <copyright file="NewAccountViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Framework.Filters;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts.AssetTypes;
using Fab.Core.Framework;

namespace Fab.Client.MoneyTracker.Accounts.Single
{
	/// <summary>
	/// View model for new account dialog.
	/// </summary>
	[Export(typeof(NewAccountViewModel))]
	public class NewAccountViewModel : LocalizableScreen, ICanBeBusy
	{
		#region Fields

		/// <summary>
		/// Accounts repository.
		/// </summary>
		private readonly IAccountsRepository accounts = IoC.Get<IAccountsRepository>();

		/// <summary>
		/// Assets repository.
		/// </summary>
		private readonly IAssetTypesRepository assetTypes = IoC.Get<IAssetTypesRepository>();

		#endregion

		#region Id

		/// <summary>
		/// Gets or sets ID for existing account (in "edit mode" only).
		/// </summary>
		public int? AccountId { get; set; }

		#endregion

		#region Account name

		/// <summary>
		/// Account name.
		/// </summary>
		private string accountName;

		/// <summary>
		/// Gets or sets account name.
		/// </summary>
		public string AccountName
		{
			get { return accountName; }
			set
			{
				if (accountName != value)
				{
					accountName = value;
					NotifyOfPropertyChange(() => AccountName);
				}
			}
		}

		#endregion

		#region Asset type

		private readonly CollectionViewSource assetsSource = new CollectionViewSource();

		public ICollectionView Assets
		{
			get { return assetsSource.View; }
		}

		private BindableCollection<AssetTypeDTO> AssetsSource { get; set; }

		#endregion

		#region Is edit mode

		/// <summary>
		/// Indicate whether view is in "edit" mode for existing account.
		/// </summary>
		private bool isEditMode;

		/// <summary>
		/// Gets or sets a value indicating whether view is in "edit" mode for existing account.
		/// </summary>
		public bool IsEditMode
		{
			get { return isEditMode; }
			set
			{
				isEditMode = value;
				NotifyOfPropertyChange(() => IsEditMode);
			}
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Creates an instance of the screen.
		/// </summary>
		[ImportingConstructor]
		public NewAccountViewModel()
		{
			AssetsSource = new BindableCollection<AssetTypeDTO>();

			AssetsSource.AddRange(assetTypes.Entities);

			assetsSource.Source = AssetsSource;

			if (AssetsSource.Count > 0)
			{
				Assets.MoveCurrentToFirst();
			}
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets the dialog title.
		/// </summary>
		public override string DisplayName
		{
			get { return IsEditMode ? Resources.Strings.NewAccountView_Edit : Resources.Strings.NewAccountView_New; }
		}

		#endregion

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

		/// <summary>
		/// Gets a value indicating whether all data for new account filled correctly.
		/// </summary>
		/// <returns><c>true</c> if the name is not empty.</returns>
		public bool CanSave
		{
			//[Dependencies] can't figure out changes of the Assets property
			//&& Assets.CurrentItem != null;
			get { return !string.IsNullOrWhiteSpace(AccountName); }
		}

		/// <summary>
		/// Create new account on server or update existing.
		/// </summary>
		[SetBusy]
		[Dependencies("AccountName")]
		public IEnumerable<IResult> Save()
		{
			if (IsEditMode)
			{
				if (AccountId.HasValue)
				{
					accounts.Update(AccountId.Value, AccountName.Trim());
				}
				else
				{
					throw new Exception("Category ID is not specified for \"Update\" operation.");
				}
			}
			else
			{
				int assetTypeId = ((AssetTypeDTO)Assets.CurrentItem).Id;
				accounts.Create(AccountName.Trim(), assetTypeId);
			}

			Close();
			yield break;
		}

		/// <summary>
		/// Cancel account creation or edition.
		/// </summary>
		public void Cancel()
		{
			Close();
		}

		#region Private Method

		/// <summary>
		/// Close dialog and empty current dialog data.
		/// </summary>
		private void Close()
		{
			(Parent as IConductor).CloseItem(this);

			AccountId = null;
			AccountName = string.Empty;
			Assets.MoveCurrentToFirst();
			
			IsEditMode = false;
		}

		#endregion
	}
}