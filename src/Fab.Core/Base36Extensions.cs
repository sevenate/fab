//------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;

namespace Fab.Core
{
	/// <summary>
	/// Extend <see cref="long"/> and <see cref="string"/> types with methods
	/// for conversion to/from base-36 numbers encoding (with alphabet "0-9A-Z").
	/// </summary>
	public static class Base36Extensions
	{
		/// <summary>
		/// Converts the given decimal number to the numeral system with the
		/// specified radix (in the range [2, 36]).
		/// </summary>
		/// <param name="decimalNumber">The number to convert.</param>
		/// <param name="radix">The radix of the destination numeral system
		/// (in the range [2, 36]).</param>
		/// <returns></returns>
		public static string ToBase36(this long decimalNumber, int radix = 36)
		{
			const int bitsInLong = 64;
			const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			if (radix < 2 || radix > digits.Length)
			{
				throw new ArgumentException("The radix must be >= 2 and <= " + digits.Length);
			}

			if (decimalNumber == 0)
			{
				return "0";
			}

			int index = bitsInLong - 1;
			long currentNumber = Math.Abs(decimalNumber);
			var charArray = new char[bitsInLong];

			while (currentNumber != 0)
			{
				var remainder = (int)(currentNumber % radix);
				charArray[index--] = digits[remainder];
				currentNumber = currentNumber / radix;
			}

			var result = new string(charArray, index + 1, bitsInLong - index - 1);
			
			if (decimalNumber < 0)
			{
				result = "-" + result;
			}

			return result;
		}

		/// <summary>
		/// Converts the given number from the numeral system with the specified
		/// radix (in the range [2, 36]) to decimal numeral system.
		/// </summary>
		/// <param name="number">The arbitrary numeral system number to convert.</param>
		/// <param name="radix">The radix of the numeral system the given number
		/// is in (in the range [2, 36]).</param>
		/// <returns></returns>
		public static long ToBase10(this string number, int radix = 36)
		{
			const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			if (radix < 2 || radix > digits.Length)
			{
				throw new ArgumentException("The radix must be >= 2 and <= " + digits.Length);
			}

			if (String.IsNullOrEmpty(number))
			{
				return 0;
			}

			// Make sure the arbitrary numeral system number is in upper case
			number = number.ToUpperInvariant();

			long result = 0;
			long multiplier = 1;
			
			for (int i = number.Length - 1; i >= 0; i--)
			{
				char c = number[i];
				
				if (i == 0 && c == '-')
				{
					// This is the negative sign symbol
					result = -result;
					break;
				}

				int digit = digits.IndexOf(c);
				
				if (digit == -1)
				{
					throw new ArgumentException( "Invalid character in the arbitrary numeral system number", "number");
				}

				result += digit * multiplier;
				multiplier *= radix;
			}

			return result;
		}
	}
}