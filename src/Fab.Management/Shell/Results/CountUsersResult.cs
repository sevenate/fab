//------------------------------------------------------------
// <copyright file="CountUsersResult.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Caliburn.Micro;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Framework;

namespace Fab.Managment.Shell.Results
{
	public class CountUsersResult : IResult
	{
		public QueryFilter Filer { get; set; }
		public int Count { get; private set; }

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			AdminServiceClient adminServiceClient = Helpers.CreateClientProxy(null, null, null);
			adminServiceClient.GetUsersCountCompleted += (o, args) =>
																	{
																		if (args.Error != null)
																		{
																			Caliburn.Micro.Execute.OnUIThread(
																				() =>
																				{
																					Completed(this, new ResultCompletionEventArgs());
																					Helpers.ErrorProcessing(args);
																				});
																		}
																		else
																		{
																			Count = args.Result;
																			Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
																		}
																	};
			adminServiceClient.GetUsersCountAsync(Filer);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		#endregion
	}
}