// <copyright file="AccountViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-05-24" />

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.MoneyTracker.Filters;
using Fab.Client.MoneyTracker.Transactions;

namespace Fab.Client.MoneyTracker.Accounts.Single
{
	/// <summary>
	/// Account basic view model.
	/// </summary>
	[Export(typeof(AccountViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class AccountViewModel : Screen, IHandle<PostingsFilterUpdatedMessage>
	{
		#region Fields

		/// <summary>
		/// Account unique ID.
		/// </summary>
		private int id;

		/// <summary>
		/// Account name.
		/// </summary>
		private string name;

		/// <summary>
		/// Associated asset type.
		/// </summary>
		private int assetTypeId;

		/// <summary>
		/// Account creation date.
		/// </summary>
		private DateTime created;

		/// <summary>
		/// Current account balance.
		/// </summary>
		private decimal balance;

		/// <summary>
		/// Date of the first posting.
		/// </summary>
		private DateTime? firstPostingDate;

		/// <summary>
		/// Date of the last posting.
		/// </summary>
		private DateTime? lastPostingDate;

		/// <summary>
		/// Total postings count in the account.
		/// </summary>
		private decimal postingsCount;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets account unique ID.
		/// </summary>
		public int Id
		{
			get { return id; }
			set
			{
				if (id != value)
				{
					id = value;

					// pass account id to the posting view model
					PostingsVM.AccountId = id;
					
					NotifyOfPropertyChange(() => Id);
				}
			}
		}

		/// <summary>
		/// Gets or sets account name.
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

		/// <summary>
		/// Gets or sets associated asset type.
		/// </summary>
		public int AssetTypeId
		{
			get { return assetTypeId; }
			set
			{
				if (assetTypeId != value)
				{
					assetTypeId = value;
					NotifyOfPropertyChange(() => AssetTypeId);
				}
			}
		}

		/// <summary>
		/// Gets or sets account creation date.
		/// </summary>
		public DateTime Created
		{
			get { return created; }
			set
			{
				if (created != value)
				{
					created = value;
					NotifyOfPropertyChange(() => Created);
				}
			}
		}

		/// <summary>
		/// Gets or sets current account balance.
		/// </summary>
		public decimal Balance
		{
			get { return balance; }
			set
			{
				if (balance != value)
				{
					balance = value;
					NotifyOfPropertyChange(() => Balance);
				}
			}
		}

		/// <summary>
		/// Gets or sets date of the first posting.
		/// </summary>
		public DateTime? FirstPostingDate
		{
			get { return firstPostingDate; }
			set
			{
				if (firstPostingDate != value)
				{
					firstPostingDate = value;
					NotifyOfPropertyChange(() => FirstPostingDate);
				}
			}
		}

		/// <summary>
		/// Gets or sets date of the last posting.
		/// </summary>
		public DateTime? LastPostingDate
		{
			get { return lastPostingDate; }
			set
			{
				if (lastPostingDate != value)
				{
					lastPostingDate = value;
					NotifyOfPropertyChange(() => LastPostingDate);
				}
			}
		}

		/// <summary>
		/// Gets or sets total postings count in the account.
		/// </summary>
		public decimal PostingsCount
		{
			get { return postingsCount; }
			set
			{
				if (postingsCount != value)
				{
					postingsCount = value;
					NotifyOfPropertyChange(() => PostingsCount);
				}
			}
		}

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#region Child VM

		private TransactionsViewModel postingsVM;

		/// <summary>
		/// Gets or sets corresponding view model of account postings.
		/// </summary>
		public TransactionsViewModel PostingsVM
		{
			get { return postingsVM; }
			private set
			{
				postingsVM = value;
				NotifyOfPropertyChange(() => PostingsVM);
			}
		}

		#endregion

		#endregion

		#region Ctore

		/// <summary>
		/// Creates an instance of the <see cref="AccountViewModel"/> class.
		/// <param name="eventAggregator">The event aggregator to listen for the specific notifications.</param>
		/// </summary>
		[ImportingConstructor]
		public AccountViewModel(IEventAggregator eventAggregator, TransactionsViewModel postingsViewModel)
		{
			if (eventAggregator == null)
			{
				throw new ArgumentNullException("eventAggregator");
			}

			if (postingsViewModel == null)
			{
				throw new ArgumentNullException("postingsViewModel");
			}

			EventAggregator = eventAggregator;
			PostingsVM = postingsViewModel;

			//TODO: Subscription required to listen on any new/updated/deleted postings so that account could update in balance locally.
			//TODO: This should be implemented later.
			EventAggregator.Subscribe(this);
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Called when activating.
		/// </summary>
		protected override void OnActivate()
		{
			base.OnActivate();
			PostingsVM.Update();
		}

		#endregion


		#region Implementation of IHandle<in PostingsFilterUpdatedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(PostingsFilterUpdatedMessage message)
		{
			PostingsVM.SetFilterPeriod(message.Start, message.End);

			if (IsActive)
			{
				PostingsVM.Update();
			}
		}

		#endregion
	}
}