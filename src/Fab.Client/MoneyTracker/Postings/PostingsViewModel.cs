//------------------------------------------------------------
// <copyright file="PostingsViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Postings.Actions;
using Fab.Client.MoneyTracker.Postings.Transactions;
using Fab.Client.MoneyTracker.Postings.Transfers;

namespace Fab.Client.MoneyTracker.Postings
{
	/// <summary>
	/// Postings view model.
	/// </summary>
	[Export(typeof (PostingsViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class PostingsViewModel : PostingViewModelBase
	{
		#region Properties

		/// <summary>
		/// Gets filter date period text ("startDate - endDate" or "startDate" only).
		/// </summary>
		public string Period
		{
			get
			{
				return startDate.Date == endDate.Date
				       	? startDate.Date.ToLongDateString()
				       	: startDate.Month == endDate.Month
							? startDate.Day + " - " + endDate.Day + " " + startDate.ToString("MMMM yyyy ã.") + " (" + ((endDate - startDate).TotalDays + 1) + " days)"
							: startDate.Date.ToLongDateString() + " - " + endDate.Date.ToLongDateString() + " (" + ((endDate - startDate).TotalDays + 1) + " days)";
			}
		}

		public PostingActionsViewModel PostingsActions { get; set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="PostingsViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public PostingsViewModel(TransactionViewModel transactionDetails, TransferViewModel transferDetails,
								PostingActionsViewModel postingsActions):base(transactionDetails, transferDetails)
		{
			startDate = DateTime.Now;
			endDate = DateTime.Now;
			UseStartDate = true;
			UseEndDate = true;

			PostingsActions = postingsActions;

			ActivationProcessed += (sender, args) => { IsDirty = (args.Item != PostingsActions); };
		}

		#endregion

		#region Overrides of PostingViewModelBase

		public override void CancelEdit()
		{
			//base.CancelEdit();
			ActivateItem(PostingsActions);
		}

		#endregion

		#region Overrides of ViewAware

		/// <summary>
		/// Called when an attached view's Loaded event fires.
		/// </summary>
		/// <param name="view"/>
		protected override void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);

			if (ActiveItem == null)
			{
				ActivateItem(PostingsActions);
			}
		}

		#endregion

		#region Overrides of OneActive

		/// <summary>
		/// Called when activating.
		/// </summary>
		protected override void OnActivate()
		{
			base.OnActivate();
			Update();
		}

		#endregion

		#region Overrides of PostingViewModelBase

		protected override IEnumerable<IResult> PreAction()
		{
			// Determine previous account balance.
			var balanceResult = new GetBalanceResult(UserCredentials.Current.UserId, AccountId, startDate.ToUniversalTime(),
			                                         eventAggregator);
			yield return balanceResult;

			StartBalance = balanceResult.Balance;
		}

		protected override AddTransactionRecordBaseResult CreateTransactionRecordResult(JournalDTO r)
		{
			return new AddTransactionRecordResult(r, categoriesRepository, StartBalance);
		}

		protected override void PostAction(AddTransactionRecordBaseResult result)
		{
			base.PostAction(result);

			if (result is AddTransactionRecordResult)
			{
				StartBalance = ((AddTransactionRecordResult) result).Balance;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Open dialog for creating new transaction.
		/// </summary>
		public void NewIncome()
		{
			TransactionDetails.IsDeposite = true;
			TransactionDetails.Create(AccountId, startDate.Date);
			ActivateItem(TransactionDetails);
		}

		/// <summary>
		/// Open dialog for creating new transaction.
		/// </summary>
		public void NewExpense()
		{
			TransactionDetails.IsDeposite = false;
			TransactionDetails.Create(AccountId, startDate.Date);
			ActivateItem(TransactionDetails);
		}

		/// <summary>
		/// Open dialog for creating new transfer.
		/// </summary>
		public void NewTransfer()
		{
			TransferDetails.Create(AccountId, startDate.Date);
			ActivateItem(TransferDetails);
		}

		public void SetFilterPeriod(DateTime startDate, DateTime endDate)
		{
			base.startDate = startDate;
			base.endDate = endDate;
			IsOutdated = true;
			NotifyOfPropertyChange(() => Period);
		}

		#endregion

	}
}