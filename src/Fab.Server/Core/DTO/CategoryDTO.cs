// <copyright file="CategoryDTO.cs" company="HD">
// 	Copyright (c) 2009-2010 nReez. All rights reserved.
// </copyright>
// <author name="Andrew Levshoff" email="78@nreez.com" date="2010-06-28" />
// <summary>Category data transfer object.</summary>

using System.Runtime.Serialization;
using Fab.Server.Core.Enums;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Category data transfer object.
	/// </summary>
	[DataContract]
	public class CategoryDTO
	{
		/// <summary>
		/// Gets or sets category unique ID.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets category name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets category name.
		/// </summary>
		[DataMember]
		public CategoryType CategoryType { get; set; }

		/// <summary>
		/// Gets or sets a value indicating how many times this category was used.
		/// </summary>
		[DataMember]
		public int Popularity { get; set; }
	}
}