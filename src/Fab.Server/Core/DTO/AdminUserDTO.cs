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
		#region Properties

		/// <summary>
		/// Gets or sets user unique login name.
		/// </summary>
		[DataMember]
		public string Login { get; set; }

		/// <summary>
		/// Gets or sets user password.
		/// Note: should be filled only by client.
		/// </summary>
		[DataMember]
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets user unique email.
		/// </summary>
		[DataMember]
		public string Email { get; set; }

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

		#region Only Server To Client

		/// <summary>
		/// Gets or sets user last access date.
		/// Note: should be filled only by service.
		/// </summary>
		[DataMember]
		public DateTime? LastAccess { get; set; }

		/// <summary>
		/// Gets or sets a date when user attributes was last changed.
		/// Note: should be filled only by service.
		/// </summary>
		[DataMember]
		public DateTime? DisabledChanged { get; set; }

		/// <summary>
		/// Gets or sets database file size in bytes.
		/// Note: should be filled only by service.
		/// </summary>
		[DataMember]
		public long? DatabaseSize { get; set; }

		/// <summary>
		/// Gets or sets free disk space in bytes, where database file located.
		/// Note: should be filled only by service.
		/// </summary>
		[DataMember]
		public long? FreeDiskSpaceAvailable { get; set; }

		#endregion

		#endregion

		#region .Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminUserDTO"/> class.
		/// </summary>
		public AdminUserDTO()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AdminUserDTO"/> class.
		/// </summary>
		/// <param name="userDTO">User instance to copy basic properties from.</param>
		public AdminUserDTO(UserDTO userDTO)
		{
			Id = userDTO.Id;
			Registered = userDTO.Registered;
			ServiceUrl = userDTO.ServiceUrl;
		}

		#endregion
	}
}