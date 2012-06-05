// <copyright file="AccountsExtensions.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-04-09" />

using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;
using Fab.Client.MoneyTracker.Accounts.Single;

namespace Fab.Client.MoneyTracker.Accounts
{
	/// <summary>
	/// Extension methods that's simplify working with accounts.
	/// </summary>
	public static class AccountsExtensions
	{
		/// <summary>
		/// Look up in <paramref name="repository"/> for the <see cref="AccountDTO"/> by the <paramref name="id"/> key.
		/// If nothing found, null will be returned.
		/// </summary>
		/// <param name="id">Account unique ID.</param>
		/// <param name="repository">Repository to lookup in.</param>
		/// <returns>Found account instance or null otherwise.</returns>
		public static AccountDTO LookupIn(this int? id, IAccountsRepository repository)
		{
			return id.HasValue
			       	? repository.ByKey(id.Value)
			       	: null;
		}

		/// <summary>
		/// Convert <see cref="AccountDTO" /> into <see cref="AccountViewModel" />.
		/// </summary>
		/// <param name="accountDTO">Source object with data.</param>
		/// <returns>Destination object with mapped data.</returns>
		/// <remarks>
		/// TODO: find a way to use "auto mapper" for SL4 instead of this method
		/// </remarks>
		public static AccountViewModel Map(this AccountDTO accountDTO)
		{
			var accountViewModel = IoC.Get<AccountViewModel>();

			accountViewModel.AssetTypeId = accountDTO.AssetTypeId;
			accountViewModel.Balance = accountDTO.Balance;
			accountViewModel.Created = accountDTO.Created;
			accountViewModel.FirstPostingDate = accountDTO.FirstPostingDate;
			accountViewModel.Id = accountDTO.Id;
			accountViewModel.LastPostingDate = accountDTO.LastPostingDate;
			accountViewModel.Name = accountDTO.Name;
			accountViewModel.PostingsCount = accountDTO.PostingsCount;

			return accountViewModel;
		}
	}
}