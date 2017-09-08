using System;
using System.Threading.Tasks;
using ApiClient.DataServices;
using ApiClient.DataServices.Base;
using ApiClient.DataServices.Interfaces;
using ApiClient.Helpers;
using ApiClient.ViewModels;
using ApiClient.Views;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace ApiClient
{
    public partial class App : Application
    {
        public static UnityContainer Container { get; private set; }

        public App()
        {
            InitializeComponent();
            Initialize();

            MainPage = new NavigationPage(new ApiClientPage());
            //SetMainPage();
        }

        public static void Initialize()
        {
            Container = new UnityContainer();
			Container.RegisterType<IRequestProvider, RequestProvider>();
			Container.RegisterType<IAccountProvider, AccountProvider>();
			Container.RegisterType<IJsonSettings, JsonSettings>();
			Container.RegisterType<IUserService, UserService>();
			Container.RegisterType<IAccountService, AccountService>();
        }

        public static void GoToMainPage()
        {
            Current.MainPage = new TabbedPage
            {
                Children = {
                    new NavigationPage(new UserListViewPage())
                    {
                        Title = "Browse",
                        Icon = Device.OnPlatform("tab_feed.png", null, null)
                    },
                    new NavigationPage(new AboutPage())
                    {
                        Title = "About",
                        Icon = Device.OnPlatform("tab_about.png", null, null)
                    },
                }
            };
        }
    }
}
