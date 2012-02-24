//------------------------------------------------------------
// <copyright file="RepairResult.cs" company="nReez">
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
	public class RepairResult : ResultBase, IResult
	{
		public Guid Id { get; set; }
		public long? DatabaseSize { get; private set; }

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			AdminServiceClient adminServiceClient = Helpers.CreateClientProxy(null, null, null);
			adminServiceClient.RepairUserDatabaseCompleted += (o, args) =>
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
																			DatabaseSize = args.Result;
																			Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
																		}
																	};
			adminServiceClient.RepairUserDatabaseAsync(Id);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		#endregion
	}
}