using Caliburn.Micro;

namespace Fab.Client.Framework.Filters
{
	public interface IExecutionWrapper : IFilter
	{
		IResult Wrap(IResult inner);
	}
}