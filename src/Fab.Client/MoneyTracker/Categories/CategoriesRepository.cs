// <copyright file="CategoriesRepository.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-03-26" />

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Authentication;
using Fab.Client.Framework;
using Fab.Client.MoneyServiceReference;

namespace Fab.Client.MoneyTracker.Categories
{
	/// <summary>
	/// Provide access to all user categories.
	/// </summary>
	[Export(typeof(ICategoriesRepository))]
	public class CategoriesRepository : RepositoryBase, ICategoriesRepository
	{
		#region Entities Collections

		/// <summary>
		/// List of all user categories.
		/// </summary>
		private readonly List<CategoryDTO> categories = new List<CategoryDTO>();

		/// <summary>
		/// Gets categories for specific user.
		/// </summary>
		public IEnumerable<CategoryDTO> Categories
		{
			get { return categories; }
		}

		#endregion

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoriesRepository"/> class.
		/// </summary>
		/// <param name="eventAggregator">Global event aggregator instance to send messages.</param>
		[ImportingConstructor]
		public CategoriesRepository(IEventAggregator eventAggregator):base(eventAggregator)
		{
		}

		#endregion

		#region Implementation of IHandle<in LoggedInMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public override void Handle(LoggedInMessage message)
		{
			base.Handle(message);
			DownloadAll();
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public override void Handle(LoggedOutMessage message)
		{
			base.Handle(message);
			categories.Clear();
			EventAggregator.Publish(new CategoriesUpdatedMessage
			{
				Categories = Categories
			});
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Download all <see cref="CategoryDTO"/> entities from server.
		/// </summary>
		public void DownloadAll()
		{
			var proxy = new MoneyServiceClient();

			proxy.GetAllCategoriesCompleted += (s, e) =>
			{
				if (e.Error == null)
				{
					categories.Clear();
					categories.AddRange(e.Result);
					Execute.OnUIThread(() => EventAggregator.Publish(new CategoriesUpdatedMessage
					{
						Categories = Categories
					}));
				}
				else
				{
					Execute.OnUIThread(() => EventAggregator.Publish(new CategoriesUpdatedMessage
					{
						Error = e.Error
					}));
				}
			};

			proxy.GetAllCategoriesAsync(UserId);
		}

		#endregion
	}
}