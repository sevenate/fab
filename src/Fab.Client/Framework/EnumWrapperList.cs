// <copyright file="EnumWrapperList.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-02-10" />
// <summary>The collection of all enumeration values for binding.</summary>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace Fab.Client.Framework
{
	/// <summary>
	/// The collection of all enumeration values for binding.
	/// </summary>
	/// <typeparam name="T">Enumeration type.</typeparam>
	public class EnumWrapperList<T> : List<EnumWrapper> where T : struct
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumWrapperList{T}"/> class.
		/// </summary>
		public EnumWrapperList()
		{
			var type = typeof (T);

			if (!type.IsEnum)
			{
				throw new ArgumentException("This class only supports enumeration types.");
			}

			var fields = typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public);

			foreach (var field in fields)
			{
				var att = field.GetCustomAttributes(typeof (DescriptionAttribute), false)
							   .OfType<DescriptionAttribute>()
							   .FirstOrDefault();

				var bindableEnum = new EnumWrapper
				                   	{
				                   		Value = field.GetValue(null),
				                   		IntegerValue = (int) field.GetValue(null),
				                   		Description = att != null
				                   		              	? att.Description
				                   		              	: field.Name
				                   	};

				Add(bindableEnum);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumWrapperList{T}"/> class.
		/// </summary>
		/// <param name="values">Allowed enumeration values from all possible.</param>
		public EnumWrapperList(params T[] values) : this()
		{
			// Remove all not specified in parameters enumeration values as "not allowed"
			var enumsToExclude = from bindableEnum in this
			               where !values.Contains((T) bindableEnum.Value)
			               select bindableEnum;

			enumsToExclude.Apply(x => Remove(x));
		}

		#endregion
	}
}