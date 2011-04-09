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
	[Export(typeof (ITransferViewModel))]
	public class TransferViewModel : DocumentBase, ITransferViewModel
	{
		private string amount;
		private string comment;
		private bool isEditMode;
		private DateTime operationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
		private int? transactionId;

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

		public string Comment
		{
			get { return comment; }
			set
			{
				comment = value;
				NotifyOfPropertyChange(() => Comment);
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
				NotifyOfPropertyChange<bool>(() => CanSave);
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

		public bool IsEditMode
		{
			get { return isEditMode; }
			set
			{
				isEditMode = value;
				NotifyOfPropertyChange(() => IsEditMode);
			}
		}

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="TransferViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public TransferViewModel(IEventAggregator eventAggregator, IAccountsRepository accountsRepository)
		{
			this.accountsRepository = accountsRepository;
			accounts1CollectionViewSource.Source = accounts1;
			accounts2CollectionViewSource.Source = accounts2;
			eventAggregator.Subscribe(this);
		}

		#endregion

		#region Accounts

		private IAccountsRepository accountsRepository;
		private readonly BindableCollection<AccountDTO> accounts1 = new BindableCollection<AccountDTO>();
		private readonly CollectionViewSource accounts1CollectionViewSource = new CollectionViewSource();
		private readonly BindableCollection<AccountDTO> accounts2 = new BindableCollection<AccountDTO>();
		private readonly CollectionViewSource accounts2CollectionViewSource = new CollectionViewSource();

		public ICollectionView Accounts1
		{
			get { return accounts1CollectionViewSource.View; }
		}

		public ICollectionView Accounts2
		{
			get { return accounts2CollectionViewSource.View; }
		}

		#endregion

		#region ITransferViewModel Members

		/// <summary>
		/// Open specific transfer to edit.
		/// </summary>
		/// <param name="transfer">Transfer to edit.</param>
		/// <param name="fromAccountId">Account ID, the source of the transfer founds.</param>
		public void Edit(TransferDTO transfer, int fromAccountId)
		{
			transactionId = transfer.Id;

			// Todo: refactor this method - here should be always only "one" first account
			var selectedAccount1 = accountsRepository.ByKey(fromAccountId);
			Accounts1.MoveCurrentTo(selectedAccount1);

			if (transfer.SecondAccountId.HasValue)
			{
				var selectedAccount2 = accountsRepository.ByKey(transfer.SecondAccountId.Value);
				Accounts2.MoveCurrentTo(selectedAccount2);
			}

			OperationDate = transfer.Date.ToLocalTime();
			Amount = transfer.Amount.ToString();
			Comment = transfer.Comment;

			IsEditMode = true;
		}

		#endregion

		#region Implementation of IHandle<in AccountsUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(AccountsUpdatedMessage message)
		{
			if (message.Error == null)
			{
				accounts1.Clear();
				accounts1.AddRange(message.Accounts);

				if (!accounts1CollectionViewSource.View.IsEmpty)
				{
					accounts1CollectionViewSource.View.MoveCurrentToFirst();
				}

				accounts2.Clear();
				accounts2.AddRange(message.Accounts);

				if (!accounts2CollectionViewSource.View.IsEmpty)
				{
					accounts2CollectionViewSource.View.MoveCurrentToFirst();
				}
			}
			else
			{
				//TODO: show error dialog here
			}
		}

		#endregion

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
				var date = OperationDate.Kind == DateTimeKind.Unspecified
				           	? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
				           	: OperationDate;

				var request = new UpdateTransferResult(
					transactionId.Value,
					UserCredentials.Current.UserId,
					((AccountDTO) Accounts1.CurrentItem).Id,
					((AccountDTO) Accounts2.CurrentItem).Id,
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
				var date = OperationDate.Kind == DateTimeKind.Unspecified
				           	? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
				           	: OperationDate.Date + DateTime.Now.TimeOfDay;

				var request = new AddTransferResult(
					UserCredentials.Current.UserId,
					((AccountDTO) Accounts1.CurrentItem).Id,
					((AccountDTO) Accounts2.CurrentItem).Id,
					date.ToUniversalTime(),
					decimal.Parse(Amount.Trim()),
					Comment != null ? Comment.Trim() : null
					);

				yield return request;
			}

			yield return Loader.Hide();
		}
	}
}