//------------------------------------------------------------
// <copyright file="AdminUserDTO.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Admin-only user data transfer object.
	/// </summary>
	[DataContract]
	public class AdminUserDTO : UserDTO
	{
		/// <summary>
		/// Gets or sets user unique login name.
		/// </summary>
		[DataMember]
		public string Login { get; set; }

		/// <summary>
		/// Gets or sets user password.
		/// </summary>
		[DataMember]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets user unique email.
		/// </summary>
		[DataMember]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets user last access date.
		/// </summary>
		[DataMember]
		public DateTime? LastAccess { get; set; }

		/// <summary>
		/// Gets or sets path to the user personal database.
		/// </summary>
		[DataMember]
		public string DatabasePath { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether a user is disabled.
		/// </summary>
		[DataMember]
		public bool IsDisabled { get; set; }

		/// <summary>
		/// Gets or sets a date when <see cref="IsDisabled"/> value was last changed.
		/// </summary>
		[DataMember]
		public DateTime? DisabledChanged { get; set; }
	}
}