﻿using System;
using System.Collections;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Fab.Client.Framework;

namespace Fab.Client.Shell
{
	[Export(typeof (IDialogManager))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class DialogConductorViewModel : PropertyChangedBase, IDialogManager, IConductor
	{
		private readonly Func<IMessageBox> createMessageBox;

		[ImportingConstructor]
		public DialogConductorViewModel(Func<IMessageBox> messageBoxFactory)
		{
			createMessageBox = messageBoxFactory;
		}

		public IScreen ActiveItem { get; private set; }

		#region IConductor Members

		public IEnumerable GetConductedItems()
		{
			return ActiveItem != null
						? new[] {ActiveItem}
						: new object[0];
		}

		public void ActivateItem(object item)
		{
			ActiveItem = item as IScreen;

			var child = ActiveItem as IChild<IConductor>;
			
			if (child != null)
			{
				child.Parent = this;
			}

			if (ActiveItem != null)
			{
				ActiveItem.Activate();
			}

			NotifyOfPropertyChange(() => ActiveItem);
			ActivationProcessed(this, new ActivationProcessedEventArgs {Item = ActiveItem, Success = true});
		}

		public void CloseItem(object item)
		{
			var guard = item as IGuardClose;
			
			if (guard != null)
			{
				guard.CanClose(result =>
				               	{
				               		if (result)
				               		{
				               			CloseActiveItemCore();
				               		}
				               	});
			}
			else
			{
				CloseActiveItemCore();
			}
		}

		object IConductor.ActiveItem
		{
			get { return ActiveItem; }
			set { ActivateItem(value); }
		}

		public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

		#endregion

		#region IDialogManager Members

		public void ShowDialog(IScreen dialogModel)
		{
			ActivateItem(dialogModel);
		}

		public void ShowMessageBox(string message, string title = null, MessageBoxOptions options = MessageBoxOptions.Ok,
		                           Action<IMessageBox> callback = null)
		{
			var box = createMessageBox();

			box.DisplayName = title ?? "Hello Screens";
			box.Options = options;
			box.Message = message;

			if (callback != null)
			{
				box.Deactivated += delegate { callback(box); };
			}

			ActivateItem(box);
		}

		#endregion

		private void CloseActiveItemCore()
		{
			var oldItem = ActiveItem;
			ActivateItem(null);
			oldItem.Deactivate(true);
		}
	}
}