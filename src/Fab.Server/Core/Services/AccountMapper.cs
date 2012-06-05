using System;
using EmitMapper.MappingConfiguration;
using Fab.Core;
using Fab.Server.Core.DTO;

namespace Fab.Server.Core.Services
{
	/// <summary>
	/// Map database account entity to data transfer object.
	/// </summary>
	public static class AccountMapper
	{
		/// <summary>
		/// Gets default mapping between <see cref="Account"/> and <see cref="AccountDTO"/>.
		/// </summary>
		public static DefaultMapConfig AccountMappingConfigurator
		{
			get
			{
				return new DefaultMapConfig()
					.MatchMembers((m1, m2) => m1 == m2 || m1 + "Id" == m2)
					.ConvertUsing<AssetType, int>(type => type.Id)
					.ConvertUsing<DateTime, DateTime>(type => type.IsUtc());
//						.ConvertUsing<DateTime?, DateTime?>(type => type.HasValue
//						                                            	? type.Value.IsUtc()
//						                                            	: (DateTime?) null)
//						.PostProcess<AccountDTO>((value, state) =>
//						                         {
//													 if (value.FirstPostingDate.HasValue)
//													 {
//														 value.FirstPostingDate = value.FirstPostingDate.Value.IsUtc();
//													 }
//
//													 if (value.LastPostingDate.HasValue)
//													 {
//														 value.LastPostingDate = value.LastPostingDate.Value.IsUtc();
//													 }
//
//						                         	return value;
//						                         })
			}
		}

		/// <summary>
		/// Workaround for "DateTime?" properties mapping between <see cref="Account"/> and <see cref="AccountDTO" />
		/// </summary>
		/// <param name="account">Account data transfer object for post processing.</param>
		/// <returns>Modified source account DTO.</returns>
		public static AccountDTO SetUTCforAccountNullableDates(AccountDTO account)
		{
			if (account.FirstPostingDate.HasValue)
			{
				account.FirstPostingDate = account.FirstPostingDate.Value.IsUtc();
			}

			if (account.LastPostingDate.HasValue)
			{
				account.LastPostingDate = account.LastPostingDate.Value.IsUtc();
			}

			return account;
		}
	}
}