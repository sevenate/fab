﻿//------------------------------------------------------------
// <copyright file="CheckAccountsCachedValuesResult.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Caliburn.Micro;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Framework;
using Fab.Managment.Framework.Results;

namespace Fab.Managment.Shell.Results
{
	public class CheckAccountsCachedValuesResult : ResultBase, IResult
	{
		public Guid Id { get; set; }
		public bool UpdateCachedValues { get; set; }
		public AccountMaintenanceDTO[] Accounts { get; private set; }

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			AdminServiceClient adminServiceClient = Helpers.CreateClientProxy(null, null, null);
			adminServiceClient.UpdateCachedValuesForUserAccountsCompleted += (o, args) =>
			                                                    	{
			                                                    		if (args.Error != null)
			                                                    		{
			                                                    			Caliburn.Micro.Execute.OnUIThread(
			                                                    				() =>
			                                                    					{
																						Completed(this, new ResultCompletionEventArgs());
			                                                    						SendErrorMessage(args.Error);
			                                                    					});
			                                                    		}
			                                                    		else
			                                                    		{
			                                                    			Accounts = args.Result;
																			Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
			                                                    		}
			                                                    	};
			adminServiceClient.UpdateCachedValuesForUserAccountsAsync(Id, !UpdateCachedValues);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		#endregion
	}
}