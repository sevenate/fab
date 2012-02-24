//------------------------------------------------------------
// <copyright file="AuthenticateResult.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ServiceModel;
using Caliburn.Micro;
using Fab.Client.Shell;

namespace Fab.Client.Authentication
{
	internal class AuthenticateResult : IResult
	{
		public string Username { get; private set; }
		public string Password { get; private set; }
		public UserCredentials Credentials { get; private set; }
		public bool Succeeded { get; private set; }
		public string Status { get; private set; }

		public AuthenticateResult(string username, string password)
		{
			Username = username;
			Password = password;
		}

		#region Implementation of IResult

		/// <summary>
		/// Executes the result within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateUserService();

			// First time client credentials should be initialized "manually";
			// all subsequent calls proxy will have them already setup by factory.
			proxy.ClientCredentials.UserName.UserName = Username;
			proxy.ClientCredentials.UserName.Password = Password;

			proxy.GetUserCompleted += (sender, args) =>
			                            	{
												if (args.Error != null)
												{
													if (args.Error is TimeoutException)
													{
														Status = "Service is not responding. Please try again later.";
														Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
													}
//													if (args.Error.InnerException is FaultException)
//													{
//														Status = args.Error.InnerException.Message;
//														Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
//													}
													else
													{
														Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs { Error = args.Error }));
													}
												}
												else
												{
													Credentials = new UserCredentials(args.Result.Id, Username, Password);
													
													// Story personal service url for future calls
													ServiceFactory.PersonalServiceUri = args.Result.ServiceUrl;
													
													Succeeded = true;
													Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
												}
			                            	};
			proxy.GetUserAsync();
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}