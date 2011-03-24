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

namespace Fab.Client.MoneyTracker.Transfers
{
	/// <summary>
	/// Transfer view model.
	/// </summary>
	[Export(typeof(ITransferViewModel))]
	public class TransferViewModel : DocumentBase, ITransferViewModel
	{
		private int? transactionId;

		private DateTime operationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);

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

		private string comment;

		public string Comment
		{
			get { return comment; }
			set
			{
				comment = value;
				NotifyOfPropertyChange(() => Comment);
			}
		}

		private string amount;

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

		public bool CanSave
		{
			get
			{
				//TODO: add CanSave guard.
				return true;
			}
		}

		/// <summary>
		/// Gets or sets <see cref="IAccountsViewModel"/>.
		/// </summary>
		private IAccountsViewModel accountsVM;

		private readonly CollectionViewSource accounts1CollectionViewSource = new CollectionViewSource();
		private readonly CollectionViewSource accounts2CollectionViewSource = new CollectionViewSource();

		public ICollectionView Accounts1
		{
			get
			{
				return accounts1CollectionViewSource.View;
			}
		}

		public ICollectionView Accounts2
		{
			get
			{
				return accounts2CollectionViewSource.View;
			}
		}

		private bool isEditMode;

		public bool IsEditMode
		{
			get { return isEditMode; }
			set
			{
				isEditMode = value;
				NotifyOfPropertyChange(() => IsEditMode);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransferViewModel"/> class.
		/// </summary>
		/// <param name="accountsVM">Accounts view model.</param>
		[ImportingConstructor]
		public TransferViewModel(IAccountsViewModel accountsVM)
		{
			this.accountsVM = accountsVM;

			var accounts1 = new BindableCollection<AccountDTO>();
			accounts1CollectionViewSource.Source = accounts1;

			this.accountsVM.Reloaded += (sender, args) =>
			{
				accounts1.Clear();

				foreach (var account in this.accountsVM.Accounts)
				{
					accounts1.Add(account as AccountDTO);
				}

				if (!accounts1CollectionViewSource.View.IsEmpty)
				{
					accounts1CollectionViewSource.View.MoveCurrentToFirst();
				}
			};

			var accounts2 = new BindableCollection<AccountDTO>();
			accounts2CollectionViewSource.Source = accounts2;

			this.accountsVM.Reloaded += (sender, args) =>
			{
				accounts2.Clear();

				foreach (var account in this.accountsVM.Accounts)
				{
					accounts2.Add(account as AccountDTO);
				}

				if (!accounts2CollectionViewSource.View.IsEmpty)
				{
					accounts2CollectionViewSource.View.MoveCurrentToFirst();
				}
			};
		}

		public void Clear()
		{
			transactionId = null;
			IsEditMode = false;
			Accounts1.MoveCurrentToFirst();
			Accounts2.MoveCurrentToFirst();
			OperationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
			Amount = string.Empty;
			Comment = string.Empty;
		}

		public IEnumerable<IResult> Save()
		{
			yield return Loader.Show("Saving...");

			if (IsEditMode)
			{
				// To not update date if it was not changed;
				// Add time of day to updated date only.
				DateTime date = OperationDate.Kind == DateTimeKind.Unspecified
										? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
										: OperationDate;

				var request = new UpdateTransferResult(
									transactionId.Value,
									UserCredentials.Current.UserId,
									((AccountDTO)Accounts1.CurrentItem).Id,
									((AccountDTO)Accounts2.CurrentItem).Id,
									date.ToUniversalTime(),
									decimal.Parse(Amount.Trim()),
									Comment != null ? Comment.Trim() : null
								);

				Clear();

				yield return request;
			}
			else
			{
				// Add time of day to date.
				DateTime date = OperationDate.Kind == DateTimeKind.Unspecified
				                	? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
				                	: OperationDate.Date + DateTime.Now.TimeOfDay;

				var request = new AddTransferResult(
									UserCredentials.Current.UserId,
									((AccountDTO)Accounts1.CurrentItem).Id,
									((AccountDTO)Accounts2.CurrentItem).Id,
									date.ToUniversalTime(),
									decimal.Parse(Amount.Trim()),
									Comment != null ? Comment.Trim() : null
								);

				yield return request;
			}

			yield return Loader.Hide();
		}

		/// <summary>
		/// Open specific transfer to edit.
		/// </summary>
		/// <param name="transfer">Transfer to edit.</param>
		/// <param name="fromAccountId">Account ID, the source of the transfer founds.</param>
		public void Edit(TransferDTO transfer, int fromAccountId)
		{
			transactionId = transfer.Id;

			// Todo: refactor this method!
			var accountsSource1 = accounts1CollectionViewSource.Source as BindableCollection<AccountDTO>;

			if (accountsSource1 != null)
			{
				var selectedAccount1 = accountsSource1.Where(a => a.Id == fromAccountId).Single();
				Accounts1.MoveCurrentTo(selectedAccount1);
			}

			var accountsSource2 = accounts2CollectionViewSource.Source as BindableCollection<AccountDTO>;

			if (accountsSource2 != null)
			{
				var selectedAccount2 = accountsSource2.Where(a => a.Id == transfer.SecondAccountId).Single();
				Accounts2.MoveCurrentTo(selectedAccount2);
			}

			OperationDate = transfer.Date.ToLocalTime();
			Amount = transfer.Amount.ToString();
			Comment = transfer.Comment;

			IsEditMode = true;
		}
	}
}