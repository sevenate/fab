// <copyright file="PasswordBindBehavior.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-12-14" inspiredby="http://blog.functionalfun.net/2008/06/wpf-passwordbox-and-data-binding.html" />
// <summary>Allow to bind to the PasswordBox.Password value.</summary>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Fab.Client.Authentication
{
	/// <summary>
	/// Allow to bind to the PasswordBox.Password value.
	/// </summary>
	public class PasswordBindBehavior : Behavior<PasswordBox>
	{
		#region UpdatingPassword DP

		/// <summary>
		/// Dependency property registration for <see cref="UpdatingPassword"/>.
		/// </summary>
		private static readonly DependencyProperty UpdatingPasswordProperty = DependencyProperty.RegisterAttached(
			"UpdatingPassword",
			typeof (bool),
			typeof (PasswordBindBehavior),
			new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value indicating whether a password is updating by the user interaction.
		/// This is the dependency property.
		/// Default value is <c>false</c>.
		/// </summary>
		private bool UpdatingPassword
		{
			get { return (bool) GetValue(UpdatingPasswordProperty); }
			set { SetValue(UpdatingPasswordProperty, value); }
		}

		#endregion

		#region BoundPassword DP

		/// <summary>
		/// Dependency property registration of the <see cref="BoundPassword"/>.
		/// </summary>
		public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
				"BoundPassword",
				typeof(string),
				typeof(PasswordBindBehavior),
				new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

		/// <summary>
		/// Gets or sets bind-enabled value of the PasswordBox.Password property.
		/// This is the dependency property.
		/// Default value is <c>string.Empty</c>.
		/// </summary>
		public string BoundPassword
		{
			get { return (string)GetValue(BoundPasswordProperty); }
			set { SetValue(BoundPasswordProperty, value); }
		}

		/// <summary>
		/// Handle <see cref="BoundPassword"/> "changed" event.
		/// </summary>
		/// <param name="d">The password box which attached <see cref="BoundPassword"/> was changed.</param>
		/// <param name="e">Event data.</param>
		private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var passwordBindBehavior = (PasswordBindBehavior)d;

			// prevent recursive updating
			if (passwordBindBehavior.UpdatingPassword)
			{
				return;
			}

			// update password if it was set from external source
			passwordBindBehavior.AssociatedObject.Password = (string)e.NewValue ?? string.Empty;
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
			AssociatedObject.PasswordChanged += OnPasswordChanged;
		}

		/// <summary>
		/// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
		/// </summary>
		/// <remarks>
		/// Override this to unhook functionality from the AssociatedObject.
		/// </remarks>
		protected override void OnDetaching()
		{
			AssociatedObject.PasswordChanged -= OnPasswordChanged;
		}

		/// <summary>
		/// Handle <see cref="PasswordBox.PasswordChanged"/> event.
		/// </summary>
		/// <param name="sender">Password box with user password.</param>
		/// <param name="routedEventArgs">Event data.</param>
		private void OnPasswordChanged(object sender, RoutedEventArgs routedEventArgs)
		{
			// set a flag to indicate that we're updating the password
			UpdatingPassword = true;

			BoundPassword = AssociatedObject.Password;

			// clear "updating" flag
			UpdatingPassword = false;
		}

		#endregion
	}
}