using Caliburn.Micro;

namespace Fab.Client.Framework
{
	public interface IHaveShutdownTask
	{
		IResult GetShutdownTask();
	}
}