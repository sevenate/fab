using System;
using Caliburn.Micro;

namespace Fab.Client.Framework.Filters
{
	public interface IContextAware : IFilter, IDisposable
	{
		void MakeAwareOf(ActionExecutionContext context);
	}
}