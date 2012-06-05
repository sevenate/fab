//------------------------------------------------------------
// <copyright file="CountUsersResult.cs" company="nReez">
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
	public class CountUsersResult : ResultBase, IResult
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
																					SendErrorMessage(args.Error);
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