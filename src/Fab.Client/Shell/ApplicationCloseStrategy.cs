using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Fab.Client.Framework;

namespace Fab.Client.Shell
{
	public class ApplicationCloseStrategy : ICloseStrategy<IModule>
	{
		private Action<bool, IEnumerable<IModule>> callback;
		private IEnumerator<IModule> enumerator;
		private bool finalResult;

		#region ICloseStrategy<IModule> Members

		public void Execute(IEnumerable<IModule> toClose, Action<bool, IEnumerable<IModule>> callback)
		{
			enumerator = toClose.GetEnumerator();
			this.callback = callback;
			finalResult = true;

			Evaluate(finalResult);
		}

		#endregion

		private void Evaluate(bool result)
		{
			finalResult = finalResult && result;

			if (!enumerator.MoveNext() || !result)
			{
				callback(finalResult, new List<IModule>());
			}
			else
			{
				IModule current = enumerator.Current;
				var conductor = current as IConductor;
				
				if (conductor != null)
				{
					IEnumerable<IResult> tasks = conductor.GetConductedItems()
						.OfType<IHaveShutdownTask>()
						.Select(x => x.GetShutdownTask())
						.Where(x => x != null);

					var sequential = new SequentialResult(tasks.GetEnumerator());
					sequential.Completed += (s, e) =>
					                        	{
					                        		if (!e.WasCancelled)
					                        			Evaluate(!e.WasCancelled);
					                        	};
					sequential.Execute(new ActionExecutionContext());
				}
				else
				{
					Evaluate(true);
				}
			}
		}
	}
}