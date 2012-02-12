//------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;
using Caliburn.Micro;
using Fab.Client.Framework.Filters;
using Fab.Client.Shell;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Framework;
using Fab.Managment.Framework.Results;
using Fab.Managment.Shell.Messages;
using Fab.Managment.Shell.Results;

namespace Fab.Managment.Shell
{
	[Export(typeof(IShell))]
	public class ShellViewModel : Conductor<IScreen>, IShell
	{
		#region Autorization

		private string password = "123";
		private string username = "admin";

		public string EndPointAddress
		{
			get { return Helpers.EndPointAddress; }

			set
			{
				Helpers.EndPointAddress = value;
				NotifyOfPropertyChange(() => EndPointAddress);
			}
		}

		public string Username
		{
			get { return username; }
			set
			{
				if (username != value)
				{
					username = value;
					Helpers.Username = username;
					NotifyOfPropertyChange(() => Username);
				}
			}
		}

		public string Password
		{
			get { return password; }
			set
			{
				password = value;
				Helpers.Password = password;
				NotifyOfPropertyChange(() => Password);
			}
		}

		#endregion

		#region Paging

		private int currentPageIndex;
		private int pageSize;
		private int pagesCount;
		private int totalUsers;

		public int TotalUsers
		{
			get { return totalUsers; }
			set
			{
				totalUsers = value;
				NotifyOfPropertyChange(() => TotalUsers);
			}
		}

		private long totalUsedSpace;

		public long TotalUsedSpace
		{
			get { return totalUsedSpace; }
			set
			{
				totalUsedSpace = value;
				NotifyOfPropertyChange(() => TotalUsedSpace);
			}
		}

		public int PageSize
		{
			get { return pageSize; }

			set
			{
				pageSize = value;
				NotifyOfPropertyChange(() => PageSize);
			}
		}

		public int PagesCount
		{
			get { return pagesCount; }

			set
			{
				pagesCount = value;
				NotifyOfPropertyChange(() => PagesCount);
			}
		}

		public int CurrentPageIndex
		{
			get { return currentPageIndex; }

			set
			{
				currentPageIndex = value;
				NotifyOfPropertyChange(() => CurrentPageIndex);
				NotifyOfPropertyChange(() => CanNextPage);
				NotifyOfPropertyChange(() => CanPrevPage);
			}
		}

