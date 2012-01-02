// <copyright file="AssetTypesUpdatedMessage.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-09-10" />

using System.Collections.Generic;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts.AssetTypes
{
	/// <summary>
	/// Send by <see cref="AssetTypesRepository"/> after <see cref="AssetTypesRepository.Download()"/> has been updated.
	/// </summary>
	public class AssetTypesUpdatedMessage
	{
		/// <summary>
		/// Gets or sets all current user asset types.
		/// </summary>
		public IEnumerable<AssetTypeDTO> AssetTypes { get; set; }
	}
}