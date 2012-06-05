using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;

namespace Fab.Client.Framework
{
	public class ExecuteActionResult : IResult
	{
		private readonly object[] parameters;

		public ExecuteActionResult(object[] parameters)
		{
			this.parameters = parameters;
		}

		public object Outcome { get; private set; }

		#region IResult Members

		public void Execute(ActionExecutionContext context)
		{
			try
			{
				Outcome = context.Method.Invoke(context.Target, parameters);
				Completed(this, new ResultCompletionEventArgs());
			}
			catch (Exception ex)
			{
				Completed(this, new ResultCompletionEventArgs {Error = ex});
			}
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion

		public IEnumerator<IResult> GetOutcomeEnumerator()
		{
			var single = Outcome as IResult;
			if (single != null)
				return Enumerable.Repeat(single, 1).GetEnumerator();

			var enumerable = Outcome as IEnumerable<IResult>;
			if (enumerable != null)
				return enumerable.GetEnumerator();

			var enumerator = Outcome as IEnumerator<IResult>;
			if (enumerator != null)
				return enumerator;

			return null;
		}
	}
}