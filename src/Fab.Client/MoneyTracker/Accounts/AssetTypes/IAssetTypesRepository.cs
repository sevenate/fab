// <copyright file="IAssetTypesRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-09-10" />

using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Accounts.AssetTypes
{
	/// <summary>
	/// Specify interface of common operation with user asset types.
	/// </summary>
	public interface IAssetTypesRepository : IRepository<AssetTypeDTO, int>
	{
	}
}