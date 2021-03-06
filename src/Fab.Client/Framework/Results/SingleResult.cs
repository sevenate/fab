﻿//------------------------------------------------------------
// <copyright file="SingleResult.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using Caliburn.Micro;
using Action = System.Action;
using Fab.Client.Framework;

namespace Fab.Client.Framework.Results
{
	/// <summary>
	/// Single <see cref="IResult"/> that will cause all changed binded properties
	/// appear on screen before any other sequential IResults will continue to execute.
	/// </summary>
	public class SingleResult : IResult
	{
		public Action Action { get; set; }

		#region Implementation of IResult

		/// <summary>
		/// Executes the result using the specified context.
		/// </summary>
		/// <param name="context">The context.</param>
		public void Execute(ActionExecutionContext context)
		{
			Action();

			// Push all binded properties changes during Action to appear faster on screen
			//DispatcherHelper.ForceBindingUpdate();

			Completed(this, new ResultCompletionEventArgs());
		}

		public event EventHandler<ResultCompletionEventArgs> Completed;

		#endregion
	}
}