		public IEnumerable<IResult> NextPage()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					CurrentPageIndex++;
				}
			};

			yield return new SequentialResult(LoadUsers().GetEnumerator());

			IsBusy = false;
		}

		public bool CanNextPage
		{
			get { return CurrentPageIndex < PagesCount; }
		}

		public IEnumerable<IResult> PrevPage()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					IsBusy = true;
					CurrentPageIndex--;
				}
			};

			yield return new SequentialResult(LoadUsers().GetEnumerator());

			IsBusy = false;
		}

		public bool CanPrevPage
		{
			get { return 1 < CurrentPageIndex; }
		}

		#endregion

		#region Search

		private string loadText = "Load";
		private DateTime notOlderThen = DateTime.Now.AddMonths(-1);
		private string searchText;
		private DateTime upTo = DateTime.Now;
		private bool useEndDate;
		private bool useStartDate;

		public bool UseStartDate
		{
			get { return useStartDate; }

			set
			{
				useStartDate = value;
				NotifyOfPropertyChange(() => UseStartDate);
			}
		}

		public bool UseEndDate
		{
			get { return useEndDate; }

			set
			{
				useEndDate = value;
				NotifyOfPropertyChange(() => UseEndDate);
			}
		}

		public DateTime NotOlderThen
		{
			get { return notOlderThen; }

			set
			{
				notOlderThen = value;
				NotifyOfPropertyChange(() => NotOlderThen);
			}
		}

		public DateTime UpTo
		{
			get { return upTo; }

			set
			{
				upTo = value;
				NotifyOfPropertyChange(() => UpTo);
			}
		}

		public string SearchText
		{
			get { return searchText; }

			set
			{
				searchText = value;
				NotifyOfPropertyChange(() => SearchText);
			}
		}

		public string LoadText
		{
			get { return loadText; }

			set
			{
				loadText = value;
				NotifyOfPropertyChange(() => LoadText);
			}
		}

		public IObservableCollection<UserViewModel> Users { get; private set; }

		public IEnumerable<IResult> Load()
		{
			yield return new SingleResult
			             	{
			             		Action = () =>
			             		         	{
												IsBusy = true;
												LoadText = "Counting...";
												CurrentPageIndex = 0;
												PagesCount = 0;
			             		         	}
			             	};

			var countResult = new CountUsersResult { Filer = CreateFilter() };
			yield return countResult;

			if (countResult.Count >= 1)
			{
				yield return new SingleResult
				{
					Action = () =>
					{
						TotalUsers = countResult.Count;
						PagesCount = countResult.Count / PageSize + (countResult.Count % PageSize > 0 ? 1 : 0);
						CurrentPageIndex = 1;
					}
				};

				yield return new SequentialResult(LoadUsers().GetEnumerator());
			}
			else
			{
				LoadText = "Load";
			}

			IsBusy = false;
		}

		private IEnumerable<IResult> LoadUsers()
		{
			yield return new SingleResult
			{
				Action = () =>
				{
					LoadText = "Loading...";
					TotalUsedSpace = 0;
				}
			};

			var loadResult = new LoadResult {Filer = CreateFilter(PageSize)};
			yield return loadResult;

			Users.Clear();

			long usedSpace = 0;

			foreach (AdminUserDTO adminUserDto in loadResult.Users)
			{
				usedSpace += adminUserDto.DatabaseSize ?? 0;
				Users.Add(MapToViewModel(adminUserDto));
			}

			TotalUsedSpace = usedSpace;
			LoadText = "Load";
		}

		private static UserViewModel MapToViewModel(AdminUserDTO adminUserDto)
		{
			return new UserViewModel
			       	{
			       		Id = adminUserDto.Id,
			       		Login = adminUserDto.Login,
			       		Registered = adminUserDto.Registered,
			       		LastAccess = adminUserDto.LastAccess,
			       		DatabaseSize = adminUserDto.DatabaseSize,
			       		DatabasePath = adminUserDto.DatabasePath,
			       		DisabledChanged = adminUserDto.DisabledChanged,
			       		Email = adminUserDto.Email,
			       		FreeDiskSpaceAvailable = adminUserDto.FreeDiskSpaceAvailable,
			       		IsDisabled = adminUserDto.IsDisabled,
			       		ServiceUrl = adminUserDto.ServiceUrl
			       	};
		}

		public void ClearSearch()
		{
			UseStartDate = false;
			UseEndDate = false;
			SearchText = string.Empty;
			TotalUsers = 0;
			TotalUsedSpace = 0;
			PagesCount = 0;
			CurrentPageIndex = 0;
			Users.Clear();
		}

		private TextSearchFilter CreateFilter(int? usersPerPage = null)
		{
			var filter = new TextSearchFilter
			             	{
			             		NotOlderThen = UseStartDate
			             		               	? NotOlderThen.ToUniversalTime()
			             		               	: (DateTime?) null,
			             		Upto = UseEndDate
			             		       	? UpTo.ToUniversalTime()
			             		       	: (DateTime?) null,
			             		Contains = !string.IsNullOrEmpty(SearchText)
			             		           	? SearchText
			             		           	: null,
			             		Take = usersPerPage,
			             		Skip = usersPerPage != null ? (CurrentPageIndex - 1)*usersPerPage : null,
			             	};
			return filter;
		}

		#endregion

		#region Misc

		private string adminPassword;

		private string hash;

		public string AdminPassword
		{
			get { return adminPassword; }
			set
			{
				adminPassword = value;
				NotifyOfPropertyChange(() => AdminPassword);
				NotifyOfPropertyChange(() => CanGenerateHash);
			}
		}

		public string Hash
		{
			get { return hash; }
			set
			{
				hash = value;
				NotifyOfPropertyChange(() => Hash);
			}
		}

		//TODO: find out why DependenciesAttribute is not woring
		[Dependencies("AdminPassword")]
		public void GenerateHash()
		{
			Hash = FormsAuthentication.HashPasswordForStoringInConfigFile(AdminPassword, FormsAuthPasswordFormat.SHA1.ToString());
		}

		public bool CanGenerateHash
		{
			get { return !string.IsNullOrWhiteSpace(AdminPassword); }
		}

		#endregion

		#region .Ctor

		/// <summary>
		/// Creates an instance of the screen.
		/// </summary>
		[ImportingConstructor]
		public ShellViewModel(IEventAggregator aggregator)
		{
			Helpers.EndPointAddress = EndPointAddress;
			Helpers.Username = Username;
			Helpers.Password = Password;
			Users = new BindableCollection<UserViewModel>();

			aggregator.Subscribe(this);
		}

		#endregion

		#region Error Handling

		private ErrorDialogViewModel modalDialog;
		public ErrorDialogViewModel ModalDialog
		{
			get { return modalDialog; }

			set
			{
				modalDialog = value;
				NotifyOfPropertyChange(() => ModalDialog);
			}
		}

		#endregion

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

		#region Implementation of IHandle<UserDeletedMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(UserDeletedMessage message)
		{
			Users.Remove(message.User);
			TotalUsers--;
			TotalUsedSpace = Users.Sum(model => model.DatabaseSize ?? 0);
		}

		#endregion

		#region Implementation of IHandle<ApplicationErrorMessage>

		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Handle(ApplicationErrorMessage message)
		{
			var errorDialog = IoC.Get<ErrorDialogViewModel>();
			errorDialog.DisplayName = "Application Error";
			errorDialog.Error = message.Error.ToString();
			ActivateItem(errorDialog);
		}

		#endregion
	}
}