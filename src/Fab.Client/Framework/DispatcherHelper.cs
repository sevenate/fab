//------------------------------------------------------------
// <copyright file="DispatcherHelper.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Windows;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Holds some UI/Background related helper functions.
	/// </summary>
	public static class DispatcherHelper
	{
		/// <summary>
		/// Invoke empty delegate on UI thread with low priority to force update binding with higher priority.
		/// </summary>
		public static void ForceBindingUpdate()
		{
#if SILVERLIGHT
			Application.Current.RootVisual.Dispatcher.BeginInvoke(delegate { });
#else
			Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate { }));
#endif
		}
	}
}
