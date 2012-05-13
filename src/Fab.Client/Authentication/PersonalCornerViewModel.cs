//------------------------------------------------------------
// <copyright file="RegisterationResult.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Localization;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// User personal corner with "welcome/logout/settings".
	/// </summary>
	[Export(typeof(PersonalCornerViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class PersonalCornerViewModel : LocalizableScreen, IHandle<LoggedInMessage>
	{
		#region Private

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// Gets current application version.
		/// </summary>
		public string Version
		{
			get { return AssemblyExtensions.AppVersion; }
		}

		#endregion

		#region Username DP

		/// <summary>
		/// User name.
		/// </summary>
		private string username;

		/// <summary>
		/// Gets or sets user name.
		/// </summary>
		public string Username
		{
			get { return username; }
			set
			{
				username = value;
				NotifyOfPropertyChange(() => Username);
			}
		}

		public IEnumerable<CultureInfo> Cultures
		{
			get { return Translator.SupportedCultures; }
		}

		public CultureInfo CurrentCulture
		{
			get { return Translator.CurrentCulture; }
			set { Translator.CurrentCulture = value; }
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="PersonalCornerViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Events exchange entry point.</param>
		[ImportingConstructor]
		public PersonalCornerViewModel(IEventAggregator eventAggregator)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Logout user from the system.
		/// </summary>
		public void Logout()
		{
			UserCredentials.Current = null;
			EventAggregator.Publish(new LoggedOutMessage());
			Username = string.Empty;
		}

		#endregion

		#region Implementation of IHandle<in LoggedInMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(LoggedInMessage message)
		{
			Username = message.Credentials.UserName;
			NotifyOfPropertyChange(() => CurrentCulture);
		}

		#endregion
	}
}