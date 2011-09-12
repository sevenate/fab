//------------------------------------------------------------
// <copyright file="TransferViewModel.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Authentication;
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
		private int? transactionId;
		private DateTime operationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
		private string amount;
		private string comment;
		private bool isEditMode;

		#endregion

		#region Properties

		#region Accounts

		private AccountDTO prevSourceAccont;
		private AccountDTO prevTargetAccont;
		private readonly CollectionViewSource sourceAccountsViewSource = new CollectionViewSource();
		private readonly CollectionViewSource targetAccountsViewSource = new CollectionViewSource();

		public ICollectionView SourceAccounts
		{
			get { return sourceAccountsViewSource.View; }
		}

		public ICollectionView TargetAccounts
		{
			get { return targetAccountsViewSource.View; }
		}

		private void InitSourceAccounts()
		{
			sourceAccountsViewSource.Source = accountsRepository.Entities.ToList();

			if (!SourceAccounts.IsEmpty)
			{
				SourceAccounts.MoveCurrentToFirst();
			}
		}

		private void InitTargetAccounts()
		{
			targetAccountsViewSource.Source = accountsRepository.Entities.ToList();

			if (!TargetAccounts.IsEmpty)
			{
				TargetAccounts.MoveCurrentToFirst();
			}
		}

		#endregion

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
			InitSourceAccounts();
			InitTargetAccounts();

			var newAccount = accountsRepository.ByKey(accountId);
			SourceAccounts.MoveCurrentTo(newAccount);

			prevSourceAccont = (AccountDTO)SourceAccounts.CurrentItem;
			prevTargetAccont = (AccountDTO)TargetAccounts.CurrentItem;

			OperationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
			Amount = string.Empty;
			Comment = string.Empty;

			IsEditMode = false;
		}

		/// <summary>
		/// Open specific transfer to edit.
		/// </summary>
		/// <param name="transfer">Transfer to edit.</param>
		/// <param name="firstAccountId">First account of the transfer.</param>
		public void Edit(TransferDTO transfer, int firstAccountId)
		{
			DisplayName = "Edit Transfer";
			InitSourceAccounts();
			InitTargetAccounts();

			transactionId = transfer.Id;

			var firstAccount = accountsRepository.ByKey(firstAccountId);
			AccountDTO secondAccount = null;

			if (transfer.SecondAccountId.HasValue)
			{
				secondAccount = accountsRepository.ByKey(transfer.SecondAccountId.Value);
			}

			if (transfer is IncomingTransferDTO)
			{
				Amount = transfer.Amount.ToString();

				TargetAccounts.MoveCurrentTo(firstAccount);

				if (secondAccount != null)
				{
					SourceAccounts.MoveCurrentTo(secondAccount);
				}
			}
			else if (transfer is OutgoingTransferDTO)
			{
				Amount = (-transfer.Amount).ToString();
				SourceAccounts.MoveCurrentTo(firstAccount);

				if (secondAccount != null)
				{
					TargetAccounts.MoveCurrentTo(secondAccount);
				}
			}

			prevSourceAccont = (AccountDTO)SourceAccounts.CurrentItem;
			prevTargetAccont = (AccountDTO)TargetAccounts.CurrentItem;

			OperationDate = transfer.Date.ToLocalTime();
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
					return SourceAccounts.CurrentItem != null && TargetAccounts.CurrentItem != null;
				}

				return false;
			}
		}

		public IEnumerable<IResult> Save()
		{
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
					((AccountDTO)SourceAccounts.CurrentItem).Id,
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
					((AccountDTO)SourceAccounts.CurrentItem).Id,
					((AccountDTO)TargetAccounts.CurrentItem).Id,
					date.ToUniversalTime(),
					decimal.Parse(Amount.Trim()),
					Comment != null ? Comment.Trim() : null
					);

				yield return request;
			}

			var sourceAccountId = (AccountDTO)SourceAccounts.CurrentItem;
			var targetAccountId = (AccountDTO)TargetAccounts.CurrentItem;

			var accountsToUpdate = new[]
			{
				prevSourceAccont.Id,
				prevTargetAccont.Id,
				sourceAccountId.Id,
				targetAccountId.Id,
			}
			.Distinct();

			// Download only updated accounts (up to 4 in worst case)
			foreach (var accountId in accountsToUpdate)
			{
				accountsRepository.Download(accountId);
			}

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