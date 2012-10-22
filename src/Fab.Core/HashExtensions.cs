//------------------------------------------------------------
// <copyright file="HashExtensions.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fab.Core
{
	/// <summary>
	/// Handy methods for friendly hash generation.
	/// </summary>
	public static class HashExtensions
	{
		/// <summary>
		/// Split string into specific "chunk-sized" substrings.
		/// </summary>
		/// <param name="str">Input string to split.</param>
		/// <param name="chunkSize">Count of characters in each result chunk.</param>
		/// <returns>Substrings of sized no larger then <paramref name="chunkSize"/> each.</returns>
		private static IEnumerable<string> Split(this string str, int chunkSize)
		{
			return Enumerable.Range(0, str.Length / chunkSize)
							 .Select(i => str.Substring(i * chunkSize, chunkSize));
		}

		/// <summary>
		/// Generate short friendly string hash value in format: D1-8A-EC
		/// </summary>
		/// <param name="input">Input string to generate hash from.</param>
		/// <param name="groupBy">Group by specific interval characters.</param>
		/// <returns>Short friendly hash.</returns>
		public static string Hash36(this object input, int groupBy = 2)
		{
			long base10 = input.GetHashCode();
			string base36 = base10.ToBase36();

			if (base36[0] == '-')
			{
				base36 = base36.Remove(0,1);
			}

			var chunks = base36.Split(2);

			var sb = new StringBuilder();

			foreach (var chunk in chunks)
			{
				if (sb.Length > 0)
				{
					sb.Append('-');
				}

				sb.Append(chunk);
			}

			return sb.ToString();
		}

		 /// <summary>
		 /// Generate short friendly string hash value in format: D282-A80F
		 /// </summary>
		 /// <param name="input">Input string to generate hash from.</param>
		 /// <returns>Short friendly hash.</returns>
		 public static string Hash16(this object input)
		 {
			 int hashCode = input.GetHashCode();
			 string hexString = string.Format("{0:X8}", hashCode);
			 return hexString.Insert(4, "-");
		 }
	}
}