//------------------------------------------------------------
// <copyright file="SelectTextOnFocusBehavior.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Fab.Client.Framework.Behaviors
{
	/// <summary>
	/// Select all text in <see cref="TextBox"/> on focus.
	/// </summary>
	public class SelectTextOnFocusBehavior : Behavior<TextBox>
	{
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
			AssociatedObject.GotFocus += OnGotFocus;
		}

		/// <summary>
		/// Handle <see cref="TextBox.GotFocus"/> event.
		/// </summary>
		/// <param name="sender">Focused text box element.</param>
		/// <param name="e">Event data.</param>
		private void OnGotFocus(object sender, RoutedEventArgs e)
		{
			AssociatedObject.SelectAll();
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
			AssociatedObject.GotFocus -= OnGotFocus;
		}

		#endregion
	}
}