//------------------------------------------------------------
// <copyright file="UserDTO.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// User data transfer object.
	/// </summary>
	[DataContract]
	public class UserDTO
	{
		/// <summary>
		/// Gets or sets user unique ID.
		/// </summary>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets user registration date.
		/// </summary>
		[DataMember]
		public DateTime Registered { get; set; }

		/// <summary>
		/// Gets or sets service api url for the client app to this particular user.
		/// </summary>
		[DataMember]
		public string ServiceUrl { get; set; }
	}
}