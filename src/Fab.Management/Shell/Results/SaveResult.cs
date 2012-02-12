//------------------------------------------------------------
// <copyright file="SaveResult.cs" company="nReez">
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
	public class SaveResult : ResultBase, IResult
	{
		public AdminUserDTO User { get; set; }
		public DateTime? LastUpdated { get; private set; }

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			AdminServiceClient adminServiceClient = Helpers.CreateClientProxy(null, null, null);
			adminServiceClient.UpdateUserCompleted += (o, args) =>
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
																			LastUpdated = args.Result;
																			Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
																		}
																	};
			adminServiceClient.UpdateUserAsync(User);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		#endregion
	}
}