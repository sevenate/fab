//------------------------------------------------------------
// <copyright file="IShell.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using Caliburn.Micro;
using Fab.Client.Shell;
using Fab.Core.Framework;
using Fab.Managment.Shell.Messages;

namespace Fab.Managment.Shell
{
	/// <summary>
	/// General application shell contract.
	/// </summary>
	public interface IShell : IScreen, ICanBeBusy, IHandle<UserDeletedMessage>, IHandle<ApplicationErrorMessage>
	{
	}
}