﻿using System;
using Caliburn.Micro;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Framework;

namespace Fab.Managment.Shell.Results
{
	public class OptimizeResult : IResult
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
			adminServiceClient.OptimizeUserDatabaseCompleted += (o, args) =>
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
			                                                    			DatabaseSize = args.Result;
																			Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
			                                                    		}
			                                                    	};
			adminServiceClient.OptimizeUserDatabaseAsync(Id);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		#endregion
	}
}