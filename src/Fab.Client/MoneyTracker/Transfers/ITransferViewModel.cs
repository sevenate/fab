// <copyright file="ITransferViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-06-19" />
// <summary>General transfer view model interface..</summary>

using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Transfers
{
	/// <summary>
	/// General transfer view model interface..
	/// </summary>
	public interface ITransferViewModel
	{
		/// <summary>
		/// Open specific transfer to edit.
		/// </summary>
		/// <param name="transfer">Transfer to edit.</param>
		/// <param name="fromAccountId">Account ID, the source of the transfer founds.</param>
		void Edit(TransferDTO transfer, int fromAccountId);
	}
}