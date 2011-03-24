// <copyright file="EnumWrapper.cs" company="HD">
// 	Copyright (c) 2009-2011 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2011-02-10" />
// <summary>Enumeration wrapper for binding.</summary>

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
}