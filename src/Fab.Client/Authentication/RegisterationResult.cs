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
		#region Consts

		private const string ErrorCode0 = "ERR-REGS-0";
		private const string ErrorCode1 = "ERR-REGS-1";

		#endregion

		#region Properties

		public string Username { get; private set; }
		public string Password { get; private set; }
		public UserCredentials Credentials { get; private set; }
		public bool Succeeded { get; private set; }
		public string Status { get; private set; }

		#endregion

		#region .Ctor

		public RegisterationResult(string username, string password)
		{
			Username = username;
			Password = password;
		}

		#endregion

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
						if (((FaultException<FaultDetail>) args.Error).Detail.ErrorCode == ErrorCode0)
						{
							Status = Resources.Strings.RegistrationView_ERR_REGS_0;
						}
						else if (((FaultException<FaultDetail>) args.Error).Detail.ErrorCode == ErrorCode1)
						{
							Status = string.Format(Resources.Strings.RegistrationView_ERR_REGS_1, Username);
						}
						else
						{
							Status = ((FaultException<FaultDetail>) args.Error).Detail.Description;
						}
						
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