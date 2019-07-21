using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Hoard.SampleXamarin.Views;
using F3N.YaMVVM.ViewModel;
using F3N.Hoard.Storage;

namespace Hoard.SampleXamarin
{
    public partial class App : F3N.YaMVVM.App.YamvvmApp
    {

        public App()
        {
            InitializeComponent();
            LocalStorage.Initialise("Hoard.SampleXamarin");
            _ = ViewModelNavigation.SetTabbedMainPage<MainPage>();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
