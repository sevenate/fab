// <copyright file="EnumWrapper.cs" company="nReez">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrey Levshov" email="78@nreez.com" date="2011-02-10" />

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Fab.Client.Framework
{
	/// <summary>
	/// Enumeration wrapper for binding.
	/// </summary>
	public class EnumWrapper
	{
		#region Bindable Properties

		/// <summary>
		/// Gets or sets original enumeration value.
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Gets or sets underlying integer value.
		/// </summary>
		public int IntegerValue { get; set; }

		/// <summary>
		/// Gets or sets friendly description for enumeration value.
		/// </summary>
		public string Description { get; set; }

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current <see cref="EnumWrapper"/>.
		/// </summary>
		/// <returns>A <see cref="string"/> that represents the current <see cref="EnumWrapper"/>.</returns>
		public override string ToString()
		{
			return Description;
		}

		/// <summary>
		/// Determines whether the specified <see cref="EnumWrapper"/> is equal to the current <see cref="EnumWrapper"/>.
		/// </summary>
		/// <param name="obj">The <see cref="EnumWrapper"/> to compare with the current <see cref="EnumWrapper"/>.</param>
		/// <returns>true if the specified <see cref="EnumWrapper"/> is equal to the current <see cref="EnumWrapper"/>; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			var wrapper = obj as EnumWrapper;

			if (wrapper != null)
			{
				return IntegerValue == wrapper.IntegerValue;
			}

			if (obj is int)
			{
				return IntegerValue.Equals((int) obj);
			}

			return false;
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for the current <see cref="EnumWrapper"/>.</returns>
		public override int GetHashCode()
		{
			return IntegerValue.GetHashCode();
		}

		#endregion
	}

	/// <summary>
	/// The enumeration value for binding.
	/// </summary>
	/// <typeparam name="T">Enumeration type.</typeparam>
	public class EnumWrapper<T> : EnumWrapper where T : struct
	{
		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumWrapper{T}"/> class.
		/// </summary>
		/// <param name="initValue">Initial strong typed value.</param>
		public EnumWrapper(T initValue)
		{
			var type = typeof(T);

			if (!type.IsEnum)
			{
				throw new ArgumentException("This class only supports enumeration types.");
			}

			var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

			var field = fields.Where(f => f.Name == initValue.ToString()).Single();

			EnumValue = initValue;
			Value = field.GetValue(null);
			IntegerValue = (int)field.GetValue(null);

			var att = field.GetCustomAttributes(typeof(DescriptionAttribute), false)
							   .OfType<DescriptionAttribute>()
							   .FirstOrDefault();

			Description = att != null
			              	? att.Description
			              	: field.Name;
		}

		#endregion

		#region Bindable Property

		/// <summary>
		/// Gets original strong typed value.
		/// </summary>
		public T EnumValue { get; private set; }

		#endregion
	}
}