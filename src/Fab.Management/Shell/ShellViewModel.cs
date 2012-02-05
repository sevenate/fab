//------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="nReez">
// 	Copyright (c) 2012 nReez. All rights reserved.
// </copyright>
//------------------------------------------------------------

using System;
using System.Web.Configuration;
using System.Web.Security;
using Caliburn.Micro;
using Fab.Client.Framework.Filters;
using Fab.Core.Framework;
using Fab.Managment.AdminServiceReference;
using Fab.Managment.Framework;

namespace Fab.Managment.Shell
{
	// ReSharper disable ClassNeverInstantiated.Global
	public class ShellViewModel : Screen, ICanBeBusy
		// ReSharper restore ClassNeverInstantiated.Global
	{
		private AdminServiceClient adminService;

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
				Helpers.Password = username;
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
			}
		}

		public void NextPage()
		{
			currentPageIndex++;

			IsBusy = true;
			LoadText = "Loadig...";

			TextSearchFilter filter = CreateFilter(PageSize);

			adminService = Helpers.CreateClientProxy(EndPointAddress, Username, Password);

			adminService.GetUsersCompleted += LoadUserCompleted;
			adminService.GetUsersAsync(filter);
		}

		public void PrevPage()
		{
			CurrentPageIndex--;

			IsBusy = true;
			LoadText = "Loadig...";

			TextSearchFilter filter = CreateFilter(PageSize);

			adminService = Helpers.CreateClientProxy(EndPointAddress, Username, Password);
			adminService.GetUsersCompleted += LoadUserCompleted;
			adminService.GetUsersAsync(filter);
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

		public void Load()
		{
			IsBusy = true;
			LoadText = "Loading...";

			// Reset curent page index
			currentPageIndex = 1;

			adminService = Helpers.CreateClientProxy(EndPointAddress, Username, Password);

			adminService.GetUsersCountCompleted += CountUsersCompleted;
			adminService.GetUsersCountAsync(CreateFilter());

			adminService.GetUsersCompleted += LoadUserCompleted;
			adminService.GetUsersAsync(CreateFilter(PageSize));
		}

		private void CountUsersCompleted(object sender, GetUsersCountCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				TotalUsers = args.Result;
				PagesCount = args.Result/PageSize + args.Result%PageSize > 0 ? 1 : 0;
				CurrentPageIndex = PagesCount > 0 ? 1 : 0;
			}
			else
			{
				Helpers.ErrorProcessing(args);
			}
		}

		private void LoadUserCompleted(object sender, GetUsersCompletedEventArgs args)
		{
			if (args.Error == null)
			{
				adminService.Close();
				Users.Clear();

				foreach (AdminUserDTO adminUserDto in args.Result)
				{
					Users.Add(new UserViewModel
					          	{
					          		Id = adminUserDto.Id,
					          		Login = adminUserDto.Login,
					          		Registered = adminUserDto.Registered,
					          		LastAccess = adminUserDto.LastAccess,
					          		DatabaseSize = adminUserDto.DatabaseSize
					          	});
				}
			}
			else
			{
				Helpers.ErrorProcessing(args);
			}

			LoadText = "Load";
			IsBusy = false;
		}

		public void ClearSearch()
		{
			UseStartDate = false;
			UseEndDate = false;
			SearchText = string.Empty;
			Users.Clear();
		}

		private TextSearchFilter CreateFilter(int? pageSize = null)
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
			             		Take = pageSize,
			             		Skip = pageSize != null ? (currentPageIndex - 1)*pageSize : null,
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

		/*
		public bool CanGenerateHash()
		{
			return !string.IsNullOrWhiteSpace(AdminPassword);
		}
		*/

		#endregion

		#region .Ctor

		/// <summary>
		/// Creates an instance of the screen.
		/// </summary>
		public ShellViewModel()
		{
			Helpers.EndPointAddress = EndPointAddress;
			Helpers.Username = Username;
			Helpers.Password = Password;
			Users = new BindableCollection<UserViewModel>();
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
	}
}