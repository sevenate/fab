using System;
using Caliburn.Micro;

namespace Fab.Client.Framework.Filters
{
	public abstract class ExecutionWrapperBase : Attribute, IExecutionWrapper, IResult
	{
		private IResult inner;

		#region IExecutionWrapper Members

		public int Priority { get; set; }

		IResult IExecutionWrapper.Wrap(IResult inner)
		{
			this.inner = inner;
			return this;
		}

		#endregion

		#region IResult Members

		void IResult.Execute(ActionExecutionContext context)
		{
			if (!CanExecute(context))
			{
				CompletedEvent(this, new ResultCompletionEventArgs {WasCancelled = true});
				return;
			}

			try
			{
				EventHandler<ResultCompletionEventArgs> onCompletion = null;
				onCompletion = (o, e) =>
				               	{
				               		inner.Completed -= onCompletion;
				               		AfterExecute(context);
				               		FinalizeExecution(context, e.WasCancelled, e.Error);
				               	};
				inner.Completed += onCompletion;

				BeforeExecute(context);
				Execute(inner, context);
			}
			catch (Exception ex)
			{
				FinalizeExecution(context, false, ex);
			}
		}

		event EventHandler<ResultCompletionEventArgs> IResult.Completed
		{
			add { CompletedEvent += value; }
			remove { CompletedEvent -= value; }
		}

		#endregion

		private event EventHandler<ResultCompletionEventArgs> CompletedEvent = delegate { };

		/// <summary>
		/// Check prerequisites
		/// </summary>
		protected virtual bool CanExecute(ActionExecutionContext context)
		{
			return true;
		}

		/// <summary>
		/// Called just before execution (if prerequisites are met)
		/// </summary>
		protected virtual void BeforeExecute(ActionExecutionContext context)
		{
		}

		/// <summary>
		/// Called after execution (if prerequisites are met)
		/// </summary>
		protected virtual void AfterExecute(ActionExecutionContext context)
		{
		}

		/// <summary>
		/// Allows to customize the dispatch of the execution
		/// </summary>
		protected virtual void Execute(IResult inner, ActionExecutionContext context)
		{
			inner.Execute(context);
		}

		/// <summary>
		/// Called when an exception was thrown during the action execution
		/// </summary>
		protected virtual bool HandleException(ActionExecutionContext context, Exception ex)
		{
			return false;
		}

		private void FinalizeExecution(ActionExecutionContext context, bool wasCancelled, Exception ex)
		{
			if (ex != null && HandleException(context, ex))
				ex = null;

			CompletedEvent(this, new ResultCompletionEventArgs {WasCancelled = wasCancelled, Error = ex});
		}
	}
}