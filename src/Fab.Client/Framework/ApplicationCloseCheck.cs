using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Client.Framework
{
	public class ApplicationCloseCheck : IResult
	{
		private readonly Action<IDialogManager, Action<bool>> closeCheck;
		private readonly IChild<IConductor> screen;

		public ApplicationCloseCheck(IChild<IConductor> screen, Action<IDialogManager, Action<bool>> closeCheck)
		{
			this.screen = screen;
			this.closeCheck = closeCheck;
		}

		[Import]
		public IShell Shell { get; set; }

		#region IResult Members

		public void Execute(ActionExecutionContext context)
		{
			var documentWorkspace = screen.Parent as IDocumentWorkspace;
			
			if (documentWorkspace != null)
			{
				documentWorkspace.Edit(screen);
			}

			closeCheck(Shell.Dialogs, result => Completed(this, new ResultCompletionEventArgs {WasCancelled = !result}));
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion
	}
}