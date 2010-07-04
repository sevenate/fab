// <copyright file="Utility.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-07-04" />
// <summary>Common helper methods.</summary>

using System;
using System.Reflection;

namespace Fab.Client.Common
{
	/// <summary>
	/// Common helper methods.
	/// </summary>
	public class Utility
	{
		/// <summary>
		/// Return assembly version.
		/// </summary>
		/// <param name="assembly">Assembly to get version from.</param>
		/// <returns>Full version number.</returns>
		internal static Version GetAssemblyVersion(Assembly assembly)
		{
			var assemblyName = new AssemblyName(assembly.FullName);
			return assemblyName.Version;
		}

		/// <summary>
		/// Gets current application version.
		/// </summary>
		internal static string AppVersion
		{
			get
			{
				return GetAssemblyVersion(Assembly.GetExecutingAssembly()).ToString();
			}
		}
	}
}