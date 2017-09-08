using ApiClient.ViewModels;
using Microsoft.Practices.Unity;

using Xamarin.Forms;

namespace ApiClient.Views
{
    public partial class UserListViewPage : ContentPage
    {
        public UserListViewPage()
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<UserListViewPageViewModel>();
        }
    }
}
