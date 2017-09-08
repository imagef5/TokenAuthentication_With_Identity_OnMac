using ApiClient.ViewModels;
using Xamarin.Forms;
using Microsoft.Practices.Unity;

namespace ApiClient
{
    public partial class ApiClientPage : ContentPage
    {
        public ApiClientPage()
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<ApiClientPageViewModel>();
        }
    }
}
