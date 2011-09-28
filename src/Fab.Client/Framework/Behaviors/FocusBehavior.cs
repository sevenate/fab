// <copyright file="FocusBehavior.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="dfaivre" url="http://caliburnmicro.codeplex.com/Thread/View.aspx?ThreadId=222892" date="2010-10-30" />
// <summary>Allow to focus specific control from view model.</summary>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Fab.Client.Framework.Behaviors
{
	/// <summary>
	/// Allow to focus specific control from view model.
	/// </summary>
	public class FocusBehavior : Behavior<Control>
	{
		#region HasInitialFocus DP

		/// <summary>
		/// <see cref="HasInitialFocus"/> dependency property registration.
		/// </summary>
		public static readonly DependencyProperty HasInitialFocusProperty =
			DependencyProperty.Register(
				"HasInitialFocus",
				typeof (bool),
				typeof (FocusBehavior),
				new PropertyMetadata(false, null));

		/// <summary>
		/// Gets or sets a value indicating whether a associated object should be focused after <see cref="FrameworkElement.Loaded"/> event.
		/// Default is <c>false</c>.
		/// This is Dependency Property.
		/// </summary>
		public bool HasInitialFocus
		{
			get { return (bool) GetValue(HasInitialFocusProperty); }
			set { SetValue(HasInitialFocusProperty, value); }
		}

		#endregion

		#region IsFocused DP

		/// <summary>
		/// <see cref="IsFocused"/> dependency property registration.
		/// </summary>
		public static readonly DependencyProperty IsFocusedProperty =
			DependencyProperty.Register(
				"IsFocused",
				typeof (bool),
				typeof (FocusBehavior),
				new PropertyMetadata(false, (d, e) =>
				                            	{
				                            		if ((bool) e.NewValue)
				                            		{
				                            			var control = ((FocusBehavior) d).AssociatedObject;

				                            			if (control.IsEnabled)
				                            			{
				                            				control.Focus();
				                            			}
				                            			else
				                            			{
															// postpone focus() until control becomes enabled
				                            				control.IsEnabledChanged += OnAssociatedObjectEnabledChanged;
				                            			}
				                            		}
				                            	}));

		/// <summary>
		/// Gets or sets a value indicating whether a associated object is focused.
		/// Default is <c>false</c>.
		/// This is Dependency Property.
		/// </summary>
		public bool IsFocused
		{
			get { return (bool) GetValue(IsFocusedProperty); }
			set { SetValue(IsFocusedProperty, value); }
		}

		/// <summary>
		/// The associated <see cref="Control.IsEnabledChanged"/> handler.
		/// </summary>
		/// <param name="sender">Associated <see cref="Control"/>.</param>
		/// <param name="args">Dependency property changed event data.</param>
		private static void OnAssociatedObjectEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			var control = (Control) sender;

			// element becomes enabled
			if ((bool) args.NewValue)
			{
				control.Focus();
				control.IsEnabledChanged -= OnAssociatedObjectEnabledChanged;
			}
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
			AssociatedObject.GotFocus += (sender, args) => IsFocused = true;
			AssociatedObject.LostFocus += (sender, args) => IsFocused = false;
			AssociatedObject.Loaded += (sender, args) =>
			                           	{
			                           		if (HasInitialFocus || IsFocused)
			                           		{
			                           			AssociatedObject.Focus();
			                           		}
			                           	};
			base.OnAttached();
		}

		#endregion
	}
}