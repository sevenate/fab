//------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Windows;

namespace Fab.Client.Framework.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="FrameworkElement"/>.
	/// </summary>
	public static class FrameworkElementExtensions
	{
		/// <summary>
		/// Analog of WPF TryFindResource method.
		/// </summary>
		/// <param name="element">Element to start search from.</param>
		/// <param name="resourceKey">Resource key to find.</param>
		/// <returns>Resource if found or null otherwise.</returns>
		public static object TryFindResource(this FrameworkElement element, object resourceKey)
		{
			var currentElement = element;

			while (currentElement != null)
			{
				var resource = currentElement.Resources[resourceKey];
				
				if (resource != null)
				{
					return resource;
				}

				currentElement = currentElement.Parent as FrameworkElement;
			}

			return Application.Current.Resources[resourceKey];
		}
	}
}