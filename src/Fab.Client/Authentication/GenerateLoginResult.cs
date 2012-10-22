//------------------------------------------------------------
// <copyright file="GenerateLoginResult.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Threading;
using Caliburn.Micro;
using Fab.Client.Shell;

namespace Fab.Client.Authentication
{
	public class GenerateLoginResult : IResult
	{
		#region Properties

		public string UniqueUsername { get; private set; }

		#endregion

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateRegistrationService();

			proxy.GenerateUniqueLoginCompleted += (sender, args) =>
			{
				Thread.Sleep(5000);

				if (args.Error != null)
				{
					Caliburn.Micro.Execute.OnUIThread(
						() => Completed(this, new ResultCompletionEventArgs { Error = args.Error }));
				}
				else
				{
					UniqueUsername = args.Result;
					Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
				}
			};

			proxy.GenerateUniqueLoginAsync();
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}