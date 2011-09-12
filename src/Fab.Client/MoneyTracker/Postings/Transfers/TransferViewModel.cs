// <copyright file="TransferViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-06-19" />
// <summary>Transfer view model.</summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;

namespace Fab.Client.MoneyTracker.Postings.Transfers
{
	/// <summary>
	/// Transfer view model.
	/// </summary>
	[Export(typeof(TransferViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class TransferViewModel : Screen, IPostingPanel
	{
		#region Fields

		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();
		private readonly CollectionViewSource targetAccountsViewSource = new CollectionViewSource();
		private int? transactionId;
		private DateTime operationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
		private string amount;
		private string comment;
		private bool isEditMode;

		#endregion

		#region Properties

		private AccountDTO sourceAccount;

		private AccountDTO SourceAccount
		{
			get { return sourceAccount; }
			set
			{
				sourceAccount = value;
					
				// Exclude "source" account from the "target" accounts list
				// and leave only accounts with the same AssetType like in the source
				if (sourceAccount != null)
				{
					targetAccountsViewSource.Source = accountsRepository.Entities
						.Where(dto => dto.AssetTypeId == sourceAccount.AssetTypeId)
						.Except(
							Enumerable.Repeat<AccountDTO>(sourceAccount, 1))
						.ToList();
						
					if (!TargetAccounts.IsEmpty)
					{
						TargetAccounts.MoveCurrentToFirst();
					}
				}
				else
				{
					targetAccountsViewSource.Source = null;
				}
			}
		}

		public ICollectionView TargetAccounts
		{
			get { return targetAccountsViewSource.View; }
		}

		[DataType(DataType.DateTime)]
		public DateTime OperationDate
		{
			get { return operationDate; }
			set
			{
				operationDate = value;
				NotifyOfPropertyChange(() => OperationDate);
			}
		}

		[Required]
		[DataType(DataType.Currency)]
		public string Amount
		{
			get { return amount; }
			set
			{
				amount = value;
				NotifyOfPropertyChange(() => Amount);
				NotifyOfPropertyChange(() => CanSave);
			}
		}

		public string Comment
		{
			get { return comment; }
			set
			{
				comment = value;
				NotifyOfPropertyChange(() => Comment);
			}
		}

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

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="TransferViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public TransferViewModel()
		{
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return "Transfer Details"; }
		}

		#endregion

		#region Methods

		public void Create(int accountId)
		{
			DisplayName = "New Transfer";

			transactionId = null;
			SourceAccount = accountsRepository.ByKey(accountId);
			OperationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
			Amount = string.Empty;
			Comment = string.Empty;

			IsEditMode = false;
		}

		/// <summary>
		/// Open specific transfer to edit.
		/// </summary>
		/// <param name="transfer">Transfer to edit.</param>
		/// <param name="fromAccountId">Account, the source of the transfer founds.</param>
		public void Edit(TransferDTO transfer, int fromAccountId)
		{
			DisplayName = "Edit Transfer";

			transactionId = transfer.Id;
			SourceAccount = accountsRepository.ByKey(fromAccountId);

			if (transfer.SecondAccountId.HasValue)
			{
				var selectedAccount2 = accountsRepository.ByKey(transfer.SecondAccountId.Value);
				TargetAccounts.MoveCurrentTo(selectedAccount2);
			}

			OperationDate = transfer.Date.ToLocalTime();
			Amount = transfer.Amount.ToString();
			Comment = transfer.Comment;

			IsEditMode = true;
		}

		public bool CanSave
		{
			get
			{
				decimal result;

				if (decimal.TryParse(Amount.Trim(), out result))
				{
					return TargetAccounts.CurrentItem != null;
				}

				return false;
			}
		}

		public IEnumerable<IResult> Save()
		{
//			yield return Loader.Show("Saving...");

			if (IsEditMode)
			{
				// To not update date if it was not changed;
				// Add time of day to updated date only.
				var date = OperationDate.Kind == DateTimeKind.Unspecified
							? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
							: OperationDate;

				var request = new UpdateTransferResult(
					transactionId.Value,
					UserCredentials.Current.UserId,
					SourceAccount.Id,
					((AccountDTO)TargetAccounts.CurrentItem).Id,
					date.ToUniversalTime(),
					decimal.Parse(Amount.Trim()),
					Comment != null ? Comment.Trim() : null
					);

				yield return request;
			}
			else
			{
				// Add time of day to date.
				var date = OperationDate.Kind == DateTimeKind.Unspecified
							? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
							: OperationDate.Date + DateTime.Now.TimeOfDay;

				var request = new AddTransferResult(
					UserCredentials.Current.UserId,
					SourceAccount.Id,
					((AccountDTO)TargetAccounts.CurrentItem).Id,
					date.ToUniversalTime(),
					decimal.Parse(Amount.Trim()),
					Comment != null ? Comment.Trim() : null
					);

				yield return request;
			}

			accountsRepository.Download(SourceAccount.Id);
			accountsRepository.Download(((AccountDTO)TargetAccounts.CurrentItem).Id);

//			yield return Loader.Hide();

			Cancel();
		}

		/// <summary>
		/// Cancel transfer edition and ask parent view to close this dialog.
		/// </summary>
		public void Cancel()
		{
			(Parent as IConductor).CloseItem(this);
		}

		#endregion
	}
}