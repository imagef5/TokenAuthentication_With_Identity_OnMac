﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using ApiClient.Helpers;
using ApiClient.DataServices.Interfaces;
using Xamarin.Forms;

namespace ApiClient.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string userName;
        private string password;
        private string message = string.Empty;
        private IAccountService _accountService;
        private Command loginCommand;


        public LoginViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            UserName = Settings.UserName;
            Password = Settings.Password;
        }

        public string UserName
        {
            get 
            { 
                return userName; 
            }
            set 
            {
                SetProperty(ref userName, value);
                loginCommand?.ChangeCanExecute();
            }
        }

        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetProperty(ref password, value);
                loginCommand?.ChangeCanExecute();
            }
        }

        public string Message{
            get{
                return message;
            }
            set{
                SetProperty(ref message, value);
            }
        }

        public ICommand LoginCommand => 
                            loginCommand ?? (loginCommand = 
                                                new Command(async () =>
													              {
                                                                      IsBusy = true;
                                                                      try
                                                                      {
                                                                          var authtoken = await _accountService.LoginAsync(UserName, Password);

                                                                          Settings.AuthToken = authtoken;
                                                                          Settings.UserName = UserName;
                                                                          Settings.Password = Password;
                                                                          App.GoToMainPage();
														              }
															          catch(HttpRequestException ex)
															          {
		                                                                  Message = ex.Message;
															          }
                                                                      catch(Exception ex)
														              {
                                                                          Message = "로그인을 할 수가 없습니다. 잠시후 다시 시도해 주세요.";
                                                                          Debug.WriteLine(ex.Message);
														              }
                                                                      IsBusy = false;
													               },
													               () =>
													               {
													                   return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
													               }
                                                           )
                                            );


	}
}
