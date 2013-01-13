// <copyright file="AssemblyExtensions.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-07-04" />
// <summary>Common helper methods.</summary>

using System;
using System.Reflection;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Assembly specific extensions.
	/// </summary>
	public static class AssemblyExtensions
	{
		/// <summary>
		/// Gets application version.
		/// </summary>
		public static string AppVersion
		{
			get
			{
				string version = Assembly.GetExecutingAssembly().GetVersion().ToString();
#if DEBUG
				return version + " Debug";
#else
				return version + " Release";
#endif
			}
		}

		/// <summary>
		/// Gets assembly version.
		/// </summary>
		/// <param name = "assembly">Assembly to get version from.</param>
		/// <returns>Full version number.</returns>
		private static Version GetVersion(this Assembly assembly)
		{
			return new AssemblyName(assembly.FullName).Version;
		}
	}
}