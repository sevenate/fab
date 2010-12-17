using System;
using Caliburn.Micro;
using Fab.Client.UserServiceReference;

namespace Fab.Client.Authentication
{
	internal class AuthenticateResult : IResult
	{
		public string Username { get; private set; }
		public string Password { get; private set; }
		public Guid UserId { get; private set; }
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

			proxy.GetUserIdCompleted += (sender, args) =>
			                            	{
												if (args.Error != null || args.Result == Guid.Empty)
												{
													Caliburn.Micro.Execute.OnUIThread(
														() => Completed(this, new ResultCompletionEventArgs { Error = args.Error }));
												}
												else
												{
													UserId = args.Result;
													Succeeded = true;
													Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
												}
			                            	};
			proxy.GetUserIdAsync(Username);
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}