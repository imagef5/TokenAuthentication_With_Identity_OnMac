using System;
using System.Collections.Generic;
using ApiClient.ViewModels;
using Xamarin.Forms;
using Microsoft.Practices.Unity;

namespace ApiClient.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            BindingContext = App.Container.Resolve<LoginViewModel>();
        }
    }
}
