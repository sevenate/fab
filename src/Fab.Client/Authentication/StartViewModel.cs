//------------------------------------------------------------
// <copyright file="StartViewModel.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Controls;
using Caliburn.Micro;
using Fab.Client.Framework;
using Fab.Client.Localization;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// View-model for <see cref="StartView"/> dialog.
	/// </summary>
	[Export(typeof (IModule))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class StartViewModel : LocalizableScreen, IStartViewModel, IHandle<LoggedOutMessage>, IHandle<NavigateToRegistrationScreenMessage>
	{
		#region Constants

		/// <summary>
		/// "Long" copyright message.
		/// </summary>
		private const string LongCopyrightTemplate = "Copyright � 2009-{0} nReez Software. All rights reserved.";

		/// <summary>
		/// "Short" copyright message.
		/// </summary>
		private const string ShortCopyrightTemplate = "� {0} nReez";

		#endregion

		#region Private Properties

		/// <summary>
		/// Gets or sets global instance of the <see cref="IEventAggregator"/> that enables loosely-coupled publication of and subscription to events.
		/// </summary>
		private IEventAggregator EventAggregator { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// Gets copyright information.
		/// </summary>
		public string Copyright
		{
			get { return string.Format(ShortCopyrightTemplate, DateTime.Now.Year); }
		}

		/// <summary>
		/// Gets current application version.
		/// </summary>
		public string Version
		{
			get { return AssemblyExtensions.AppVersion; }
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

		public LoginViewModel LoginForm { get; set; }
		public RegistrationViewModel RegistrationForm { get; set; }

		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="StartViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Events exchange entry point.</param>
		/// <param name="loginVM">Login view model.</param>
		/// <param name="registrationVM">Registration view model.</param>
		[ImportingConstructor]
		public StartViewModel(IEventAggregator eventAggregator, LoginViewModel loginVM, RegistrationViewModel registrationVM)
		{
			EventAggregator = eventAggregator;
			EventAggregator.Subscribe(this);

			LoginForm = loginVM;
			RegistrationForm = registrationVM;
		}

		#endregion

		#region Overrides of Screen

		/// <summary>
		/// Called when activating.
		/// </summary>
		protected override void OnActivate()
		{
			base.OnActivate();
			LoginForm.ActivateWith(this);
		}

		/// <summary>
		/// Called when deactivating.
		/// </summary>
		/// <param name="close">Indicates whether this instance will be closed.</param>
		protected override void OnDeactivate(bool close)
		{
			base.OnDeactivate(close);
			LoginForm.DeactivateWith(this);
		}

		#endregion

		#region Implementation of IHandle<in LoggedOutMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(LoggedOutMessage message)
		{
			ClearForm();
		}

		#endregion

		public void Handle(NavigateToRegistrationScreenMessage message)
		{
//			var a = Animation.Begin("GoToRegisterStoryboard");
//			IEnumerable<IResult> resutls = new BindableCollection<IResult>();
//			resutls.Apply(a);
//			Coroutine.BeginExecute();

//			yield return Animation.Begin("GoToRegisterStoryboard");
		}

		#region Implementation of IModule

		public string Name
		{
			get { return "Start"; }
		}

		public Control Icon { get { return null; } }

		/// <summary>
		/// Determine order in UI representation (like position in ItemsControl.Items).
		/// </summary>
		public int Order
		{
			get { return 0; }
		}

		public void Show()
		{
			if (Parent is IHaveActiveItem && ((IHaveActiveItem)Parent).ActiveItem == this)
			{
				DisplayName = Name;
			}
			else
			{
				((IConductor)Parent).ActivateItem(this);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Clear form data.
		/// </summary>
		private void ClearForm()
		{
		}

		#endregion
	}
}