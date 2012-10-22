
//------------------------------------------------------------
// <copyright file="ObjectExtensionsTests.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Xunit;

namespace Fab.Core.Tests
{
	/// <summary>
	/// Unit tests for <see cref="HashExtensions"/> class.
	/// </summary>
	public class ObjectExtensionsTests
	{
		private const int HashGenerationIterationsCount = 100000;

		[Fact]
// ReSharper disable InconsistentNaming
		public void Check_Generated_Hash_Uniqueness()
// ReSharper restore InconsistentNaming
		{
			IList<string> cache = new List<string>();
			
			for (int i = 0; i < HashGenerationIterationsCount; i++)
			{
				string hash = Guid.NewGuid().Hash36();
				
				if (!cache.Contains(hash))
				{
					cache.Add(hash);
				}
				else
				{
					Assert.True(true, string.Format("Hash '{0}' was already generated early on {1} trial; current trial is {2}.", hash, cache.IndexOf(hash), i));
				}
			}
		}
	}
}