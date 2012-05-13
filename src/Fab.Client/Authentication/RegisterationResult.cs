//------------------------------------------------------------
// <copyright file="RegisterationResult.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.ServiceModel;
using Caliburn.Micro;
using Fab.Client.RegistrationServiceReference;
using Fab.Client.Shell;

namespace Fab.Client.Authentication
{
	public class RegisterationResult : IResult
	{
		public string Username { get; private set; }
		public string Password { get; private set; }
		public UserCredentials Credentials { get; private set; }
		public bool Succeeded { get; private set; }
		public string Status { get; private set; }

		public RegisterationResult(string username, string password)
		{
			Username = username;
			Password = password;
		}

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			var proxy = ServiceFactory.CreateRegistrationService();

			proxy.RegisterCompleted += (sender, args) =>
			{
				if (args.Error != null)
				{
					if (args.Error is FaultException<FaultDetail>)
					{
						Status = ((FaultException<FaultDetail>)args.Error).Detail.Description;
						Caliburn.Micro.Execute.OnUIThread(
							() => Completed(this, new ResultCompletionEventArgs()));
					}
					else
					{
						Caliburn.Micro.Execute.OnUIThread(
							() => Completed(this, new ResultCompletionEventArgs { Error = args.Error }));
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
			
			proxy.RegisterAsync(Username, Password);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}