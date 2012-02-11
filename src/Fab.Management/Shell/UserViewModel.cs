//------------------------------------------------------------
// <copyright file="UserViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Core.Framework;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Framework.Results;
using Fab.Managment.Shell.Messages;
using Fab.Managment.Shell.Results;

namespace Fab.Managment.Shell
{
	public class UserViewModel : Screen, ICanBeBusy
	{
		private readonly IEventAggregator aggregator = IoC.Get<IEventAggregator>();

		#region Implementation of ICanBeBusy

		/// <summary>
		/// Indicate whether view-model is busy by some background operation.
		/// </summary>
		private bool isBusy;

		/// <summary>
		/// Gets or sets a value indicating whether view-model is busy by some background operation.
		/// </summary>
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				NotifyOfPropertyChange(() => IsBusy);
			}
		}

		#endregion

		#region Id

		private Guid id;
		public Guid Id
		{
			get { return id; }
			set
			{
				id = value;
				NotifyOfPropertyChange(() => Id);
			}
		}

		#endregion

		#region Login

		private string login;
		public string Login
		{
			get { return login; }
			set
			{
				login = value;
				NotifyOfPropertyChange(() => Login);
			}
		}

		#endregion

		#region Registered

		private DateTime registered;
		public DateTime Registered
		{
			get { return registered; }
			set
			{
				registered = value;
				NotifyOfPropertyChange(() => Registered);
			}
		}

		#endregion

		#region Last Access

		private DateTime? lastAccess;
		public DateTime? LastAccess
		{
			get { return lastAccess; }
			set
			{
				lastAccess = value;
				NotifyOfPropertyChange(() => LastAccess);
			}
		}

		#endregion

		#region Database Size

		private long? databaseSize;
		public long? DatabaseSize
		{
			get { return databaseSize; }
			set
			{
				databaseSize = value;
				NotifyOfPropertyChange(() => DatabaseSize);
			}
		}

		public IEnumerable<IResult> Optimize()
		{
			yield return new SingleResult
			             	{
			             		Action = () =>
			             		         	{
			             		         		IsBusy = true;
			             		         	}
			             	};
			
			var result = new OptimizeResult{Id = Id};
			yield return result;

			DatabaseSize = result.DatabaseSize;
			IsBusy = false;
		}

		#endregion

		#region Database Path

		private string databasePath;
		public string DatabasePath
		{
			get { return databasePath; }
			set
			{
				databasePath = value;
				NotifyOfPropertyChange(() => DatabasePath);
			}
		}

		#endregion

		#region Disabled Changed

		private DateTime? disabledChanged;
		
		/// <summary>
		/// Last Updated date.
		/// </summary>
		public DateTime? DisabledChanged
		{
			get { return disabledChanged; }
			set
			{
				disabledChanged = value;
				NotifyOfPropertyChange(() => DisabledChanged);
			}
		}

		#endregion

		#region Email

		private string email;
		public string Email
		{
			get { return email; }
			set
			{
				email = value;
				NotifyOfPropertyChange(() => Email);
			}
		}

		#endregion

		#region Password

		private string password;
		public string Password
		{
			get { return password; }
			set
			{
				password = value;
				NotifyOfPropertyChange(() => Password);
			}
		}

		#endregion

		#region Free Disk Space Available

		private long? freeDiskSpaceAvailable;
		public long? FreeDiskSpaceAvailable
		{
			get { return freeDiskSpaceAvailable; }
			set
			{
				freeDiskSpaceAvailable = value;
				NotifyOfPropertyChange(() => FreeDiskSpaceAvailable);
			}
		}

		#endregion

		#region Is Disabled

		private bool isDisabled;
		public bool IsDisabled
		{
			get { return isDisabled; }
			set
			{
				isDisabled = value;
				NotifyOfPropertyChange(() => IsDisabled);
			}
		}

		#endregion

		#region Service URL

		private string serviceUrl;
		public string ServiceUrl
		{
			get { return serviceUrl; }
			set
			{
				serviceUrl = value;
				NotifyOfPropertyChange(() => ServiceUrl);
			}
		}

		#endregion

		#region Repair

		private string repairStatus = "Repair DB";
		public string RepairStatus
		{
			get { return repairStatus; }
			set
			{
				repairStatus = value;
				NotifyOfPropertyChange(() => RepairStatus);
			}
		}

		public IEnumerable<IResult> Repair()
		{
			yield return new SingleResult
			             	{
			             		Action = () =>
			             		         	{
			             		         		IsBusy = true;
			             		         		RepairStatus = "Repairing...";
			             		         	}
			             	};

			var result = new RepairResult { Id = Id };
			yield return result;

			DatabaseSize = result.DatabaseSize;
			RepairStatus = "Repair DB";
			IsBusy = false;
		}

		#endregion

		#region Verify

		private string verifyStatus = "Verify DB";
		public string VerifyStatus
		{
			get { return verifyStatus; }
			set
			{
				verifyStatus = value;
				NotifyOfPropertyChange(() => VerifyStatus);
			}
		}

		public IEnumerable<IResult> Verify()
		{
			yield return new SingleResult
			             	{
			             		Action = () =>
			             		         	{
			             		         		IsBusy = true;
			             		         		VerifyStatus = "Verifing...";
			             		         	}
			             	};

			var result = new VerifyDbResult { Id = Id };
			yield return result;

			if (result.Success)
			{
			}

			VerifyStatus = "Verify DB";
			IsBusy = false;
		}

		#endregion

		#region Check Accounts

		private string checkCacheStatus = "Check Cache";
		public string CheckCacheStatus
		{
			get { return checkCacheStatus; }
			set
			{
				checkCacheStatus = value;
				NotifyOfPropertyChange(() => CheckCacheStatus);
			}
		}

		private bool updateCache;
		public bool UpdateCache
		{
			get { return updateCache; }
			set
			{
				updateCache = value;
				NotifyOfPropertyChange(() => UpdateCache);
			}
		}

		public IEnumerable<IResult> CheckCache()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					CheckCacheStatus = "Checking...";
				}
			};

			var result = new CheckAccountsCachedValuesResult
			             	{
			             		Id = Id,
								UpdateCachedValues = UpdateCache
			             	};
			yield return result;

			var a = result.Accounts;
			CheckCacheStatus = "Check Cache";
			IsBusy = false;
		}

		#endregion

		#region Save

		private string saveStatus = "Save";
		public string SaveStatus
		{
			get { return saveStatus; }
			set
			{
				saveStatus = value;
				NotifyOfPropertyChange(() => SaveStatus);
			}
		}

		public IEnumerable<IResult> Save()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					SaveStatus = "Saving...";
				}
			};

			var result = new SaveResult { User = MapToDTO(this)};
			yield return result;

			if (result.LastUpdated.HasValue)
			{
				DisabledChanged = result.LastUpdated;
			}

			SaveStatus = "Save";
			IsBusy = false;
		}

		#endregion

		public IEnumerable<IResult> Delete()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
				}
			};

			var result = new DeleteResult {Id = Id};
			yield return result;

			if (result.Success)
			{
				aggregator.Publish(new UserDeletedMessage { User = this });
			}

			IsBusy = false;
		}

		private static AdminUserDTO MapToDTO(UserViewModel userViewModel)
		{
			return new AdminUserDTO
			       	{
			       		Id = userViewModel.Id,
			       		Login = userViewModel.Login,
			       		Password = userViewModel.Password,
			       		DatabasePath = userViewModel.DatabasePath,
			       		ServiceUrl = userViewModel.ServiceUrl,
			       		IsDisabled = userViewModel.IsDisabled,
			       		Email = userViewModel.Email,
			       	};
		}
	}
}