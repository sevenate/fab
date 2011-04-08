// <copyright file="TestItem.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-04-02</date>
// </author>
// <editor name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-04-02</date>
// </editor>
// <summary>Money tracker module.</summary>

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyTracker.Accounts;
using Fab.Client.MoneyTracker.Categories;
using Fab.Client.MoneyTracker.TransactionDetails;
using Fab.Client.MoneyTracker.Transactions;
using Fab.Client.MoneyTracker.Transfers;

namespace Fab.Client.MoneyTracker
{
	/// <summary>
	/// Money tracker module.
	/// </summary>
	[Export(typeof(IModule))]
	public class MoneyModuleViewModel : Conductor<IScreen>.Collection.OneActive, IModule
	{
		#region Implementation of IModule

		public string Name
		{
			get { return "Money"; }
		}

		public void Show()
		{
			if (Parent.ActiveItem == this)
			{
				DisplayName = Name;
			}
			else
			{
				Parent.ActivateItem(this);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets <see cref="AccountsViewModel"/>.
		/// </summary>
		public object Accounts { get; private set; }

		/// <summary>
		/// Gets <see cref="CategoriesViewModel"/>.
		/// </summary>
		public object Categories { get; private set; }

		/// <summary>
		/// Gets <see cref="TransactionsViewModel"/>.
		/// </summary>
		public object Transactions { get; private set; }

		/// <summary>
		/// Gets <see cref="TransactionDetailsViewModel"/>.
		/// </summary>
		public object AddNew { get; private set; }

		/// <summary>
		/// Gets <see cref="TransferViewModel"/>.
		/// </summary>
		public object Transfer { get; private set; }

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="MoneyModuleViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public MoneyModuleViewModel(
			AccountsViewModel accountsVM,
			CategoriesViewModel categoriesVM,
			ITransactionsViewModel transactionsVM,
			ITransactionDetailsViewModel transactionDetailsVM,
			ITransferViewModel transferViewModel)
		{
			Accounts = accountsVM;
			Categories = categoriesVM;
			Transactions = transactionsVM;
			AddNew = transactionDetailsVM;
			Transfer = transferViewModel;
		}

		#endregion
	}
}