using System.Threading;
using Caliburn.Micro;

namespace Fab.Client.Framework.Filters
{
	/// <summary>
	/// Provides asynchronous execution of the action in a background thread
	/// </summary>
	/// <example>
	/// <code>
	/// [Async]
	/// public void MyAction() { ... }
	/// </code>
	/// </example>
	public class AsyncAttribute : ExecutionWrapperBase
	{
		protected override void Execute(IResult inner, ActionExecutionContext context)
		{
			ThreadPool.QueueUserWorkItem(state => { inner.Execute(context); });
		}
	}
}