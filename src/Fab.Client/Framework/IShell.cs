using Caliburn.Micro;
using Fab.Client.Authentication;

namespace Fab.Client.Framework
{
	public interface IShell : IConductor, IGuardClose, IHandle<LoggedInMessage>, IHandle<LoggedOutMessage>
	{
		IDialogManager Dialogs { get; }
	}
}