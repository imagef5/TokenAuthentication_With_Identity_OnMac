using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ApiClient.DataServices.Interfaces;
using ApiClient.Helpers;
using ApiClient.Views;
using Xamarin.Forms;

namespace ApiClient.ViewModels
{
    public class ApiClientPageViewModel : BaseViewModel
    {
        private string authtoken;
        private string username;
        private string password;
        private DateTime authTokenExpirationDate;
        private IAccountService _accountService;

        public ApiClientPageViewModel(IAccountService accountService)
        {
            _accountService = accountService;
            authtoken = Settings.AuthToken;
            username = Settings.UserName;
            password = Settings.Password;
            authTokenExpirationDate = Settings.AuthTokenExpirationDate;
        }

		public Command AppearingCommand => new Command(async () =>
																  {
																	  try
																	  {
                                                                          IsBusy = true;

                                                                          await Init();

                                                                          IsBusy = false;
																	  }
																	  catch (Exception ex)
																	  {
																		  //Message = "로그인을 할 수가 없습니다. 잠시후 다시 시도해 주세요.";
																		  Debug.WriteLine(ex.Message);
																	  }

																  }
													);

        public async Task Init()
        {
            if (!string.IsNullOrEmpty(authtoken))
			{
				var utcNow = DateTime.UtcNow.AddMinutes(5);
                if (authTokenExpirationDate < utcNow)
                {
                    authtoken = await _accountService.LoginAsync(username, password);

					Settings.AuthToken = authtoken;
                    Settings.UserName = username;
                    Settings.Password = password;
				}
                App.GoToMainPage();
			}
            else
			{
                App.Current.MainPage = new NavigationPage(new LoginPage())
				{
					BarBackgroundColor = (Color)App.Current.Resources["Primary"],
					BarTextColor = Color.White
				};
			}
		}
    }
}
