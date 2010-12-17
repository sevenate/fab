// <copyright file="DocumentBase.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-04-11</date>
// </author>
// <editor name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-04-11</date>
// </editor>
// <summary>Base view model with common validation mechanism for all screens.</summary>

using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Base view model with common validation mechanism for all screens.
	/// </summary>
	public abstract class DocumentBase : Screen, IHaveShutdownTask
	{
		private const string UnsavedDataConfirmationText = "You have unsaved data. Are you sure you want to close this document? All changes will be lost.";
		private const string UnsavedDataHeader = "Unsaved Data";
		private bool isDirty;

		public bool IsDirty
		{
			get { return isDirty; }
			set
			{
				isDirty = value;
				NotifyOfPropertyChange(() => IsDirty);
			}
		}

		[Import]
		public IDialogManager Dialogs { get; set; }

		public override void CanClose(Action<bool> callback)
		{
			if (IsDirty)
			{
				DoCloseCheck(Dialogs, callback);
			}
			else
			{
				callback(true);
			}
		}

		protected virtual void DoCloseCheck(IDialogManager dialogs, Action<bool> callback)
		{
			dialogs.ShowMessageBox(
				UnsavedDataConfirmationText,
				UnsavedDataHeader,
				MessageBoxOptions.YesNo,
				box => callback(box.WasSelected(MessageBoxOptions.Yes))
				);
		}

		#region Implementation of IHaveShutdownTask

		public IResult GetShutdownTask()
		{
			return IsDirty ? new ApplicationCloseCheck(this, DoCloseCheck) : null;
		}

		#endregion
	}
}