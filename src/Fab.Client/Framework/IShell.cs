using Caliburn.Micro;

namespace Fab.Client.Framework
{
	public interface IShell : IConductor, IGuardClose
	{
		IDialogManager Dialogs { get; }
	}
}