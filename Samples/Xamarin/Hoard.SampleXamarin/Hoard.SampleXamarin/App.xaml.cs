using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Hoard.SampleXamarin.Views;
using F3N.YaMVVM.ViewModel;
using F3N.Hoard.Storage;
using Splat;
using F3N.Hoard.State;
using Hoard.SampleXamarin.Models;
using Hoard.SampleXamarin.Store;

namespace Hoard.SampleXamarin
{
    public partial class App : F3N.YaMVVM.App.YamvvmApp
    {

        public App()
        {
            InitialiseDependencies();
            InitializeComponent();
            _ = ViewModelNavigation.SetTabbedMainPage<MainPage>();
        }

        private void InitialiseDependencies()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new ItemStore(), typeof(ItemStore));
            LocalStorage.Initialise("Hoard.SampleXamarin");
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
