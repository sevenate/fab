﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fab.Client.Shell
{
	public partial class DialogConductorView
	{
		private bool disabled;

		public DialogConductorView()
		{
			InitializeComponent();
			ActiveItem.ContentChanged += OnTransitionCompleted;
			Loaded += OnLoad;
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			if (disabled) DisableBackground();
		}

		private void OnTransitionCompleted(object sender, EventArgs e)
		{
			if (ActiveItem.Content == null)
			{
				EnableBackground();
			}
			else
			{
				DisableBackground();

				var control = ActiveItem.Content as Control;
				
				if (control != null)
				{
					control.Focus();
				}
			}
		}

		public void EnableBackground()
		{
			disabled = false;
			ChangeEnabledState(GetBackground(), true);
		}

		public void DisableBackground()
		{
			disabled = true;
			ChangeEnabledState(GetBackground(), false);
		}

		private IEnumerable<UIElement> GetBackground()
		{
			var contentControl = (ContentControl) Parent;
			var container = (Panel) contentControl.Parent;
			return container.Children.Where(child => child != contentControl);
		}

		private void ChangeEnabledState(IEnumerable<UIElement> background, bool state)
		{
			foreach (var uiElement in background)
			{
				var control = uiElement as Control;
				
				if (control != null)
				{
					control.IsEnabled = state;
				}
				else
				{
					var panel = uiElement as Panel;
					
					if (panel != null)
					{
						foreach (var child in panel.Children)
						{
							ChangeEnabledState(new[] {child}, state);
						}
					}
				}
			}
		}
	}
}