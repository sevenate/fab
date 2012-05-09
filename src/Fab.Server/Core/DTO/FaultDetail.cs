//------------------------------------------------------------
// <copyright file="FaultDetail.cs" company="nReez">
// 	Copyright (c) 2011 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System.Runtime.Serialization;

namespace Fab.Server.Core.DTO
{
	/// <summary>
	/// Represent common public fault message.
	/// </summary>
	[DataContract]
	public class FaultDetail
	{
		#region Properties

		/// <summary>
		/// Gets or sets error code.
		/// </summary>
		[DataMember(Order = 0)]
		public string ErrorCode { get; set; }

		/// <summary>
		/// Gets or sets error message.
		/// </summary>
		[DataMember(Order = 1)]
		public string ErrorMessage { get; set; }

		/// <summary>
		/// Gets or sets error description.
		/// </summary>
		[DataMember(Order = 2)]
		public string Description { get; set; }

		#endregion

		#region .Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="FaultDetail"/> class.
		/// </summary>
		public FaultDetail()
		{
			ErrorCode = "ERR-UNKN-0";
			ErrorMessage = "Service encountered an error.";
			Description = "Internal service error. Please, try again later.";
		}

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns a System.String that represents the current feedback.
		/// </summary>
		/// <returns>A System.String that represents the current feedback.</returns>
		public override string ToString()
		{
			return string.Format("Error {0}: {1}\nDescription: {2}", ErrorCode, ErrorMessage, Description);
		}

		#endregion
	}
}