using System;
using Caliburn.Micro;
using Fab.Client.Shell;

namespace Fab.Client.Framework
{
	internal class OpenConfirmationResult : IResult
	{
		/// <summary>
		/// Enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private readonly IEventAggregator eventAggregator;

		public string Message { get; set; }
		public string Title { get; set; }
		public MessageBoxOptions Options { get; set; }
		public MessageBoxOptions Selected { get; private set; }

		public OpenConfirmationResult(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;
		}

		#region Implementation of IResult

		/// <summary>
		/// Executes the result within the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			eventAggregator.Publish(new OpenMessageBoxMessage
			{
				Message = Message,
				Title = Title,
				Options = Options,
				Callback = box =>
				{
					//TODO: refactor this if possible
					if (box.WasSelected(MessageBoxOptions.Ok))
					{
						Selected = MessageBoxOptions.Ok;
					}
					else if (box.WasSelected(MessageBoxOptions.Cancel))
					{
						Selected = MessageBoxOptions.Cancel;
					}
					else if (box.WasSelected(MessageBoxOptions.Yes))
					{
						Selected = MessageBoxOptions.Yes;
					}
					else if (box.WasSelected(MessageBoxOptions.No))
					{
						Selected = MessageBoxOptions.No;
					}
					
					Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
				}
			});
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}