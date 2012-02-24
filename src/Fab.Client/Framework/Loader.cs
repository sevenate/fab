// <copyright file="Loader.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-11-26" />
// <summary>Busy loader result.</summary>

using System;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Busy loader result.
	/// </summary>
	public class Loader : IResult
	{
		private readonly bool hide;
		private readonly string message;

		private Loader(string message)
		{
			this.message = message;
		}

		private Loader(bool hide)
		{
			this.hide = hide;
		}

		#region IResult Members

		public void Execute(ActionExecutionContext context)
		{
			var view = context.View as FrameworkElement;
			
			while (view != null)
			{
				var busyIndicator = view as BusyIndicator;
				
				if (busyIndicator != null)
				{
					if (!string.IsNullOrEmpty(message))
					{
						busyIndicator.BusyContent = message;
					}

					busyIndicator.IsBusy = !hide;
					break;
				}

				view = view.Parent as FrameworkElement;
			}

			Completed(this, new ResultCompletionEventArgs());
		}

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		#endregion

		public static IResult Show(string message = null)
		{
			return new Loader(message);
		}

		public static IResult Hide()
		{
			return new Loader(true);
		}
	}
}