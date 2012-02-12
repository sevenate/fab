// <copyright file="DefaultButtonBehavior.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Michael Collins" url="http://blogs.neudesic.com/post/2010/07/27/Text-Boxes-and-Default-Buttons-in-Silverlight-and-WPF.aspx" date="2010-07-27" />
// <summary>Associated <see cref="TextBox"/> "enter" key press with default button click.</summary>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Fab.Client.Framework.Behaviors
{
	/// <summary>
	/// Associated <see cref="TextBox"/> "enter" key press with default button click.
	/// </summary>
	public class DefaultButtonBehavior : Behavior<UIElement>
	{
		#region Default Button DP

		/// <summary>
		/// Dependency property registration for <see cref="DefaultButton"/>.
		/// </summary>
		public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.RegisterAttached(
			"DefaultButton",
			typeof (Button),
			typeof (DefaultButtonBehavior),
			new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets associated default button.
		/// </summary>
		[Description("Gets or sets the associated default button")]
		public Button DefaultButton
		{
			get { return (Button) GetValue(DefaultButtonProperty); }
			set { SetValue(DefaultButtonProperty, value); }
		}

		#endregion

		#region Overrides of Behavior

		/// <summary>
		/// Called after the behavior is attached to an AssociatedObject.
		/// </summary>
		/// <remarks>
		/// Override this to hook up functionality to the AssociatedObject.
		/// </remarks>
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.KeyUp += OnKeyUp;
		}

		/// <summary>
		/// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		/// <remarks>
		/// Override this to unhook functionality from the AssociatedObject.
		/// </remarks>
		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.KeyUp -= OnKeyUp;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// The <see cref="UIElement.KeyUp"/> handler.
		/// </summary>
		/// <param name="sender">The <see cref="UIElement"/> itself. Most likely it will be <see cref="TextBox"/>.</param>
		/// <param name="args">"Key" event data.</param>
		private void OnKeyUp(object sender, KeyEventArgs args)
		{
			// react only on "enter" key
			if (args.Key != Key.Enter)
			{
				return;
			}

			UpdateBinding<TextBox>(sender, TextBox.TextProperty);

			// "click" on associated button
			if (DefaultButton == null)
			{
				throw new Exception("Default button is not specified - or - binding to specified property failed.");
			}

			if (DefaultButton.IsEnabled)
			{
				var peer = new ButtonAutomationPeer(DefaultButton);
				var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;

				if (invokeProvider != null)
				{
					invokeProvider.Invoke();
				}
			}
		}

		/// <summary>
		/// Update binding source so that view-model property
		/// could have the latest actual value at the moment when default button will be pressed.
		/// </summary>
		/// <typeparam name="T">Any descendant of the <see cref="FrameworkElement"/>.</typeparam>
		/// <param name="sender">The actual element instance.</param>
		/// <param name="dp">Dependency property on <typeparamref name="T"/>, i.e. binding target.</param>
		private static void UpdateBinding<T>(object sender, DependencyProperty dp) where T : FrameworkElement
		{
			if (sender is T)
			{
				var textBox = sender as T;

				var binding = textBox.GetBindingExpression(dp);

				if (binding != null)
				{
					binding.UpdateSource();
				}
			}
		}

		#endregion
	}
}