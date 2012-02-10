//------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Fab.Server.Core
{
	/// <summary>
	/// Contains all custom string extension methods.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Create SHA1 hash from the string.
		/// </summary>
		/// <param name="password">Input string.</param>
		/// <returns>Output SHA1 hash of the string.</returns>
		public static string Hash(this string password)
		{
			var provider = new SHA1CryptoServiceProvider();
			byte[] data = Encoding.ASCII.GetBytes(password);
			data = provider.ComputeHash(data);
			return Encoding.ASCII.GetString(data);
		}
	}
}