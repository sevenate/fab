using System;
using Caliburn.Micro;
using Fab.Client.UserServiceReference;

namespace Fab.Client.Authentication
{
	internal class AuthenticateResult : IResult
	{
		public string Username { get; private set; }
		public string Password { get; private set; }
		public UserCredentials Credentials { get; private set; }
		public bool Succeeded { get; set; }

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
			var proxy = new UserServiceClient();

			proxy.GetUserCompleted += (sender, args) =>
			                            	{
												if (args.Error != null || args.Result.Id == Guid.Empty)
												{
													Caliburn.Micro.Execute.OnUIThread(
														() => Completed(this, new ResultCompletionEventArgs { Error = args.Error }));
												}
												else
												{
													Credentials = new UserCredentials(args.Result.Id, Username);
													Succeeded = true;
													Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
												}
			                            	};
			proxy.GetUserAsync(Username, Password);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}