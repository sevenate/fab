//------------------------------------------------------------
// <copyright file="IModule.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Windows.Controls;
using Caliburn.Micro;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Contract for standalone application functionality group that could be presented on single "tab" screen.
	/// </summary>
	public interface IModule : IScreen
	{
		/// <summary>
		/// Gets module name.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets module icon.
		/// </summary>
		Control Icon { get; }

		/// <summary>
		/// Gets rder in UI representation (like position in ItemsControl.Items) for module.
		/// </summary>
		int Order { get; }

		/// <summary>
		/// Show module to user.
		/// </summary>
		void Show();
	}
}