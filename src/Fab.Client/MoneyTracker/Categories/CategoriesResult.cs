// <copyright file="CategoriesResult.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-12" />
// <summary>Load all categories for user result.</summary>

using System;
using Caliburn.Micro;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Load all categories for user result.
	/// </summary>
	public class CategoriesResult : IResult
	{
		public CategoriesResult(Guid userId)
		{
			UserId = userId;
		}

		public Guid UserId { get; private set; }

		public CategoryDTO[] Categories { get; set; }

		public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

		public void Execute(ActionExecutionContext context)
		{
			var proxy = new MoneyServiceClient();

			proxy.GetAllCategoriesCompleted += (s, e) =>
			{
				if (e.Error != null)
				{
					Caliburn.Micro.Execute.OnUIThread(
						() => Completed(this, new ResultCompletionEventArgs { Error = e.Error }));
				}
				else
				{
					Categories = e.Result;
					Caliburn.Micro.Execute.OnUIThread(() => Completed(this, new ResultCompletionEventArgs()));
				}
			};

			proxy.GetAllCategoriesAsync(UserId);
		}
	}
}