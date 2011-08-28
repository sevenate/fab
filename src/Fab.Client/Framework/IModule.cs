// <copyright file="ITestItem.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-03-31" />

using Caliburn.Micro;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Screen part service.
	/// </summary>
	public interface IModule : IScreen
	{
		string Name { get; }
		void Show();
	}
}