// <copyright file="CategoriesViewModel.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="alevshoff@hd.com" date="2010-04-12" />
// <summary>Categories view model.</summary>

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Categories view model.
	/// </summary>
	[Export(typeof(ICategoriesViewModel))]
	public class CategoriesViewModel : Screen, ICategoriesViewModel
	{
		#region Fields

		/// <summary>
		/// Accounts owner ID.
		/// </summary>
		private readonly Guid userId = new Guid("DC57BFF0-57A6-4BFC-9104-5F323ABBEDAB"); // 7F06BFA6-B675-483C-9BF3-F59B88230382

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesViewModel"/> class.
		/// </summary>
		public CategoriesViewModel()
		{
			Categories = new BindableCollection<CategoryDTO>();
		}

		#endregion

		#region Implementation of ICategoriesViewModel

		/// <summary>
		/// Gets categories for specific user.
		/// </summary>
		public IObservableCollection<CategoryDTO> Categories { get; private set; }

		/// <summary>
		/// Download all categories for specific user.
		/// </summary>
		/// <returns>Operation result.</returns>
		public IEnumerable<IResult> LoadAllCategories()
		{
			yield return Loader.Show("Loading...");

			var request = new CategoriesResult(userId);
			yield return request;

			Categories.Clear();
			Categories.AddRange(request.Categories);

			if (Reloaded != null)
			{
				Reloaded(this, EventArgs.Empty);
			}

			yield return Loader.Hide();
		}

		/// <summary>
		/// Raised right after categories were reloaded from server.
		/// </summary>
		public event EventHandler<EventArgs> Reloaded;

		#endregion
	}
}