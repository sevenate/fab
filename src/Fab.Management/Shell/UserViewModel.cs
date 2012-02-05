using System;
using System.Collections.Generic;
using Caliburn.Micro;
using Fab.Core.Framework;
using Fab.Managment.Shell.Results;

namespace Fab.Managment.Shell
{
	public class UserViewModel : Screen, ICanBeBusy
	{
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

		private DateTime? disabledChanged;
		public DateTime? DisabledChanged
		{
			get { return disabledChanged; }
			set
			{
				disabledChanged = value;
				NotifyOfPropertyChange(() => DisabledChanged);
			}
		}

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

		public IEnumerable<IResult> Optimize()
		{
			IsBusy = true;
			var optimizeResult = new OptimizeResult{Id = Id};
			yield return optimizeResult;

			DatabaseSize = optimizeResult.DatabaseSize;
			IsBusy = false;
		}

		public IEnumerable<IResult> Delete()
		{
			IsBusy = true;
			var deleteResult = new DeleteResult {Id = Id};
			yield return deleteResult;

			IsBusy = false;
		}
	}
}