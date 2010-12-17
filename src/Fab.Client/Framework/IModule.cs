// <copyright file="ITestItem.cs" company="HD">
// 	Copyright (c) 2010 HD. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-03-31</date>
// </author>
// <editor name="Andrew Levshoff">
// 	<email>alevshoff@hd.com</email>
// 	<date>2010-03-31</date>
// </editor>
// <summary>Test item interface.</summary>

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