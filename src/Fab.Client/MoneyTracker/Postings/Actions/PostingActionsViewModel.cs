// <copyright file="PostingsActionViewModel.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-08-16" />

using System.ComponentModel.Composition;
using Fab.Client.Localization;

namespace Fab.Client.MoneyTracker.Postings.Actions
{
	/// <summary>
	/// View model for <see cref="PostingActionsView"/>.
	/// </summary>
	[Export(typeof(PostingActionsViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class PostingActionsViewModel : LocalizableScreen, IPostingPanel
	{
		/// <summary>
		/// Open dialog for creating new income transaction.
		/// </summary>
		public void NewIncome()
		{
			((PostingsViewModel)Parent).NewIncome();
		}

		/// <summary>
		/// Open dialog for creating new expense transaction.
		/// </summary>
		public void NewExpense()
		{
			((PostingsViewModel)Parent).NewExpense();
		}

		/// <summary>
		/// Open dialog for creating new transfer.
		/// </summary>
		public void NewTransfer()
		{
			((PostingsViewModel)Parent).NewTransfer();
		}
	}
}