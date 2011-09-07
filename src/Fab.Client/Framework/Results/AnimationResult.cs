using Caliburn.Micro;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Fab.Client.Framework.Results
{
	public class AnimationResult : IResult
    {
        public enum AnimationAction
        {
            Begin,
            Pause,
            Resume,
            Stop
        }

        private bool wait;
        private readonly string key;
        private readonly AnimationAction action;

        public AnimationResult(string key, AnimationAction action)
        {
            this.key = key;
            this.action = action;
        }

        public string Key
        {
            get { return key; }
        }

        public AnimationAction Action
        {
            get { return action; }
        }

        public AnimationResult Wait()
        {
            wait = true;
            return this;
        }

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			FrameworkElement element = null;

			if (context.View != null)
				element = context.View as FrameworkElement;

			if (element == null && context.Message != null)
				element = context.Source;

//			if (context.HandlingNode != null)
//				element = context.HandlingNode.UIElement as FrameworkElement;

//			if (element == null && context.Message != null)
//				element = context.Message.Source.UIElement as FrameworkElement;

			if (element == null)
			{
				Completed(this, new ResultCompletionEventArgs());
				return;
			}

			var storyboard = (Storyboard)element.Resources[key];

			if (wait)
				storyboard.Completed += delegate { Completed(this, new ResultCompletionEventArgs()); };

			switch (action)
			{
				case AnimationAction.Begin:
					storyboard.Begin();
					break;
				case AnimationAction.Pause:
					storyboard.Pause();
					break;
				case AnimationAction.Resume:
					storyboard.Resume();
					break;
				case AnimationAction.Stop:
					storyboard.Stop();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (!wait)
				Completed(this, new ResultCompletionEventArgs());
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}