﻿using System;
using Xamarin.Forms;
using ApiClient.Models;
using ApiClient.Helpers;
using ApiClient.DataServices.Interfaces;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using ApiClient.Exceptions;
using ApiClient.Views;

namespace ApiClient.ViewModels
{
    public class UserListViewPageViewModel : BaseViewModel
	{
		#region private fields
		private IUserService _userService;
		private int page = 1;
		//private bool isNotBusy = true;
		private bool isRefreshing;
        //private bool isBusy;
		private Command refleshCommand;
		private Command<User> dataLoadCommand;
		private Command<User> userSelectedCommand;
		#endregion

		#region Constructor Area
        public UserListViewPageViewModel(IUserService userService)
		{
			_userService = userService;
            Task.Run(async () =>
                        {
                            IsRefreshing = true;

                            await LoadData();

                            IsRefreshing = false;
                        }
                    ).Wait();
		}
		#endregion

		#region Property area
		/// <summary>
		/// Get or set the "Users" property
		/// </summary>
		/// <value>User 리스트</value>
		public ObservableRangeCollection<User> Users { get; set; } = new ObservableRangeCollection<User>();

		/// <summary>
		/// Get or set the "IsRefreshing" property
		/// </summary>
		/// <value><c>trun</c> if this data is reloaded; otherwise, <c>false</c>.</value>
		public bool IsRefreshing
		{
			get { return isRefreshing; }
			set
			{
                SetProperty(ref isRefreshing, value);
                IsBusy = isRefreshing;
                //refleshCommand?.ChangeCanExecute();
   			}
		}

        //public override bool IsBusy
        //{
        //    get { return isBusy; }
        //    set{
        //        SetProperty(ref isBusy, value, "IsBusy", () => 
        //        {
        //            dataLoadCommand?.ChangeCanExecute();
        //        });
        //    }
        //}
		#endregion

		#region Command Area

		/// <summary>
		/// Get RefleshCommand
		/// </summary>
		/// <remarks>
		/// Pull to refresh 이벤트 처리용 
		/// </remarks>
		public Command RefleshCommand =>
								refleshCommand ?? (refleshCommand =
												   new Command
													(
														async () =>
														{
                                                            if (isRefreshing || IsBusy)
																return;

															IsRefreshing = true;

															page = 1;
															await LoadData();

															IsRefreshing = false;
														},
														() =>
														{
                                                            return !IsBusy;
														}
													));
		/// <summary>
		/// Get DataLoadCommand
		/// </summary>
		/// <remarks>
		/// ListView ItemAppearing 이벤트 처리용
		/// </remarks>
		public Command<User> DataLoadCommand =>
								 dataLoadCommand ?? (dataLoadCommand =
													 new Command<User>
													(
														async (user) =>
														{
                                                            if (IsBusy || Users.Count == 0)
																return;

															IsBusy = true;

															if (user == Users[Users.Count - 1])
															{
																await LoadData();
															}

															IsBusy = false;
														}
														, (user) =>
														 {
                                                            return !IsBusy;
														 }
													));
		#endregion

		#region Private Method
		private async Task LoadData()
		{
			try
			{
                var token = Settings.AuthToken;
				var users = await _userService.GetUsersAsync(token, page);
				if (users?.Count > 0)
				{
					if (page == 1)
						Users.Clear();

					Users.AddRange(users);
					page++;
				}
			}
            catch (ServiceAuthenticationException ex)
            {
                Debug.WriteLine(ex.Message);
                //await App.Current.MainPage.Navigation.PushAsync(new LoginPage());
				App.Current.MainPage = new NavigationPage(new LoginPage())
				{
					BarBackgroundColor = (Color)App.Current.Resources["Primary"],
					BarTextColor = Color.White
				};
            }
			catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
			{
				await Application.Current.MainPage.DisplayAlert("네트웍 에러 입니다.", "에러", "Ok");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error in: {ex}");
			}
		}
		#endregion
	}
}
