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
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework.Filters;
using Fab.Client.Localization;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;
using Fab.Core.Framework;

namespace Fab.Client.MoneyTracker.Postings.Transactions
{
	/// <summary>
	/// Single transaction details view model.
	/// </summary>
	[Export(typeof(TransactionViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class TransactionViewModel : LocalizableScreen, ICanBeBusy, IPostingPanel
	{
		#region Fields

		#region Dependencies

		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

		private readonly IAccountsRepository accountsRepository = IoC.Get<IAccountsRepository>();
		private readonly ICategoriesRepository categoryRepository = IoC.Get<ICategoriesRepository>();

		#endregion

		private readonly CollectionViewSource categoriesViewSource = new CollectionViewSource();
		private AutoCompleteFilterPredicate<object> categoryFilter;
		private int? transactionId;

		private bool isDeposite;
		private DateTime operationDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
		private string price;

		private string quantity = "1";

		private string comment;
		private bool isEditMode;

		#endregion

		#region Properties

		private int AccountId { get; set; }

		public IEnumerable<CategoryDTO> CategoriesSource
		{
			set
			{
				categoriesViewSource.Source = value;

				if (!categoriesViewSource.View.IsEmpty)
				{
					categoriesViewSource.View.MoveCurrentToFirst();
				}

				NotifyOfPropertyChange(() => Categories);
			}
		}

		/// <summary>
		/// Gets accounts for specific user.
		/// </summary>
		public ICollectionView Categories
		{
			get { return categoriesViewSource.View; }
		}

		public AutoCompleteFilterPredicate<object> CategoryFilter
		{
			get { return categoryFilter; }
			set
			{
				categoryFilter = value;
				NotifyOfPropertyChange(() => CategoryFilter);
			}
		}

		private CategoryDTO previousCategory;

		public CategoryDTO CurrentCategory
		{
			get
			{
				if (Categories == null)
				{
					return null;
				}

				var currentCategory = (CategoryDTO)Categories.CurrentItem;
				return currentCategory != null && currentCategory.Id != -1
				       	? currentCategory
				       	: null;
			}
			set
			{
				if (value == null/* && !Categories.IsEmpty*/)
				{
					previousCategory = (CategoryDTO)(Categories.CurrentItem);
					Categories.MoveCurrentTo(null);
					NotifyOfPropertyChange(() => CurrentCategory);
					NotifyOfPropertyChange(() => CanSave);
					return;
				}

				foreach (var category in Categories)
				{
					if (((CategoryDTO)category).Id == value.Id)
					{
						previousCategory = (CategoryDTO)(Categories.CurrentItem);
						if (Categories.MoveCurrentTo(category))
						{
							NotifyOfPropertyChange(() => CurrentCategory);
							NotifyOfPropertyChange(() => CanSave);
						}

						return;
					}
				}

				previousCategory = (CategoryDTO)(Categories.CurrentItem);
				Categories.MoveCurrentTo(null);
				NotifyOfPropertyChange(() => CurrentCategory);
				NotifyOfPropertyChange(() => CanSave);
			}
		}

		public bool IsDeposite
		{
			get { return isDeposite; }
			set
			{
				isDeposite = value;
				NotifyOfPropertyChange(() => IsDeposite);
			}
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
		public string Price
		{
			get { return price; }
			set
			{
				if (price != value)
				{
					price = value;
					NotifyOfPropertyChange(() => Price);
					NotifyOfPropertyChange(() => Amount);
					NotifyOfPropertyChange(() => CanSave);
				}
			}
		}

		#region Price is focused DP

		/// <summary>
		/// Specify whether a <see cref="Price"/> field is focused.
		/// </summary>
		private bool priceIsFocused;

		/// <summary>
		/// Gets or sets a value indicating whether a <see cref="Price"/> field is focused.
		/// </summary>
		public bool PriceIsFocused
		{
			get { return priceIsFocused; }
			set
			{
				priceIsFocused = value;
				NotifyOfPropertyChange(() => PriceIsFocused);
			}
		}

		#endregion

		[Required]
		public string Quantity
		{
			get { return quantity; }
			set
			{
				if (quantity != value)
				{
					quantity = value;
					NotifyOfPropertyChange(() => Quantity);
					NotifyOfPropertyChange(() => Amount);
					NotifyOfPropertyChange(() => CanSave);
				}
			}
		}

		/// <summary>
		/// Gets transaction amount value.
		/// </summary>
		public decimal? Amount
		{
			get
			{
				decimal p;
				decimal q;

				if (decimal.TryParse(Price.Trim(), out p) && decimal.TryParse(Quantity.Trim(), out q))
				{
					return p * q;
				}

				return null;
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
		/// Initializes a new instance of the <see cref="TransactionViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public TransactionViewModel(IEventAggregator eventAggregator)
		{
			eventAggregator.Subscribe(this);

			categoryFilter = (search, item) =>
			                 	{
									if (string.IsNullOrEmpty(search))
									{
										return true;
									}

									if (item is CategoryDTO)
									{
										string searchToLower = search.ToLower(CultureInfo.InvariantCulture);
										return (item as CategoryDTO).Name.ToLower(CultureInfo.InvariantCulture).Contains(searchToLower);
									}

									return false;
			                 	};
			Translator.CultureChanged += (sender, args) => OperationDate = DateTime.Now;
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Gets the Display Name
		/// </summary>
		public override string DisplayName
		{
			get { return "Posting Details"; }
		}

		#endregion

		#region Methods

		public void Create(int accountId, DateTime date)
		{
			InitCategories();

			transactionId = null;
			AccountId = accountId;
			OperationDate = date;
			CurrentCategory = null;
			Price = string.Empty;
			PriceIsFocused = true;
			Quantity = "1";
			Comment = string.Empty;

			IsEditMode = false;
		}

		/// <summary>
		/// Initialize categories from repository.
		/// </summary>
		private void InitCategories()
		{
			var specific—ategoryType = IsDeposite ? CategoryType.Deposit : CategoryType.Withdrawal;

			CategoriesSource = IoC.Get<ICategoriesRepository>().Entities
								.Where(dto => dto.CategoryType == CategoryType.Common
										   || dto.CategoryType == specific—ategoryType)
								.OrderByDescending(dto => dto.Popularity);
		}

		/// <summary>
		/// Open specific deposit or withdrawal transaction to edit.
		/// </summary>
		/// <param name="transaction">Transaction to edit.</param>
		/// <param name="accountId">Current selected accountId.</param>
		public void Edit(TransactionDTO transaction, int accountId)
		{
			IsDeposite = transaction is DepositDTO;
			InitCategories();

			transactionId = transaction.Id;
			AccountId = accountId;
			OperationDate = transaction.Date.ToLocalTime();
			CurrentCategory = Categories.Cast<CategoryDTO>().Where(c => c.Id == transaction.CategoryId).SingleOrDefault();
			Price = transaction.Rate.ToString("n");
			PriceIsFocused = true;
			Quantity = transaction.Quantity.ToString("0.####");
			Comment = transaction.Comment;

			IsEditMode = true;
		}

		public bool CanSave
		{
			get { return Amount != null && CurrentCategory != null; }
		}

		[SetBusy]
		public IEnumerable<IResult> Save()
		{
			if (IsEditMode)
			{
				// To not update date if it was not changed;
				// Add time of day to updated date only.
				DateTime date = OperationDate.Kind == DateTimeKind.Unspecified
				                	? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
				                	: OperationDate;

				var request = new EditTransactionResult(
					transactionId.Value,
					UserCredentials.Current.UserId,
					AccountId,
					date.ToUniversalTime(),
					decimal.Parse(Price.Trim()),
					decimal.Parse(Quantity.Trim()),
					Comment != null ? Comment.Trim() : null,
					CurrentCategory != null
						? CurrentCategory.Id
						: (int?) null,
					IsDeposite,
					eventAggregator
					);


				yield return request;
			}
			else
			{
				// Add time of day to date.
				DateTime date = OperationDate.Kind == DateTimeKind.Unspecified
				                	? DateTime.SpecifyKind(OperationDate, DateTimeKind.Local) + DateTime.Now.TimeOfDay
				                	: OperationDate.Date + DateTime.Now.TimeOfDay;

				var request = new AddTransactionResult(
					UserCredentials.Current.UserId,
					AccountId,
					date.ToUniversalTime(),
					decimal.Parse(Price.Trim()),
					decimal.Parse(Quantity.Trim()),
					Comment != null ? Comment.Trim() : null,
					CurrentCategory != null
						? CurrentCategory.Id
						: (int?) null,
					IsDeposite,
					eventAggregator
					);

				yield return request;
			}

			accountsRepository.Download(AccountId);
			
			if (CurrentCategory != null)
			{
				categoryRepository.Download(CurrentCategory.Id);
			}

			if (previousCategory != null && CurrentCategory != null && CurrentCategory.Id != previousCategory.Id)
			{
				categoryRepository.Download(previousCategory.Id);
			}

			Cancel();
		}

		/// <summary>
		/// Cancel transaction edition and ask parent view to close this dialog.
		/// </summary>
		public void Cancel()
		{
			(Parent as IConductor).CloseItem(this);
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
	}
}