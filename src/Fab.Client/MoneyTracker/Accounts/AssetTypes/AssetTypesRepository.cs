// <copyright file="AccountsRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-09-10" />

using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;
using Fab.Client.Shell;
using Fab.Client.Shell.Async;

namespace Fab.Client.MoneyTracker.Accounts.AssetTypes
{
	/// <summary>
	/// Provide access to all user asset types.
	/// </summary>
	[Export(typeof (IAssetTypesRepository))]
	public class AssetTypesRepository : RepositoryBase<AssetTypeDTO>, IAssetTypesRepository
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetTypesRepository"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public AssetTypesRepository(IEventAggregator eventAggregator)
			: base(eventAggregator)
		{
		}

		#endregion

		#region Overrides of RepositoryBase<AccountDTO>

		/// <summary>
		/// Retrieve asset type by unique key.
		/// </summary>
		/// <param name="key">Asset type key.</param>
		/// <returns>Corresponding asset type or null if not found.</returns>
		public override AssetTypeDTO ByKey(int key)
		{
			return Entities.Where(a => a.Id == key).SingleOrDefault();
		}

		/// <summary>
		/// Download all <see cref="AssetTypeDTO"/> entities from server.
		/// </summary>
		public override void Download()
		{
			var proxy = ServiceFactory.CreateMoneyService();
			proxy.GetAllAssetTypesCompleted += (s, e) =>
			                                 {
			                                 	if (e.Error == null)
			                                 	{
			                                 		Entities.Clear();
													Entities.AddRange(e.Result);

//													foreach (var item in e.Result)
//													{
//														Entities.Add(item);
//													}

			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new AssetTypesUpdatedMessage
			                                 		                                                 {
																										 AssetTypes = Entities
			                                 		                                                 }));
			                                 	}
			                                 	else
			                                 	{
			                                 		Execute.OnUIThread(() => EventAggregator.Publish(new ServiceErrorMessage
			                                 		                                                 {
			                                 		                                                 	Error = e.Error
			                                 		                                                 }));
			                                 	}

												EventAggregator.Publish(new AsyncOperationCompleteMessage());
											 };

			proxy.GetAllAssetTypesAsync(UserId);
			EventAggregator.Publish(new AsyncOperationStartedMessage{OperationName = "Downloading asset types"});
		}

		#endregion
	}
}